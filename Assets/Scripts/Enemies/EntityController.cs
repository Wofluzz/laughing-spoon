using Inventory;
using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using Random = System.Random;

[RequireComponent(typeof(EnemyMovements))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyHealthBar))]
[RequireComponent(typeof(Enemy_Sideways))]
[RequireComponent(typeof(Rigidbody2D))]
public class EntityController : NetworkBehaviour
{
    [SerializeField]
    private EntitySO entity;

    [SerializeField]
    private GameObject itemComponents;

    [SerializeField]
    private InventoryController inventoryController;

    private SpriteRenderer spriteRenderer;
    private PlayerAwarenessController playerAwarenessController;
    private Animator anim;
    private EnemyHealth Health;
    private EnemyHealthBar healthBar;
    private Enemy_Sideways sideways;
    private EnemyMovements movements;
    [SerializeField]
    private AudioSource audioSource;

    public override void OnNetworkSpawn()
    {
        InitializeComponents();
        SetupEntity();
    }
    

    private void InitializeComponents()
    {
        audioSource = FindObjectOfType<InventoryController>().GetComponent<AudioSource>();
        inventoryController = FindObjectOfType<InventoryController>().GetComponent<InventoryController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        if (entity.EntityType != EntityType.Humans)
        {
            Health = GetComponent<EnemyHealth>();
            healthBar = GetComponent<EnemyHealthBar>();
            sideways = GetComponent<Enemy_Sideways>();
            playerAwarenessController = GetComponent<PlayerAwarenessController>();
        }
        movements = GetComponent<EnemyMovements>();
    }

    private void SetupEntity()
    {
        if (entity == null)
            return;

        spriteRenderer.sprite = entity.EntitySprite;
        anim.runtimeAnimatorController = entity.EntityAnimationTree;
        SpriteRenderer USprite = gameObject.AddComponent<SpriteRenderer>();
        USprite.sprite = entity.EntitySprite;
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
        Destroy(GetComponent<SpriteRenderer>());

        switch (entity.EntityType)
        {
            case EntityType.Monster:
                SetupMonster(entity);
                break;
            case EntityType.Animals:
                SetupMonster(entity);
                break;
            case EntityType.Boss:
                break;
            case EntityType.Humans:
                break;
        }
        SetupMovements(entity);
        if (entity.DamageToPlayer == 0)
        {
            GetComponent<Enemy_Sideways>().enabled = false;
        }
        StartCoroutine(SetupTriggerCollider());

    }

    private void SetupMovements(EntitySO entity)
    {
        if (entity.IsMove)
            movements.SetupMovement(entity.MoveSpeed, entity.rotateSpeed);
        else
            movements.enabled = false;
    }

    private void SetupMonster(EntitySO entity)
    {
        Health.startingHealth = entity.MaxHealth;
        sideways.SetDamageTo(entity.DamageToPlayer);
        if (entity.CanFollowPlayer && entity.IsMove)
            playerAwarenessController.SetAwarenessDistanceTo(entity.DetectionRadius);
        else
            playerAwarenessController.enabled = false;
    }

    public void DropItemsAndDie()
    {
        if (entity.Loots == null) return;
        foreach (Loots item in entity.Loots)
        {
            Vector3 entityPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1);
            GameObject newItem = Instantiate(itemComponents);
            newItem.transform.position = entityPos;
            Random rnd = new Random();
            inventoryController.ItemDropping(newItem, item.item, (int) rnd.Next(item.minQuantity, item.maxQuantity + 1));
        }
        AudioClip sound = System.Array.Find(entity.Sounds, s => s.Name == "Die").Audio;
        Debug.Log(sound);
        audioSource.PlayOneShot(sound, 1);
        Destroy(gameObject);
    }

    IEnumerator SetupTriggerCollider()
    { 
        yield return new WaitForSeconds(10);
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
    }
}