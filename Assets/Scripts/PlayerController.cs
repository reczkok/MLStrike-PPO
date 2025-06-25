using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerShoot))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 18f;
    public float jumpForce = 5f;

    [Header("Look Settings")]
    public Transform eyes;

    [Header("Shooting Settings")]
    public float shootCooldown = 0.5f;

    private Rigidbody rb;
    private PlayerShoot playerShoot;
    private float lastShotTime;
    private float verticalLookAngle = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerShoot = GetComponent<PlayerShoot>();
        
        Cursor.lockState = CursorLockMode.Locked;

        // Prevent unwanted tipping
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Auto-assign eyes if not set
        if (eyes == null)
        {
            var eyesTransform = transform.Find("Eyes");
            if (eyesTransform != null)
                eyes = eyesTransform;
            else
                Debug.LogWarning("Eyes Transform not assigned and child 'Eyes' not found.");
        }
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        HandleJump();
        HandleShooting();
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Rotate the player horizontally
        transform.Rotate(0f, mouseX * Time.deltaTime, 0f);

        // Rotate the eyes (camera) vertically
        verticalLookAngle -= mouseY * Time.deltaTime;
        verticalLookAngle = Mathf.Clamp(verticalLookAngle, -20f, 60f);

        if (eyes != null)
            eyes.localRotation = Quaternion.Euler(verticalLookAngle, 0f, 0f);
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical"); // W/S or Up/Down
        float strafeInput = Input.GetAxis("Horizontal"); // A/D or Left/Right

        Vector3 movement = transform.forward * moveInput + transform.right * strafeInput;
        Vector3 targetPosition = rb.position + movement * (speed * Time.deltaTime);
        rb.MovePosition(targetPosition);
    }

    private void HandleJump()
    {
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleShooting()
    {
        // Fire1 is left mouse button by default
        if (Input.GetButton("Fire1") && Time.time - lastShotTime > shootCooldown)
        {
            float shootAngle = eyes != null ? verticalLookAngle : 0f;
            playerShoot.Shoot(this.gameObject, shootAngle);
            lastShotTime = Time.time;
        }
    }
}
