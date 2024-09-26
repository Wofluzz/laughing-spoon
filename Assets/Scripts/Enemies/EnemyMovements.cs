using UnityEngine;

[RequireComponent(typeof(PlayerAwarenessController))]
public class EnemyMovements : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _rotationSpeed;

    private Rigidbody2D _rigidbody;
    private Animator anim;
    private PlayerAwarenessController _playerAwarnessController;
    private Vector2 _targetDirection;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwarnessController = GetComponent<PlayerAwarenessController>();
        anim = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        SetVelocity();
    }

    private void UpdateTargetDirection()
    {
        if (_playerAwarnessController.AwareOfPlayer)
            _targetDirection = _playerAwarnessController.DirectionToPlayer;
        else
            _targetDirection = Vector2.zero;
    }

    private void RotateTowardsTarget()
    {
        if (_targetDirection == Vector2.zero) return;

        float angle = Mathf.Atan2(_targetDirection.y, _targetDirection.x) * Mathf.Rad2Deg;
        anim.SetFloat("Rotation", angle);
    }

    private void SetVelocity()
    {
        if (_targetDirection == Vector2.zero)
            _rigidbody.velocity = Vector2.zero;
        else
            _rigidbody.velocity = _targetDirection.normalized * _speed;
    }

    public void SetupMovement(float speed, float rotationSpeed)
    {
        _speed = speed;
        _rotationSpeed = rotationSpeed;
    }

}
