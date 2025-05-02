using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private EnemyHealth playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    private void Start()
    {
        totalHealthBar.fillAmount = (playerHealth.currentHealth * 10) / playerHealth.startingHealth;
    }

    private void Update()
    {
        currentHealthBar.fillAmount = (playerHealth.currentHealth * 10) / playerHealth.startingHealth;

    }
}

