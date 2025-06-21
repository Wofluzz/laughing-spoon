using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] public float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    public EnemyHealthBar healthBar;
    private bool dead;

    private EntityController controller;
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] UnityEvent onHit, onDie;

    [SerializeField] private float health;
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    [SerializeField] private SpriteRenderer spriteRend;


    private void Awake()
    {
        controller = GetComponent<EntityController>();
        currentHealth = startingHealth;
        spriteRend = GetComponentInChildren<SpriteRenderer>();
    }
    private void Update()
    {
        health = currentHealth;
    }

    public void Damage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        if (players.Length > 0)
        {
            GameObject player = players[0].gameObject;
        }
        onHit?.Invoke();

        if (currentHealth > 0)
        {
            StartCoroutine(Invulnerability());
        }
        else
        {
            if (!dead)
            {
                onDie?.Invoke();
                dead = true;
                if (controller)
                    controller.DropItemsAndDie();
            }
        }
    }

    private IEnumerator Invulnerability()
    {
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
    }
}
