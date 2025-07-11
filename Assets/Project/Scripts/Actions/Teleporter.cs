using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform ToPortal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player in Door");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Player"));
            if (playerCollider != null)
            {
                Debug.Log("Teleporting...");
                playerCollider.transform.position = ToPortal.transform.position;
            }
        }
    }
}
