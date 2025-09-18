using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetInteger("Direction", 0); // up
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetInteger("Direction", 1); // down
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            animator.SetInteger("Direction", 2); // left
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            animator.SetInteger("Direction", 3); // right
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("DeadTrigger"); // dead
        }
    }
}
