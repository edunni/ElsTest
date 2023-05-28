using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlternatePlayerMovement : MonoBehaviour
{
    [Header("Alternate")]
    [Header("Walking")]
    private float moveSpeed;
    public float sprintSpeed;
    public float walkSpeed;
    public float sprintTimeTheshold = 0.25f;
    private bool shouldSprint = false;
    private bool isSprinting = false;
    private bool sprintTemp = false;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump = true;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchScale;
    private float startScale;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;
    public float groundDrag;

    [Header("Keys")]
    public KeyCode jumpKey = KeyCode.UpArrow;
    public KeyCode crouchKey = KeyCode.DownArrow;
    public KeyCode keyGoLeft = KeyCode.LeftArrow;
    public KeyCode keyGoRight = KeyCode.RightArrow;

    public Transform orientation;
    public TMP_Text myText;

    private Vector3 moveDir;
    private Rigidbody rb;
    private float horizontaInput;
    private float verticalInput;
    
    public MovementState moveState;
    public GroundedState groundState;

    bool A;
    bool B;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching
    }

    public enum GroundedState{
        ground,
        air
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startScale = transform.localScale.y;
        
    }

    void Update()
    {
        MoveInput();
        SpeedControl();
        StateHandler();
        MoveSpeedStateHandler();
        DragGrounded();
        IsPlayerSprinting();
        
        myText.text = "Speed: " + moveSpeed;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        
        moveDir = orientation.right * horizontaInput;

        if(grounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }    
    }

    private void DragGrounded(){

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded){
            rb.drag = groundDrag;
        } else { 
            rb.drag = 0;
        }
    }

    private void StateHandler(){
        //ground
        if (grounded){
            groundState = GroundedState.ground;
        } else { 
            groundState = GroundedState.air;
        }


        if (isSprinting && moveState != MovementState.crouching)
        { 
            moveState = MovementState.sprinting; 
        } //sprinting
        else if (grounded && Input.GetKey(crouchKey))
        { 
            moveState = MovementState.crouching; 
        } //crouch
        else 
        { 
            moveState = MovementState.walking; 
        } //walk
    }

    private void MoveSpeedStateHandler()
    {
        switch (moveState)
        {
            case MovementState.walking:
                if(groundState == GroundedState.ground)
                {
                    Debug.Log("Player is walking");
                    moveSpeed = walkSpeed;
                }
                break;
            case MovementState.sprinting:
                Debug.Log("Player is sprinting");
                moveSpeed = sprintSpeed;
                break;
            case MovementState.crouching:
                if(groundState == GroundedState.ground)
                {
                    Debug.Log("Player is crouching");
                    moveSpeed = crouchSpeed;
                }
                break;
            default:
                Debug.Log("Player doesnt exist wtf");
                break;
        } 
    }

    private void MoveInput()
    {
        horizontaInput = Input.GetAxisRaw("Horizontal");
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(Input.GetKey(crouchKey))
        {
            rb.AddForce(Vector3.down * 0.1f, ForceMode.Impulse);
        }

        if(Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchScale, 0f);
            rb.AddForce(Vector3.down * 6f, ForceMode.Impulse);
        }

        if(Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startScale, 0f);
        }
    }

    private void IsPlayerSprinting(){
        if (!shouldSprint) 
        { 
            if (Input.GetKeyDown(keyGoLeft) || Input.GetKeyDown(keyGoRight)) 
            { 
                StartCoroutine(CheckSprint()); 
            } 
        } 
        else 
        { 
            if (Input.GetKeyDown(keyGoLeft) || Input.GetKeyDown(keyGoRight)) 
            { 
                isSprinting = true; 
                sprintTemp = true;
            } 
        } 
        if (isSprinting) 
        {
            if(sprintTemp)
            {
                Debug.Log("Player double clicked to sprint");
                sprintTemp = false;
            } 
        } 
        if(Input.GetKeyUp(keyGoLeft) || Input.GetKeyUp(keyGoRight))
        { 
            isSprinting = false;
            sprintTemp = false; 
        } 
    }

    IEnumerator CheckSprint()
    { 
        shouldSprint = true; 
        yield return new WaitForSeconds(sprintTimeTheshold); 
        shouldSprint = false; 
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, 0f);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, 0f);

        if(flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    } 
}
