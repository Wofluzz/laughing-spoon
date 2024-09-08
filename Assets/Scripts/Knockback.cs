using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb2d;
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        player = FindAnyObjectByType<PlayerController>().gameObject;
    }
    // Start is called before the first frame update
    public void Knock()
    {
        StopAllCoroutines();
        Vector2 direction = (transform.position - player.transform.position).normalized;
        rb2d.AddForce(direction * 15, ForceMode2D.Impulse);
        StartCoroutine(ResetKnockBack());
    }

    private IEnumerator ResetKnockBack()
    {
        yield return new WaitForSeconds(0.15f);
        rb2d.velocity = Vector3.zero;
    }
}
