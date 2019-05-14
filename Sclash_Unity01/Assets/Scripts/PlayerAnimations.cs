using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D rigid;


    [SerializeField]
    Animator animator;
    [SerializeField]
    string walkBool = "Walk";
    [SerializeField]
    float speedForWalking = 0.5f;

    // Update is called once per frame
    void Update()
    {
        UpdateAnims();
    }

    void UpdateAnims()
    {
        if (Mathf.Abs(rigid.velocity.x) > speedForWalking)
        {
            animator.SetBool(walkBool, true);
        }
        else
        {
            animator.SetBool(walkBool, false);
        }
    }


}
