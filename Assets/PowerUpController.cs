using Inventory2D.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public PowerUp_SO powerUp;
    public PowerUp_SO lastPU;

    [SerializeField]
    private AudioClip powerUpSound;
    [SerializeField]
    private AudioSource soundSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision?.GetComponent<PowerUpSpawner>()?.powerUpSO?.PowerUp?.Count != 0)
        {
            Debug.Log("OK");
            PowerUp_SO powerUp = collision?.gameObject?.GetComponent<PowerUpSpawner>()?.powerUpSO?.PowerUp[0];
            if (powerUp != null)
            {
                Debug.Log("Power-up detected: " + powerUp.powerupName);

                if (powerUp is CoinsPotato)
                {
                    Debug.Log("Coin collected: " + powerUp.powerupName);
                    powerUp.Execute(gameObject);
                }
                else
                {
                    GameManager.instance.CurrentPowerUp = powerUp;
                    GetComponent<Health>().AddHealth(1);
                    this.powerUp = GameManager.instance.CurrentPowerUp;
                    Debug.Log("Power-up acquired: " + powerUp.powerupName);
                    soundSource.clip = powerUpSound;
                    soundSource.Play();
                    lastPU = powerUp;
                }

                Destroy(collision.gameObject);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (powerUp != null)
        {
            powerUp.Execute(gameObject);
        }

    }
}
