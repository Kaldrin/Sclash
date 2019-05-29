using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{

    // COMPONENTS
    Rigidbody rigid;


    // CAMERA
    [SerializeField] float sensitivity = 3;
    [SerializeField] GameObject cameraArm;


    // MOVEMENTS
    [SerializeField] float speed;


    // JUMP
    [SerializeField] float jumpForce = 100;
    bool canJump = true;




    // Start is called before the first frame update
    void Start()
    {
        // Getting components
        rigid = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }



    // Update is called once per frame
    void Update()
    {
        // MOVEMENTS
        ManageMovements();

        // CAMERA
        ManageCamera();

        // JUMP
        if (Input.GetButtonDown("Jump") && canJump)
            Jump();
    }





    // MOVEMENTS
    void ManageMovements()
    {
        Vector3 movementX = Input.GetAxis("Horizontal1") * speed * transform.right,
            movementZ = Input.GetAxis("Vertical1") * speed * transform.forward;





        rigid.velocity = new Vector3(0, rigid.velocity.y, 0) + movementX + movementZ;
    }





    // JUMP
    void Jump()
    {
        rigid.velocity = new Vector3(rigid.velocity.x, jumpForce, rigid.velocity.z);

    }

    void OnTriggerEnter(Collider other)
    {
        canJump = true;
    }

    void OnTriggerStay(Collider other)
    {
        canJump = true;
    }




    // CAMERA
    void ManageCamera()
    {
        // Getting mouse movements
        float x = Input.GetAxis("Mouse X") * sensitivity,
            y = Input.GetAxis("Mouse Y") * sensitivity;

        // Getting camera base rotation
        Vector3 playerBaseRot = transform.eulerAngles;
        Vector3 baseCameraArmRot = cameraArm.transform.localEulerAngles;



        // Reassigning camera rotation to move with mouse
        transform.eulerAngles = new Vector3(playerBaseRot.x, playerBaseRot.y + x, playerBaseRot.z);  
        cameraArm.transform.localEulerAngles = new Vector3(baseCameraArmRot.x + y, 0, 0);
    }
}
