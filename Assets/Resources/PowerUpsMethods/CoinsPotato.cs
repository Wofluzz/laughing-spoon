using UnityEngine;
using System.Collections;
using Inventory2D.Model;
using UnityEngine.Rendering;

namespace Inventory2D.Model
{
    [CreateAssetMenu(menuName = "PowerUps/CoinsPotato")]
    public class CoinsPotato : PowerUp_SO
    {
        [Range(0, 10)]
        public int Coins = 0;

        public AudioClip powerUpSound;

        public override void Execute(GameObject player)
        {
            player.GetComponent<AudioSource>().PlayOneShot(powerUpSound);
            GameManager.instance.AddCoins(Coins);
            Debug.Log($"Added {Coins} coins.");

            // Remove dependency on CurrentPowerUp to allow multiple pickups  
            GameManager.instance.StartCoroutine(RemovePowerUpAfterDuration());
        }

        private IEnumerator RemovePowerUpAfterDuration()
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log("Power-up duration ended.");
        }
    }
}