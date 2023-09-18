using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class movements : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    //Grappling
    [Header("Grappling")]
    public bool freeze;
    public bool activeGrapple;
    private Vector3 velocityToSet;


    [Header("camera")]
    public Transform orientation;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private bool isAbleToSelect;


    //must be assigned in the inspector
    public GameObject bootPrefab;
    public GameObject wingsPrefab;
    public GameObject spawner;

    //items attached to the player
    private GameObject boot;
    private GameObject wings;

    //items
    private Boot bootComponent;
    private Wings wingsComponent;

    //items images
    private Image[] itemsImages;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        //getting the items Prefabs
        bootComponent = GameObject.Find("Player").GetComponent<Boot>();
        wingsComponent = GameObject.Find("Player").GetComponent<Wings>();

        //getting the Player items

        //the index must be changed 
        boot = gameObject.transform.GetChild(1).gameObject;
        wings = gameObject.transform.GetChild(2).gameObject;

        //ability to select items
        isAbleToSelect = false;

        
    }

    void Update()
    {
        //Basic Movements
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded && !activeGrapple)
            rb.drag = groundDrag;
        else
            rb.drag = 0;


        //Display SacrifceMenuImage
        if (Input.GetKeyDown(KeyCode.E) && GameManager.isInsidePortal)
        {
            foreach (Image image in GameManager.sacrificeMenu.GetComponentsInChildren<Image>())
            {
                image.enabled = true;
            }
            isAbleToSelect = true;
        }

        itemsImages = GameManager.sacrificeMenu.GetComponentsInChildren<Image>();

        //disable boot
        if (Input.GetKeyDown(KeyCode.Keypad2) & isAbleToSelect)
        {
            itemsImages[1].color = Color.red;

            bootComponent.enabled = false;

            Destroy(boot);
            Instantiate(bootPrefab, spawner.transform.position, Quaternion.identity);

            isAbleToSelect = false;

            GameManager.portal.enabled = true;
        }

        //disable wings
        if (Input.GetKeyDown(KeyCode.Keypad1) && isAbleToSelect)
        {
            itemsImages[0].color = Color.red;

            wingsComponent.enabled = false;

            Destroy(wings);
            Instantiate(wingsPrefab, spawner.transform.position, Quaternion.identity);

            isAbleToSelect = false;
            GameManager.portal.enabled = true;
        }

        if (GameManager.isOutsidePortal == false)
        {
            Debug.Log("outside");
            GameManager.portal.SetBool("isClosed", true);
        }

        //Grappling
        if (freeze)
        {
            rb.velocity = Vector3.zero;
        }

    }


    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private bool enableMovmentOnNextTouch;
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovmentOnNextTouch)
        {
            enableMovmentOnNextTouch = false;
            ResetRestrictions();

            GetComponent<Grappling>().StopGrapple();
        }
    }
    private void SetVelocity()
    {
        enableMovmentOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        if (activeGrapple) return;
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}
