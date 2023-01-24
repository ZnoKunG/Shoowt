using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayerMask;
    bool grounded;

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDir;
    public Transform orientation;

    // Jumping
    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    bool isReadyToJump;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    void Awake() {
        rb = GetComponent<Rigidbody>();
        isReadyToJump = true;
    }

    public void Update() {
        GetInput();
        CheckGround();
        SpeedControl();
    }

    // Add Force needs to be in FixedUpdate
    public void FixedUpdate() {
        MovePlayer();
    }

    void GetInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Debug.Log($"Button press: {Input.GetKey(jumpKey)}, isReadyToJump: {isReadyToJump}, Grounded: {grounded}");
        if (Input.GetKey(jumpKey) && isReadyToJump && grounded) {
            isReadyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }
    }

    void MovePlayer() {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        Vector3 force = moveDir * moveSpeed * 10f;
        if (!grounded) {
            force *= airMultiplier;
        }
        rb.AddForce(force, ForceMode.Force);
    }

    void CheckGround() {
        grounded = Physics.Raycast(transform.position, Vector3.down, (playerHeight / 2) + 0.2f, groundLayerMask);

        // No drag while in the air
        if (grounded) {
            rb.drag = groundDrag;
        } else {
            rb.drag = 0;
        }
    }

    void SpeedControl() {
        Vector3 velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Limite the velocity (if needed)
        if (velocity.magnitude > moveSpeed) {
            Vector3 limitedVelocity = velocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    void Jump() {
        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Apply Force Once (Impulse)
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump() {
        isReadyToJump = true;
    }
}
