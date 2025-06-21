using System;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // Added default value
    [SerializeField] private float jumpPower = 15f; // Added default value
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Health health; // Ensure this component exists or is handled
    [SerializeField] private AudioClip DeathAudio;
    [SerializeField] private bool canWallClimb = true;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private float wallJumpCooldown;
    private float horizontalInput;
    public bool isDashing; // Public to be accessible by PowerUp script

    private float normalGravityScale = 5f; // Store the normal gravity scale

    public static event Action OnPlayerDied;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        // Initialize normalGravityScale from Rigidbody2D
        normalGravityScale = body.gravityScale;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // ONLY apply regular movement if NOT dashing
        if (!isDashing)
        {
            if (horizontalInput > 0.01f)
                transform.localScale = Vector3.one;
            else if (horizontalInput < -0.01f)
                transform.localScale = new Vector3(-1, 1, 1);

            // Jump input check (regular jump)
            if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetButtonDown("Jump")) && isGrounded())
            {
                Jump();
            }

            // Wall climbing/jumping logic
            // This entire block should only execute if not dashing
            if (canWallClimb && wallJumpCooldown > 0.2f)
            {
                body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

                if (onWall() && !isGrounded() && Mathf.Abs(horizontalInput) > 0.01f && Mathf.Sign(horizontalInput) == Mathf.Sign(transform.localScale.x))
                {
                    body.gravityScale = 0; // Stick to wall
                    body.linearVelocity = Vector2.zero; // Stop vertical movement while sticking
                }
                else
                {
                    body.gravityScale = normalGravityScale; // Reset gravity if not wall-clinging
                }

                // Wall jump input check
                if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)))
                {
                    Jump(); // Will handle wall jump if on wall
                }
            }
            else // Regular horizontal movement when not wall climbing or wall jump cooldown active
            {
                body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);
                body.gravityScale = normalGravityScale; // Ensure gravity is normal
            }
        }
        else
        {
            // If dashing, do not apply regular horizontal movement.
            // Gravity is handled by the dash power-up setting it to 0 temporarily.
            // We might want to keep some gravity if the dash isn't purely horizontal,
            // but for now, the power-up manages it.
            // Make sure the player still faces the direction of the dash if there's no horizontal input during the dash.
            if (body.linearVelocity.x > 0.01f)
                transform.localScale = Vector3.one;
            else if (body.linearVelocity.x < -0.01f)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        // Wall jump cooldown update (applies regardless of dashing, but impacts movement *after* dash)
        if (canWallClimb)
        {
            wallJumpCooldown += Time.deltaTime;
        }
        else
        {
            wallJumpCooldown = 0.21f; // Ensure it's above the threshold if climbing is off
        }

        anim.SetBool("run", horizontalInput != 0 && !isDashing); // Run animation depends on input AND not dashing
        anim.SetBool("grounded", isGrounded());
    }

    // New method to set gravity scale, useful for external scripts (like power-ups)
    public void SetGravityScale(float scale)
    {
        if (body != null)
        {
            body.gravityScale = scale;
        }
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            anim.SetTrigger("jump");
            Debug.Log("Jump: Grounded Jump");
        }
        else if (canWallClimb && onWall() && !isGrounded())
        {
            if (horizontalInput == 0) // Wall jump away from wall without input
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                Debug.Log("Jump: Wall Jump (no input)");
            }
            else // Wall jump with input (climb-like)
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
                anim.SetTrigger("climb"); // Assuming "climb" is the correct animation for this
                Debug.Log("Jump: Wall Jump (with input/climb)");
            }

            wallJumpCooldown = 0;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);

        Color rayColor = (raycastHit.collider != null) ? Color.green : Color.red;
        Debug.DrawRay(boxCollider.bounds.center + new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + 0.1f), rayColor);
        Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + 0.1f), rayColor);
        Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y + 0.1f), Vector2.right * (boxCollider.bounds.size.x), rayColor);

        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);

        Color rayColor = (raycastHit.collider != null) ? Color.green : Color.red;
        Vector2 origin = boxCollider.bounds.center;
        Vector2 size = boxCollider.bounds.size;
        float distance = 0.1f;
        Vector2 direction = new Vector2(transform.localScale.x, 0);

        Vector2 castCenter = origin + direction * (size.x / 2 + distance);
        Vector2 topLeft = new Vector2(castCenter.x - size.x / 2 * Mathf.Sign(direction.x), castCenter.y + size.y / 2);
        Vector2 bottomLeft = new Vector2(castCenter.x - size.x / 2 * Mathf.Sign(direction.x), castCenter.y - size.y / 2);
        Vector2 topRight = new Vector2(castCenter.x + size.x / 2 * Mathf.Sign(direction.x), castCenter.y + size.y / 2);
        Vector2 bottomRight = new Vector2(castCenter.x + size.x / 2 * Mathf.Sign(direction.x), castCenter.y - size.y / 2);

        Debug.DrawLine(topLeft, topRight, rayColor);
        Debug.DrawLine(topRight, bottomRight, rayColor);
        Debug.DrawLine(bottomRight, bottomLeft, rayColor);
        Debug.DrawLine(bottomLeft, topLeft, rayColor);
        Debug.DrawLine(origin + new Vector2(boxCollider.bounds.extents.x * Mathf.Sign(transform.localScale.x), boxCollider.bounds.extents.y), topLeft, rayColor);
        Debug.DrawLine(origin + new Vector2(boxCollider.bounds.extents.x * Mathf.Sign(transform.localScale.x), -boxCollider.bounds.extents.y), bottomLeft, rayColor);

        return raycastHit.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player entered trigger: " + collision.name);

        if (collision.CompareTag("Bounds"))
        {
            Debug.Log("Player out of bounds, taking damage.");

            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null && DeathAudio != null)
            {
                audioSource.PlayOneShot(DeathAudio);
            }
            else
            {
                Debug.LogWarning("AudioSource or DeathAudio clip missing on player for 'Bounds' collision.");
            }

            OnPlayerDied?.Invoke();

            if (health != null)
            {
                health.TakeDamage(100000); // Massive damage to ensure death
            }
            else
            {
                Debug.LogWarning("Health component not found on player for 'Bounds' collision.");
            }

            // Consider if you really want to disable these scripts permanently on death.
            // Often, you'd want to reload the scene or show a game over screen.
            GetComponent<PlayerMovements>().enabled = false;
            PowerUpController powerUpController = GetComponent<PowerUpController>();
            if (powerUpController != null)
            {
                powerUpController.enabled = false;
            }
        }
    }
}
