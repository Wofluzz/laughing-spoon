using Inventory2D.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public PowerUp_SO powerUp;

    // Start is called before the first frame update
    void Start()
    {
        
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
