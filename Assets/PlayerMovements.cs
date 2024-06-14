using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jump;
    [SerializeField] private float multiJump;
    public float jumps;

    [SerializeField] private bool IsJumping;
    [SerializeField] private bool doubleJumpAllowed;
    private Rigidbody2D body;
    private float JumpVel;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);
        JumpVel = body.velocity.y;
        if (JumpVel == 0)
        {
            IsJumping = false;
            doubleJumpAllowed = false;
            multiJump = jumps;
        }

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow))
        {
            if (IsJumping == false)
            {
                body.velocity = new Vector2(body.velocity.x, jump);
                IsJumping = !IsJumping;
            }
            else
            {
                if (doubleJumpAllowed)
                {
                    IsJumping = false;
                    if (multiJump !< 0)
                    {
                        body.velocity = new Vector2(body.velocity.x, jump);
                        multiJump -= 1;
                    }
                }
            }
        }

        if (IsJumping)
        {
            doubleJumpAllowed = true;
        }
    }


}
