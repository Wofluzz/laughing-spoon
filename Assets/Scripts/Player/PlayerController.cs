using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float speedBoost;
    //[SerializeField] private float jumpPower;
    [SerializeField] private float forceDamping;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private StaminaWheeel staminaWheel;
    private Rigidbody2D body;
    private Animator anim;
    private Vector2 ForceToApply;
    private BoxCollider2D boxCollider;
    private bool IsWalking;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        Walk(speed);

        //Gere les paramètres d'animation
        IsWalking = Input.GetAxisRaw("Horizontal") != 0 | Input.GetAxisRaw("Vertical") != 0;
        if (IsWalking && Input.GetKey(KeyCode.LeftControl) && (staminaWheel.stamina > 0 && !staminaWheel.staminaExhausted))
        {
            Walk(speed * speedBoost);
            anim.SetBool("Running", true);
        }
        else
            anim.SetBool("Running", false);
        anim.SetBool("IsWalking", IsWalking);

        anim.SetFloat("moveX", Input.GetAxisRaw("Horizontal"));
        anim.SetFloat("moveY", Input.GetAxisRaw("Vertical"));
    }

    private void Walk(float speed)
    {
        Vector2 PlayerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Vector2 moveForce = PlayerInput * speed;
        moveForce += ForceToApply;
        ForceToApply /= forceDamping;

        if (Mathf.Abs(ForceToApply.x) <= 0.01f && Mathf.Abs(ForceToApply.y) <= 0.01f)
        {
            ForceToApply = Vector2.zero;
        }
        body.velocity = moveForce;
    }
}
