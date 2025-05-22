using UnityEngine;

public class ExplosionForceField : MonoBehaviour
{
    public float radius = 5f;
    public float force = 700f;
    public float upwardModifier = 0.5f;
    public LayerMask affectedLayers;

    public void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, affectedLayers);

        foreach (Collider2D nearby in colliders)
        {
            Rigidbody2D rb = nearby.attachedRigidbody;
            if (rb != null)
            {
                Vector2 direction = (rb.position - (Vector2)transform.position).normalized;
                float distance = Vector2.Distance(rb.position, transform.position);
                float forceMagnitude = force * (1 - (distance / radius));
                Vector2 explosionForce = direction * forceMagnitude + Vector2.up * upwardModifier;

                rb.AddForce(explosionForce, ForceMode2D.Impulse);
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}