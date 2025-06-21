using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Knockback : NetworkBehaviour
{
    private Rigidbody2D rb2d;
    private GameObject player;

    [SerializeField]
    private float knockbackForce = 15f;

    public override void OnNetworkSpawn()
    {
        rb2d = GetComponent<Rigidbody2D>();

        // Trouver le joueur du propri�taire ou du serveur, selon la situation
        if (IsOwner)
        {
            player = gameObject;  // Le joueur local est celui qui poss�de cet objet
        }
        else
        {
            player = FindClosestPlayer();  // Si ce n'est pas le propri�taire, on cherche le joueur le plus proche
        }
    }

    public void Knock()
    {
        if (IsServer)
        {
            Vector2 direction = (transform.position - player.transform.position).normalized;
            ApplyKnockbackServerRpc(direction);
        }
    }

    [ServerRpc]
    private void ApplyKnockbackServerRpc(Vector2 direction)
    {
        // Applique la force de knockback uniquement sur le serveur
        rb2d.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        StartCoroutine(ResetKnockBack());
    }

    private IEnumerator ResetKnockBack()
    {
        yield return new WaitForSeconds(0.15f);
        rb2d.linearVelocity = Vector3.zero;
    }

    // Helper pour trouver le joueur le plus proche (si c'est n�cessaire pour des ennemis)
    private GameObject FindClosestPlayer()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (var playerController in players)
        {
            float distance = Vector2.Distance(playerController.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestPlayer = playerController.gameObject;
                closestDistance = distance;
            }
        }

        return closestPlayer;
    }
}
