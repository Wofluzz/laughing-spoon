using UnityEngine;
using System.Collections;
using Inventory2D.Model; // Ensure this namespace is correctly defined if PowerUp_SO is in it
using UnityEngine.Rendering;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Inventory2D.Model
{
    [CreateAssetMenu(menuName = "PowerUps/SlipperyPotato")]
    public class SliperyPotatoPowerUp : PowerUp_SO
    {
        public float dashForce = 50f;
        public float dashDuration = 0.2f;

        public AudioClip powerUpSound;

        public override void Execute(GameObject player)
        {
            // Debugging: Confirm Execute is called
            Debug.Log("SlipperyPotatoPowerUp.Execute() called.");

            var sr = player.GetComponent<SpriteRenderer>();
            var tr = player.GetComponent<TrailRenderer>();
            var rb = player.GetComponent<Rigidbody2D>();
            var pm = player.GetComponent<PlayerMovements>();

            if (sr == null || tr == null || rb == null || pm == null)
            {
                Debug.LogError("Missing component(s) on player for SlipperyPotatoPowerUp to execute. Ensure SpriteRenderer, TrailRenderer, Rigidbody2D, and PlayerMovements are present.");
                return;
            }

            var block = new MaterialPropertyBlock();
            sr.GetPropertyBlock(block);

            block.SetFloat("_PowerUP", 1); // Example: for shader effect
            sr.SetPropertyBlock(block);

            // Determine dash direction:
            // Prefer current horizontal input, otherwise use the player's current horizontal linearVelocity direction if dashing, or default to 1f.
            // It's important to get currentInput for immediate response, not just the cached horizontalInput from PlayerMovements.
            float currentInput = Input.GetAxisRaw("Horizontal");
            float lastDirection = currentInput != 0 ? Mathf.Sign(currentInput) : pm.isDashing ? Mathf.Sign(rb.linearVelocity.x) : player.transform.localScale.x; // Use player's current facing direction if no input and not dashing

            // Debugging: Check input and dashing state
            Debug.Log($"Input.GetButtonDown('Attack'): {Input.GetButtonDown("Attack")}, pm.isDashing: {pm.isDashing}");

            if (Input.GetButtonDown("Attack") && !pm.isDashing) // Use !pm.isDashing for clarity
            {
                var aS = player.GetComponent<AudioSource>();
                if (aS != null && powerUpSound != null)
                {
                    aS.clip = powerUpSound;
                    aS.Play();
                }
                else
                {
                    Debug.LogWarning("AudioSource or PowerUpSound missing on player.");
                }

                pm.isDashing = true;
                tr.emitting = true;

                // Apply the dash force
                // Important: Temporarily set gravity to 0 or a very low value during the dash
                // to prevent it from immediately pulling the player down and reducing horizontal speed.
                //pm.SetGravityScale(0f); // Set gravity scale via PlayerMovements to manage it centrally
                rb.linearVelocity = new Vector2(lastDirection * dashForce, 0f); // Set y-linearVelocity to 0 for a purely horizontal dash, or keep rb.linearVelocity.y if you want to preserve vertical momentum. I've set it to 0 for a clearer horizontal dash.

                Debug.Log($"Dash initiated. linearVelocity: {rb.linearVelocity}, isDashing: {pm.isDashing}");

                // Start the coroutine on the PlayerMovements script as ScriptableObjects cannot run coroutines directly.
                pm.StartCoroutine(StopDash(pm, dashDuration, tr, rb));
            }
        }

        // Added Rigidbody2D rb parameter to StopDash to reset gravity
        private IEnumerator StopDash(PlayerMovements pm, float delay, TrailRenderer tr, Rigidbody2D rb)
        {
            yield return new WaitForSeconds(delay);
            tr.emitting = false;
            pm.isDashing = false;
            //pm.SetGravityScale(5f); // Reset gravity to its normal value after dash
            Debug.Log("Dash stopped. isDashing: false, Gravity Reset.");
        }
    }
}