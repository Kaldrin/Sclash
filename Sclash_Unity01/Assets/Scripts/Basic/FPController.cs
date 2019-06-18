using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPController : MonoBehaviour {

    //Camera controls
    public new GameObject camera = null;
    public Vector2 cameraControlSpeed = new Vector2(1, 1);

    //Player base controls
    Rigidbody playerRigidBody;
    public Vector2
        playerWalkSpeed = new Vector2(4, 4),
        playerRunSpeed = new Vector2(9, 9);
    Vector2 actualPlayerSpeed;
    public Vector3 jumpForce;
    public int jumpNumber;
    int actualJumpNumber;
    

    // Use this for initialization
    void Start () {
        playerRigidBody = GetComponent<Rigidbody>();
        actualJumpNumber = jumpNumber;
        actualPlayerSpeed = playerWalkSpeed;

        CreateStaminaBar();
    }
	
	// Update is called once per frame
	void Update () {
        Run();
    }

    void FixedUpdate()
    {
        CameraRotate();
        MovePlayer();
        Jump();  
    }


    //Collisions
    void OnTriggerEnter(Collider triggerCollider)
    {
        actualJumpNumber = jumpNumber;
    }

    //Move the player
    void CameraRotate()
    {
        float
            cameraXMovement = - Input.GetAxis("Mouse Y") * cameraControlSpeed.y,
            cameraXNextPosition = cameraXMovement + camera.transform.localEulerAngles.x;


        if (cameraXNextPosition < 90 || cameraXNextPosition > 270)
            camera.transform.Rotate(cameraXMovement, 0, 0);

        transform.Rotate(0, Input.GetAxis("Mouse X") * cameraControlSpeed.x, 0);
    }

    void MovePlayer()
    {
        Vector3 playerVelocity;

        playerVelocity.z = actualPlayerSpeed.y * Input.GetAxis("Vertical");
        playerVelocity.x = actualPlayerSpeed.x * Input.GetAxis("Horizontal");
        playerVelocity.y = playerRigidBody.velocity.y;

        //playerRigidBody.AddForce = transform.forward * playerVelocity.y + transform.right * playerVelocity.x + new Vector3(0, playerRigidBody.velocity.y, 0);

        transform.Translate(playerVelocity * Time.deltaTime, Space.Self);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && actualJumpNumber > 0)
        {
            Vector3 velocity = playerRigidBody.velocity;

            playerRigidBody.velocity = new Vector3(velocity.x, 0, velocity.z);
            playerRigidBody.AddForce(jumpForce);
            actualJumpNumber--;
        }
    }

    void Run()
    {
        if (Input.GetButtonDown("Run"))
        {
            if (Input.GetAxis("Vertical") > 0 && actualPlayerSpeed == playerWalkSpeed)
            {
                actualPlayerSpeed = playerRunSpeed;
            }
            else if (actualPlayerSpeed != playerWalkSpeed)
            {
                actualPlayerSpeed = playerWalkSpeed;
            }
        }
        else if (Input.GetAxis("Vertical") == 0)
        {
            actualPlayerSpeed = playerWalkSpeed;
        }
    }


    //Stamina

    void CreateStaminaBar()
    {
        Transform camTransform = camera.transform;
        GameObject staminaBar;

        staminaBar = Instantiate(new GameObject("staminaBar"), camTransform.position + new Vector3(0, 5, 0), camTransform.rotation, camTransform);

        staminaBar.AddComponent<SpriteRenderer>();
    }
}
