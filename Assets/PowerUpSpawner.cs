using Inventory2D.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.U2D;

public class PowerUpSpawner : MonoBehaviour
{
    public ItemSO_2D powerUpSO;

    
    private enum ItemType
    {
        Floating,
        Falling
    }

    [SerializeField]
    private ItemType itemType;

    private SpriteRenderer sr;
    private Animator anim;

    // Start is called before the first frame update  
    void Start()
    {
        SetupPowerUp(powerUpSO);
    }

    public void SetupPowerUp(ItemSO_2D itemSO)
    {
        if (powerUpSO == null)
            return;
        sr = GetComponent<SpriteRenderer>();
        anim = gameObject.AddComponent<Animator>();

        sr.sprite = itemSO.ItemImage;
        anim.runtimeAnimatorController = itemSO.ItemAnimator;

        gameObject.AddComponent<PolygonCollider2D>();

        // Fix for CS0119: Replace invalid usage of 'ItemType' with proper logic  
        if (itemType == ItemType.Falling)
        {
            gameObject.AddComponent<Rigidbody2D>().AddForce(new Vector2((Random.Range(0, 2) * 2 - 1) * 3, 5), ForceMode2D.Impulse);
        }
        else
        {
            gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }

        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void OnDestroy()
    {
        GameManager.instance.ShowScore(powerUpSO.PowerUp[0].Score.ToString(), gameObject);
        GameManager.instance.AddScore(powerUpSO.PowerUp[0].Score);
    }
}
