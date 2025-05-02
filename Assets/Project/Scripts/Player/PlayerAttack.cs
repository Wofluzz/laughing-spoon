using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField]
    private Animator anim;
    private GameObject attackArea = default;
    [SerializeField]
    private Camera cam;

    private bool attacking = false;

    private float timeToAttack = 0.25f;
    private float timer = 0f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        attackArea = transform.GetChild(0).gameObject;    
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (FindObjectOfType<ChatManager>().isChatting) return;
       
        if (Input.GetButtonDown("Attack"))
        {
            Transform player = GetComponent<Transform>();
            Vector3 mouseScreenPosition = Input.mousePosition;
            Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(mouseScreenPosition);
            mouseWorldPosition.z = player.position.z; // Assuming the game is 2D

            Vector3 direction = mouseScreenPosition - player.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            anim.SetFloat("rotation", angle);
            Attack();
        }


        if (attacking)
        {
            timer += Time.deltaTime;

            if (timer >= timeToAttack)
            {
                timer = 0;
                attacking = false;
                attackArea.SetActive(attacking);
            }
        }
    }

    private void Attack()
    {
        attacking = true;
        anim.SetTrigger("attack");
        attackArea.SetActive(attacking);
    }
}
