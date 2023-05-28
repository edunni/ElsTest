using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Walking")]
    private float moveSpeed;
    public float sprintSpeed;
    public float walkSpeed;
    public float doubleClickSprintTimeThreshold = 0.25f;
    private float lastClickTime = 0f;
    private bool isSprinting = false;

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
    
    private float timerName;
    private bool startTimer = true;
    
    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        air,
        crouching
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startScale = transform.localScale.y;

        timerName = 20;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MoveInput();
        SpeedControl();
        StateHandler();
        // checkSprint();

        if(startTimer==true) {
            timerName -= 1*Time.deltaTime;
        }
        if((timerName>=0)&&(timerName<=1)) {
            startTimer = false;
            timerName = 0;
        }
    
        myText.text = "Speed: " + moveSpeed + " Time: " + timerName;

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;        
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MoveInput()
    {
        horizontaInput = Input.GetAxisRaw("Horizontal");
        // verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
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

    // private void checkSprint(){
    //     if ((grounded && Input.GetKeyDown(keyGoLeft) || grounded && Input.GetKeyDown(keyGoRight)))
    //     {
    //         float timeSinceLastClick = Time.time - lastClickTime;

    //         if (timeSinceLastClick <= doubleClickSprintTimeThreshold)
    //         {
    //             // Double-click detected
    //             Debug.Log("Double-clicked left or right");
    //             isSprinting = true;
    //         } else { isSprinting = false; }
    //     } 
    // }

    private void StateHandler()
    {
        //checking if double clicked -> if yes, sprint
        if ((grounded && Input.GetKeyDown(keyGoLeft) || grounded && Input.GetKeyDown(keyGoRight)))
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= doubleClickSprintTimeThreshold)
            {
                // Double-click detected
                Debug.Log("Double-clicked left or right");
                state = MovementState.sprinting;
                moveSpeed = sprintSpeed;
            } 
        } 
        // if (isSprinting)
        // {
        //     state = MovementState.sprinting;
        //     moveSpeed = sprintSpeed;
        // }

        else if (grounded && Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        else
        {
            state = MovementState.air;
        }
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