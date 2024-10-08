using TMPro;
using Unity.Netcode;
using UnityEngine;

public class AttackArea : NetworkBehaviour
{
    private int damage = 3;
    private ParticleSystem particle;

    //[SerializeField]
    //private TMP_Text damageText;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!IsOwner) return;
        if (collider.GetComponent<EnemyHealth>() != null)
        {
            EnemyHealth health = collider.GetComponent<EnemyHealth>();
            health.Damage(damage);
            particle.Play();
            //damageText.text = damage.ToString();
            particle.Stop();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position,1);
    }
}
