using Inventory2D.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PowerUpSpawner : MonoBehaviour
{
    public ItemSO_2D powerUpSO;

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
        gameObject.AddComponent<Rigidbody2D>().AddForce(new Vector2((Random.Range(0, 2) * 2 - 1) * 3, 5), ForceMode2D.Impulse);

        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
