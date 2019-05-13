using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    Rigidbody rigidbody;

    [SerializeField]
    float speed = 5;
    [SerializeField]
    float jumpForce = 50;


    bool canJump = true;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.velocity = new Vector3(Input.GetAxis("Horizontal") * speed, rigidbody.velocity.y, Input.GetAxis("Vertical") * speed);
        

        if (Input.GetAxis("Jump") > 0 && canJump)
        {
            Jump();
        }
    }


    void Jump()
    {
        canJump = false;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpForce, rigidbody.velocity.z);
    }



    void OnCollisionEnter(Collision collision)
    {
        canJump = true;
    }
}
