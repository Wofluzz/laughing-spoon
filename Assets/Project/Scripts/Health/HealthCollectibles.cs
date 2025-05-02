using UnityEngine;

public class HealthCollectibles : MonoBehaviour
{
    [SerializeField] private float healthValue;
    [SerializeField] private bool isBonusHearth;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!isBonusHearth)
                collision.GetComponent<Health>().AddHealth(healthValue);
            else 
                collision.GetComponent<Health>().BonusHealth(healthValue);

            gameObject.SetActive(false);
        }
    }
}
