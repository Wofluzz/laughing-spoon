using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaWheeel : MonoBehaviour
{
    public float stamina { private set; get; }
    [SerializeField] private float maxStamina = 100f;

    [SerializeField] private Image greenWheel;
    [SerializeField] private Image redWheel;
    [SerializeField] private GameObject UI;
    private Animation anim;

    public bool staminaExhausted { private set; get; }

    void Start()
    {
        stamina = maxStamina;   
    }

    void Update()
    {
        if (Input.GetButton("Sprint") && !staminaExhausted)
        {
            UI.SetActive(true);
            if (stamina > 0)
            {
                stamina -= 30 * Time.deltaTime;
            } else
            {
                greenWheel.enabled = false;
                staminaExhausted = true;
            }

            redWheel.fillAmount = (stamina / maxStamina + 0.07f);
        } else
        {
            if (stamina < maxStamina)
            {
                stamina += 30 * Time.deltaTime;
            }
            else
            {
                StartCoroutine(disapear());

                greenWheel.enabled = true;
                staminaExhausted = false;
            }

            redWheel.fillAmount = (stamina / maxStamina);
        }
        greenWheel.fillAmount = (stamina / maxStamina);
    }

    private IEnumerator disapear()
    {
        yield return new WaitForSeconds(5);
        UI.SetActive(false);
    }
}
