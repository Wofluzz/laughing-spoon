using UnityEngine;
using System.Linq;

public class PlayerAwarenessController : MonoBehaviour
{
    public bool AwareOfPlayer { get; private set; }
    public Vector2 DirectionToPlayer { get; private set; }

    [SerializeField]
    private float _playerAwarenessDistance;

    private Transform _closestPlayer;

    private void Update()
    {
        _closestPlayer = GetClosestPlayer();
        
        if (_closestPlayer != null)
        {
            Vector2 enemyToPlayerVector = _closestPlayer.position - transform.position;
            DirectionToPlayer = enemyToPlayerVector.normalized;

            if (enemyToPlayerVector.magnitude <= _playerAwarenessDistance)
                AwareOfPlayer = true;
            else
                AwareOfPlayer = false;
        }
        else
        {
            AwareOfPlayer = false;
        }
    }

    private Transform GetClosestPlayer()
    {
        // Trouver tous les objets PlayerController dans la scène
        var players = FindObjectsOfType<PlayerController>();

        if (players.Length == 0)
            return null;

        // Trouver le joueur le plus proche
        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (var player in players)
        {
            float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
            if (distanceToPlayer < closestDistance)
            {
                closestPlayer = player.transform;
                closestDistance = distanceToPlayer;
            }
        }

        return closestPlayer;
    }

    public void SetAwarenessDistanceTo(float distance)
    {
        _playerAwarenessDistance = distance;
    }
}
