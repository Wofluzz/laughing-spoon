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
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        JumpVel = body.velocity.y;
        if (JumpVel == 0)
        {
            IsJumping = false;
            doubleJumpAllowed = false;
            multiJump = jumps;
        }

        //Gère la rotaion du personnage selon sa direction
        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Mouvement : Saut
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow))
        {
            if (IsJumping == false)
            {
                body.velocity = new Vector2(body.velocity.x, jump);
                IsJumping = true;
                doubleJumpAllowed = true;
            }
            else
            {
                if (doubleJumpAllowed == true)
                {
                    if (multiJump! < 0)
                    {
                        body.velocity = new Vector2(body.velocity.x, jump);
                        multiJump -= 1;
                    }
                }
            }
        }
    }

    //Gestion des collisions et Triggers

    private void OnTriggerEnter(Collider collision)
    {
        //Gestion des collisions Joueur-Monde
        if (collision.tag == "World")
        {
            IsJumping = false;            
        }
    }


}
