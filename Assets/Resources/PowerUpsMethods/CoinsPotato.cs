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
            if (GameManager.instance.CurrentPowerUp != this)
            {
                GameManager.instance.CurrentPowerUp = this;
                GameManager.instance.AddCoins(Coins);
                GameManager.instance.StartCoroutine(RemovePowerUpAfterDuration());
            }
        }

        private IEnumerator RemovePowerUpAfterDuration()
        {
            yield return new WaitForSeconds(duration);
            if (GameManager.instance.CurrentPowerUp == this)
            {
                GameManager.instance.CurrentPowerUp = null;
            }
        }
    }
}