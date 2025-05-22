using UnityEngine;
using System.Collections;
using Inventory2D.Model;

namespace Inventory2D.Model
{
    [CreateAssetMenu(menuName = "PowerUps/SlipperyPotato")]
    public class SliperyPotatoPowerUp : PowerUp_SO
    {
        public float dashForce = 50f;
        public float dashDuration = 0.2f;

        public override void Execute(GameObject player)
        {
            var sr = player.GetComponent<SpriteRenderer>();
            var tr = player.GetComponent<TrailRenderer>();
            var rb = player.GetComponent<Rigidbody2D>();
            var pm = player.GetComponent<PlayerMovements>();

            if (sr == null || tr == null || rb == null || pm == null)
                return;

            Material mat = sr.material;
            mat.EnableKeyword("_POWERUP_SLIPPERY");

            float currentInput = Input.GetAxisRaw("Horizontal");
            float lastDirection = currentInput != 0 ? Mathf.Sign(currentInput) : 1f;

            if (Input.GetButtonDown("Attack"))
            {
                Debug.Log("Dash!");

                pm.isDashing = true;
                tr.emitting = true;
                rb.velocity = new Vector2(lastDirection * dashForce, rb.velocity.y);

                pm.StartCoroutine(StopDash(pm, dashDuration, tr));
            }
        }

        private IEnumerator StopDash(PlayerMovements pm, float delay, TrailRenderer tr)
        {
            yield return new WaitForSeconds(delay);
            tr.emitting = false;
            pm.isDashing = false;
        }
    }
}