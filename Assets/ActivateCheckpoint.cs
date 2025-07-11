using UnityEngine;

public class ActivateCheckpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vérifie si l'objet entrant est le joueur  
        GameObject player = FindAnyObjectByType<PlayerMovements>().gameObject;
        if (collision.gameObject == player)
        {
            // Logique à exécuter lorsque le joueur entre en collision avec CheckPointObj  
            GameManager.instance.Checkpoint = true;
            Debug.Log("Checkpoint atteint par le joueur !");
        }
    }
}
