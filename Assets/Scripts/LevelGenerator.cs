using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    // get manual maps
    [Header("Tilemaps")]
    public Tilemap[] wallTilemaps;

    // get map tiles
    [Header("Wall Tiles")]
    public TileBase empty;
    public TileBase outsideCorner;
    public TileBase outsideWall;
    public TileBase insideCorner;
    public TileBase insideWall;
    public TileBase standardPellet;
    public TileBase powerPellet;
    public TileBase tJunction;
    public TileBase ghostExitWall;

    const int e_empty = 0;
    const int outside_corner = 1;
    const int outside_wall = 2;
    const int inside_corner = 3;
    const int inside_wall = 4;
    const int standard_pellet = 5;
    const int power_pellet = 6;
    const int t_junction = 7;
    const int ghost_exit_wall = 8;

    // load map array
    private int[,] levelMap =
        {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,8},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
        };

    void Start()
    {
        DestroyAllTilemaps();
        GenerateLevel();
    }
    void DestroyAllTilemaps()
    {
        foreach (Tilemap t in wallTilemaps)
            if (t != null) t.ClearAllTiles();
    }
    void GenerateLevel()
    {
        int h = levelMap.GetLength(0);
        int w = levelMap.GetLength(1);

        int finalH = h * 2;
        int finalW = w * 2;
        int[,] finalMap = new int[finalH, finalW];

        // flip out whole map
        for (int row = 0; row < h; row++)
        {
            for (int col = 0; col < w; col++)
            {
                int val = levelMap[row, col];
                finalMap[row, col] = val; // topleft
                finalMap[row, finalW - 1 - col] = val; // topright 
                finalMap[finalH - 1 - row, col] = val; // bottomleft
                finalMap[finalH - 1 - row, finalW - 1 - col] = val; // bottomright
            }
        }

        // move bottom pieces one row above
        for (int row = h; row < finalH; row++)
        {
            for (int col = 0; col < finalW; col++)
            {
                finalMap[row - 1, col] = finalMap[row, col];
                finalMap[row, col] = e_empty;
            }
        }

        // turn each map tile to correct pos
        for (int row = 0; row < finalH; row++)
        {
            for (int col = 0; col < finalW; col++)
            {
                int code = finalMap[row, col];
                TileBase tile = GetTileBaseFromCode(code);
                if (tile != null)
                {
                    Vector3Int pos = new Vector3Int(col, finalH - 1 - row, 0);

                    foreach (Tilemap t in wallTilemaps)
                    {
                        if (t == null) continue;
                        t.SetTile(pos, tile);
                        RotateTile(finalMap, row, col, t);
                    }
                }
            }
        }


        AdjustCameraToBounds(finalW, finalH);
    }

    // turning function
    void RotateTile(int[,] map, int row, int col, Tilemap tilemap)
    {
        int tile = map[row, col];
        Vector3Int pos = new Vector3Int(col, map.GetLength(0) - 1 - row, 0);
        float angle = 0f;
        Vector3 scale = Vector3.one;

        int h = map.GetLength(0);
        int w = map.GetLength(1);

        // get wall for four diretion
        bool upWall = IsWall(map, row - 1, col);
        bool downWall = IsWall(map, row + 1, col);
        bool leftWall = IsWall(map, row, col - 1);
        bool rightWall = IsWall(map, row, col + 1);

        switch (tile)
        {
            case outside_corner:
                if (upWall && leftWall) angle = 180;
                else if (upWall && rightWall) angle = 90;
                else if (downWall && rightWall) angle = 0;
                else if (downWall && leftWall) angle = 270;
                break;

            case inside_corner:
            {
                int wallCount = (upWall ? 1 : 0) + (downWall ? 1 : 0) + (leftWall ? 1 : 0) + (rightWall ? 1 : 0);
                if (wallCount == 2) // if two walls attached
                {
                    if (upWall && leftWall) angle = 180;
                    else if (upWall && rightWall) angle = 90;
                    else if (downWall && rightWall) angle = 0;
                    else if (downWall && leftWall) angle = 270;
                }
                else if (wallCount == 4) // need to detect four oblique direation if attached four walls
                {
                    // get empty oblique diretion block
                    bool upLeftEmpty = !IsWall(map, row - 1, col - 1);
                    bool downLeftEmpty = !IsWall(map, row + 1, col - 1);
                    bool downRightEmpty = !IsWall(map, row + 1, col + 1);
                    bool upRightEmpty = !IsWall(map, row - 1, col + 1);

                    if (upLeftEmpty) angle = 180;
                    else if (downLeftEmpty) angle = 270;
                    else if (downRightEmpty) angle = 0;
                    else if (upRightEmpty) angle = 90;
                }
            }
            break;


            case outside_wall:
                angle = (leftWall || rightWall) ? 0 : (upWall || downWall ? 90 : 0);
                break;

            case inside_wall:
                if ((leftWall && rightWall)) angle = 0;   // ˮƽ
                else if ((upWall && downWall)) angle = 90;
                break;

            case t_junction:
            {
                // detect if outside wall/corner attached
                bool upOutside = IsOutsideWall(map, row - 1, col);
                bool downOutside = IsOutsideWall(map, row + 1, col);
                bool leftOutside = IsOutsideWall(map, row, col - 1);
                bool rightOutside = IsOutsideWall(map, row, col + 1);


                // listout all possibilities
                if (leftOutside && rightWall && downWall)
                    angle = 0;           
                else if (downOutside && rightWall && upWall)
                    angle = 90;
                else if (rightOutside && leftWall && upWall)
                    angle = 180;
                else if (upOutside && leftWall && downWall)
                    angle = 270;
                else if (leftOutside && rightWall && upWall) // filp if needed
                {
                    scale = new Vector3(1, -1, 1);
                    angle = 0;
                }
                else if (downOutside && upWall && leftWall)
                {
                    scale = new Vector3(1, -1, 1);
                    angle = 90;
                }
                else if (rightOutside && leftWall && downWall)
                {
                    scale = new Vector3(1, -1, 1);
                    angle = 180;
                }
                else if (upOutside && downWall && rightWall)
                {
                    scale = new Vector3(1, -1, 1);
                    angle = 270;
                }
                               
            }
            break;
        }

        tilemap.SetTransformMatrix(pos, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angle), scale));
    }

    // detect if the block is wall
    private bool IsWall(int[,] map, int r, int c)
    {
        int h = map.GetLength(0);
        int w = map.GetLength(1);
        if (r < 0 || r >= h || c < 0 || c >= w) return false;
        int code = map[r, c];
        return code == outside_wall || code == inside_wall || code == outside_corner || code == inside_corner || code == t_junction;
    }

    // detect if the block is outsidewall
    private bool IsOutsideWall(int[,] map, int r, int c)
    {
        int h = map.GetLength(0);
        int w = map.GetLength(1);
        if (r < 0 || r >= h || c < 0 || c >= w) return false;
        int code = map[r, c];
        return code == outside_wall || code == outside_corner;
    }

    // detect if its insidewall
    private bool IsInsideWall(int[,] map, int r, int c)
    {
        int h = map.GetLength(0);
        int w = map.GetLength(1);
        if (r < 0 || r >= h || c < 0 || c >= w) return false;
        int code = map[r, c];
        return code == inside_wall || code == inside_corner;
    }

    // place map tile to map accordingly
    TileBase GetTileBaseFromCode(int code)
    {
        switch (code)
        {
            case e_empty: return empty;
            case outside_corner: return outsideCorner;
            case outside_wall: return outsideWall;
            case inside_corner: return insideCorner;
            case inside_wall: return insideWall;
            case standard_pellet: return standardPellet;
            case power_pellet: return powerPellet;
            case t_junction: return tJunction;
            case ghost_exit_wall: return ghostExitWall;
            default: return null;
        }
    }

    // camerasetup
    void AdjustCameraToBounds(int width, int height)
    {
        Camera cam = Camera.main;
        if (cam == null || !cam.orthographic) return;

        float aspect = cam.aspect;
        float sizeByHeight = height / 2f;
        float sizeByWidth = (width / 2f) / aspect;
        cam.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth) + 0.5f; // padding
        cam.transform.position = new Vector3((width - 1) / 2f, (height - 1) / 2f, cam.transform.position.z);

    }
}
