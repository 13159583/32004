using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float tileSize = 1f;
    private float distanceSinceLastStep = 0f;

    // Set showcase positions
    [Header("Targets")]
    public Transform topLeft;
    public Transform topRight;
    public Transform bottomLeft;
    public Transform bottomRight;

    // Set speed
    [Header("Speed")]
    public float speed = 2f;

    
    private Vector3[] corners;
    private int currentTargetIndex = 0;

    // Set anim and audio
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

    void Update()
    {
        // get destination
        Vector3 target = corners[currentTargetIndex];
        Vector3 direction = (target - transform.position).normalized;

        float moveThisFrame = speed * Time.deltaTime;
        transform.position += direction * moveThisFrame;

        distanceSinceLastStep += moveThisFrame;

        // play walk sfx every step
        if (distanceSinceLastStep >= tileSize)
        {
            if (moveAudio != null)
                moveAudio.Play();

            distanceSinceLastStep = 0f; 
        }

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            animator.SetInteger("Direction", direction.x > 0 ? 3 : 2);
        else
            animator.SetInteger("Direction", direction.y > 0 ? 0 : 1);

        // fix position
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            transform.position = target;
            currentTargetIndex = (currentTargetIndex + 1) % corners.Length;
        }
    }

}
