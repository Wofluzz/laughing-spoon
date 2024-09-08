using UnityEngine;

public class PlayerAwarnessController : MonoBehaviour
{
    public bool AwareOfPlayer { get; private set; }

    public Vector2 DirectionToPlayer { get; private set; }

    [SerializeField]
    private float _playerAwarnessDistance;

    private Transform _player;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerController>().transform;
    }

    void Update()
    {
        Vector2 enemyToPlayerVector = _player.position - transform.position;
        DirectionToPlayer = enemyToPlayerVector.normalized;

        if (enemyToPlayerVector.magnitude <= _playerAwarnessDistance)
            AwareOfPlayer = true;
        else
            AwareOfPlayer = false;
    }

    public void SetAwarnessDistanceTo(float distance)
    {
        _playerAwarnessDistance = distance;
    }
}
