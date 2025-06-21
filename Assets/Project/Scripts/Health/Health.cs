using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{

    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    public HealthBar healthBar;
    private bool dead;

    [Header("IFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;


    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {
        if (!IsOwner) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            if (_damage > 0)
            {
                anim.SetTrigger("hurt");
                StartCoroutine(Invulnerability());
            }
        }
        else
        {
            if (!dead)
            {
                //Desctiver les classe ratachées au player
                /*foreach (Behaviour component in components)
                    component.enabled = false;*/

                anim.SetTrigger("die");
                GetComponent<PlayerController>().enabled = false;
                GetComponent<PlayerMovements>().enabled = false;
                dead = true;
            }
        }
    }

    public void AddHealth(float _value)
    {
        if (!IsOwner) return;
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    public void BonusHealth(float _value)
    {
        currentHealth = Mathf.Clamp(startingHealth + _value, 0, startingHealth + _value);
    }
    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
    }
    public void Respawn()
    {
        if (!IsOwner) return;
        dead = false;
        AddHealth(startingHealth);
        anim.ResetTrigger("die");
        anim.Play("Idle");
        StartCoroutine(Invulnerability());

        //Activer les classe ratachées au player
        /*foreach (Behaviour component in components)
            component.enabled = true;*/
        GetComponent<PlayerMovements>().enabled = true;

    }
}
