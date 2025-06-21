using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.VFX;

namespace Inventory2D.Model
{
    public class FireBall : MonoBehaviour
    {
        public float speed = 10f;
        private float direction;
        private float lifetime = 30f;
        public VisualEffect visualEffect;
        public AudioSource PlayerAudioSource;

        public List<Audios> SFXs;

        void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float playerDirection = player.transform.localScale.x > 0 ? 1f : -1f;
                SetDirection(playerDirection);

                transform.position = new Vector2(player.transform.position.x + (playerDirection * 0.5f), player.transform.position.y);
            }
        }

        void Update()
        {
            transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
            lifetime -= Time.deltaTime;

            if (lifetime <= 0)
            {
                OnFireBallHit();
            }
        }

        public void SetDirection(float dir)
        {
            direction = dir;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("Collision with: " + collision.gameObject.name);

            if (collision.gameObject.CompareTag("Enemy"))
            {
                var enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.Damage(1);
                }
                OnFireBallHit();
            }
            else if (collision.gameObject.CompareTag("Breakables"))
            {
                FindInAudiosAndPlay("Break");
                GameManager.instance.AddScore(50);

                GameManager.instance.ShowScore("50", gameObject);
                Destroy(collision.gameObject);
                OnFireBallHit();
            }
            else if (!collision.gameObject.CompareTag("Player"))
            {
                OnFireBallHit();
            }
            else
            {
                FindInAudiosAndPlay("Bounce");
            }
        }

        private void FindInAudiosAndPlay(string name)
        {
            var audio = System.Array.Find<Audios>(SFXs.ToArray(), sfx => sfx.name == name);
            if (audio.clip != null)
            {
                PlayerAudioSource.clip = audio.clip;
                PlayerAudioSource.Play();
            }
        }

        private void OnFireBallHit()
        {
            transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            visualEffect.Play();
            StartCoroutine(DestroyFireball(1f));
        }

        IEnumerator DestroyFireball(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }

        [System.Serializable]
        public struct Audios
        {
            public string name;
            public AudioClip clip;
        }
    }
}