using UnityEngine;
using System.Collections;
using Inventory2D.Model;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections.Generic;

namespace Inventory2D.Model
{
    [CreateAssetMenu(menuName = "PowerUps/FireBallPotato")]
    public class FireBallPotatoPowerUp : PowerUp_SO
    {
        public float cooldown = 0.2f;
        public GameObject gm_fireball;

        public AudioClip powerUpSound;

        public override void Execute(GameObject player)
        {
            var sr = player.GetComponent<SpriteRenderer>();

            if (sr == null)
                return;

            var block = new MaterialPropertyBlock();
            sr.GetPropertyBlock(block);

            block.SetFloat("_PowerUP", 2); // par exemple
            sr.SetPropertyBlock(block); // Pas nécessaire, mais explicite    

            if (Input.GetButtonDown("Attack") && GameManager.instance.isPaused == false)
            {
                var aS = player.GetComponent<AudioSource>();
                aS.clip = powerUpSound;
                aS.Play();

                List<GameObject> fireballs = new List<GameObject>();
                GameObject fb = Instantiate(gm_fireball);
                fb.transform.position = player.transform.position + new Vector3(player.transform.localScale.x > 0 ? 1 : -1, 0, 0);
                fireballs.Add(fb);

                foreach (GameObject fireball in fireballs)
                {
                    fireball.transform.position = player.transform.position;
                    fireball.transform.rotation = player.transform.rotation;
                    fireball.GetComponent<FireBall>().SetDirection(player.transform.localScale.x);
                }

                // Utiliser la méthode Cooldown pour gérer le délai entre les attaques  
                player.GetComponent<MonoBehaviour>().StartCoroutine(Cooldown(cooldown));
            }
        }

        private IEnumerator Cooldown(float delay)
        {
            yield return new WaitForSeconds(delay);
        }
    }
}