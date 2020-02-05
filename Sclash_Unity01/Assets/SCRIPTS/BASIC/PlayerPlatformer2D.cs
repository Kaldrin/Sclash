using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformer2D : MonoBehaviour
{
    #region VARIABLES
    [Header("PLAYER COMPONENTS")]
    [SerializeField] Rigidbody2D rigidBody2D = null;
    [SerializeField] Collider2D
        groundDetectionTriggerCollider = null,
        ceilingDetectionTriggerCollider = null,
        wallDetectionTriggerCollider = null;




    [Header("INPUTS")]
    [SerializeField] string horirontalAxisName = "Horizontal";
    [SerializeField] string jumpAxisName = "Jump";

    float horizontalInput = 0;




    [Header("PLAYER STATES")]
    [SerializeField] PLAYERSTATE currentPlayerState = PLAYERSTATE.groundedNormal;
    PLAYERSTATE oldPlayerState = PLAYERSTATE.groundedNormal;

    enum PLAYERSTATE
    {
        groundedNormal,
        inAir,
        onWall,
        wallJumping,
    }




    [Header("GROUND MOVEMENTS")]
    [SerializeField] float playerGroundHorizontalMovementsSpeed = 12f;
    float playerBaseZPosition = 0;




    
    [Header("AIR MOVEMENTS")] 
    [SerializeField] Vector2 baseMinAndMaxVerticalVelocityLimiters = new Vector2(-10, 10);
    [SerializeField] Vector2 onWallMinAndMaxVerticalVelocityLimiters = new Vector2(-5, 10);
    Vector2 actualUsedminAndMaxVerticalVelocityLimiters = new Vector2(0, 0);





    [Header("JUMP")]
    [SerializeField] Vector2 fromWallJumpPower = new Vector2(2, 2);
    [SerializeField] float
        fromGroundJumpPower = 10f,
        fromAirJumpPower = 2f,
        maxJumpInputDuration = 0.4f;
    [SerializeField] float jumpInputStartTime = 0f;

    [SerializeField] int baseMaxJumpNumber = 2;
    int currentJumpNumberLeft = 0;

    bool rising = false;


    [Header("WALL JUMP")]
    [SerializeField] float wallJumpDuration = 0.2f;
    float wallJumpStartTime = 0;



    [Header("WALL")]
    [SerializeField] float playerOnWallGravity = 1f;
    float basePlayerGravity = 9.6f;





    [Header("TAGS")]
    [SerializeField] string groundTag = "Ground";
    [SerializeField] string wallTag = "Wall";
    #endregion












    #region FUNCTIONS
    #region BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        playerBaseZPosition = transform.position.z;
        ResetJumps();
        actualUsedminAndMaxVerticalVelocityLimiters = baseMinAndMaxVerticalVelocityLimiters;
    }

    // Update is called once per frame
    void Update()
    {
        // Keep player Z position to 0, safe
        transform.position = new Vector3(transform.position.x, transform.position.y, playerBaseZPosition);


        switch (currentPlayerState)
        {
            case PLAYERSTATE.groundedNormal:
                ManageHorizontalMovements();
                ManageJump();
                break;
            case PLAYERSTATE.inAir:
                ManageJump();
                ManageHorizontalMovements();
                break;

            case PLAYERSTATE.onWall:
                ManageJump();
                ManageHorizontalMovements();
                break;

            case PLAYERSTATE.wallJumping:
                ManageJump();
                break;
        }
    }


    private void FixedUpdate()
    {
        switch (currentPlayerState)
        {
            case PLAYERSTATE.groundedNormal:
                ManageOrientation();
                if (!groundDetectionTriggerCollider.IsTouchingLayers())
                    SwitchState(PLAYERSTATE.inAir);
                break;
            case PLAYERSTATE.inAir:
                LimitVerticalSpeed();
                ManageOrientation();

                if (wallDetectionTriggerCollider.IsTouchingLayers() && rigidBody2D.velocity.y < 0)
                    SwitchState(PLAYERSTATE.onWall);
                break;

            case PLAYERSTATE.onWall:
                LimitVerticalSpeed();
                if (!wallDetectionTriggerCollider.IsTouchingLayers() || rigidBody2D.velocity.y > 0)
                    SwitchState(PLAYERSTATE.inAir);
                break;

            case PLAYERSTATE.wallJumping:
                LimitVerticalSpeed();
                ManageOrientation();
                if (wallDetectionTriggerCollider.IsTouchingLayers() && rigidBody2D.velocity.y < 0)
                    SwitchState(PLAYERSTATE.onWall);
                if (Time.time + wallJumpDuration >= wallJumpStartTime)
                    SwitchState(PLAYERSTATE.inAir);
                break;
        }
    }
    #endregion




    #region PLAYER STATE
    void SwitchState(PLAYERSTATE newState)
    {
        oldPlayerState = currentPlayerState;
        currentPlayerState = newState;

        if (oldPlayerState == PLAYERSTATE.onWall)
        {
            rigidBody2D.gravityScale = basePlayerGravity;
            actualUsedminAndMaxVerticalVelocityLimiters = baseMinAndMaxVerticalVelocityLimiters;
        }


        switch (newState)
        {
            case PLAYERSTATE.groundedNormal:
                ResetJumps();
                break;
            case PLAYERSTATE.inAir:
                break;
            case PLAYERSTATE.onWall:
                ResetJumps();
                rigidBody2D.gravityScale = playerOnWallGravity;
                rigidBody2D.velocity = Vector2.zero;
                actualUsedminAndMaxVerticalVelocityLimiters = onWallMinAndMaxVerticalVelocityLimiters;
                break;

            case PLAYERSTATE.wallJumping:
                wallJumpStartTime = Time.time;
                break;
        }
    }
    #endregion




    #region COLLISIONS
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentPlayerState == PLAYERSTATE.inAir)
        {
            if (groundDetectionTriggerCollider.IsTouchingLayers())
            {
                SwitchState(PLAYERSTATE.groundedNormal);
            }


            if (ceilingDetectionTriggerCollider.IsTouchingLayers())
            {
                rising = false;
            }


            if (wallDetectionTriggerCollider.IsTouchingLayers() && collision.CompareTag(wallTag))
            {
                if (rigidBody2D.velocity.y <= 0)
                    SwitchState(PLAYERSTATE.onWall);
            }
        }

        
        if (currentPlayerState == PLAYERSTATE.onWall)
        {
            if (groundDetectionTriggerCollider.IsTouchingLayers())
            {
                SwitchState(PLAYERSTATE.groundedNormal);
            }
        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (currentPlayerState == PLAYERSTATE.onWall)
        {
            if (collision.CompareTag(wallTag))
            {
                if (rigidBody2D.velocity.y <= 0)
                    SwitchState(PLAYERSTATE.inAir);
            }
        }
    }
    #endregion



    void ManageHorizontalMovements()
    {
        horizontalInput = Input.GetAxis(horirontalAxisName);
        if (currentPlayerState == PLAYERSTATE.onWall && Mathf.Abs(horizontalInput) > 0)
        {
            Vector2 newPlayerSpeed = new Vector2(horizontalInput * playerGroundHorizontalMovementsSpeed, rigidBody2D.velocity.y);
            rigidBody2D.velocity = newPlayerSpeed;
        }
        else if (currentPlayerState != PLAYERSTATE.onWall)
        {
            Vector2 newPlayerSpeed = new Vector2(horizontalInput * playerGroundHorizontalMovementsSpeed, rigidBody2D.velocity.y);
            rigidBody2D.velocity = newPlayerSpeed;
        }
    }



    #region JUMP
    void ManageJump()
    {
        if (Input.GetButtonDown(jumpAxisName) && currentJumpNumberLeft > 0)
        {
            rising = true;
            jumpInputStartTime = Time.time;
            currentJumpNumberLeft--;

            if (currentPlayerState == PLAYERSTATE.onWall)
                SwitchState(PLAYERSTATE.wallJumping);

        }


        if (Input.GetButtonUp(jumpAxisName))
            rising = false;


        if (rising)
        {
            Vector2 newPlayerJumpVelocity = new Vector2();



            if (currentPlayerState == PLAYERSTATE.wallJumping)
            {
                newPlayerJumpVelocity = new Vector2(fromWallJumpPower.x * - Mathf.Sign(transform.localScale.x), fromWallJumpPower.y);
                Debug.Log(newPlayerJumpVelocity);
                newPlayerJumpVelocity = new Vector2(fromWallJumpPower.x * -Mathf.Sign(transform.localScale.x), fromWallJumpPower.y);

            }
            else if (currentJumpNumberLeft >= baseMaxJumpNumber - 1)
                newPlayerJumpVelocity = new Vector2(rigidBody2D.velocity.x, fromGroundJumpPower);
            else if (currentPlayerState == PLAYERSTATE.inAir)
                newPlayerJumpVelocity = new Vector2(rigidBody2D.velocity.x, fromAirJumpPower);


            rigidBody2D.velocity = newPlayerJumpVelocity;


            if (Time.time > jumpInputStartTime + maxJumpInputDuration)
            {
                rising = false;
            }
        }
    }

    void ResetJumps()
    {
        currentJumpNumberLeft = baseMaxJumpNumber;
    }
    #endregion


    void LimitVerticalSpeed()
    {
        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Mathf.Clamp(rigidBody2D.velocity.y, actualUsedminAndMaxVerticalVelocityLimiters.x, actualUsedminAndMaxVerticalVelocityLimiters.y));
    }

    void ManageOrientation()
    {
        if (Mathf.Abs(rigidBody2D.velocity.x) > 0)
        {

            Vector3 newPlayerScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(rigidBody2D.velocity.x), transform.localScale.y, transform.localScale.z);
            transform.localScale = newPlayerScale;
        }
    }
    #endregion
}
