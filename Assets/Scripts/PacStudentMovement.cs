using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float tileSize = 1f;
    private float distanceSinceLastStep = 0f;

    [Header("Targets")]
    public Transform topLeft;
    public Transform topRight;
    public Transform bottomLeft;
    public Transform bottomRight;

    [Header("Speed")]
    public float speed = 2f;

    
    private Vector3[] corners;
    private int currentTargetIndex = 0;

    [Header("Anim & Audio")]
    public Animator animator;
    public AudioSource moveAudio;

    void Start()
    {
        corners = new Vector3[]
        {
            topLeft.position,
            topRight.position,
            bottomRight.position,
            bottomLeft.position
        };
        transform.position = corners[0];
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    Vector3 target = corners[currentTargetIndex];
    //    Vector3 direction = (target - transform.position).normalized;

    //    // 移动 PacStudent
    //    transform.position += direction * speed * Time.deltaTime;

    //    // 播放动画，根据方向切换
    //    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
    //        animator.SetInteger("Direction", direction.x > 0 ? 3 : 2); // 右 =3, 左 =2
    //    else
    //        animator.SetInteger("Direction", direction.y > 0 ? 0 : 1); // 上 =0, 下 =1



    //    // 到达目标点，切换下一个
    //    if (Vector3.Distance(transform.position, target) < 0.01f)
    //    {
    //        transform.position = target;
    //        currentTargetIndex = (currentTargetIndex + 1) % corners.Length;
    //    }
    //}

    void Update()
    {
        Vector3 target = corners[currentTargetIndex];
        Vector3 direction = (target - transform.position).normalized;

        // 计算本帧移动距离
        float moveThisFrame = speed * Time.deltaTime;
        transform.position += direction * moveThisFrame;

        // 累计移动距离
        distanceSinceLastStep += moveThisFrame;

        // 每走 tileSize 距离就播放一次声音
        if (distanceSinceLastStep >= tileSize)
        {
            if (moveAudio != null)
                moveAudio.Play();

            distanceSinceLastStep = 0f; // 重置计数
        }

        // 播放动画
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            animator.SetInteger("Direction", direction.x > 0 ? 3 : 2);
        else
            animator.SetInteger("Direction", direction.y > 0 ? 0 : 1);

        // 到达目标点，切换下一个
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            transform.position = target;
            currentTargetIndex = (currentTargetIndex + 1) % corners.Length;
        }
    }

}
