using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory2D.Model
{
    public abstract class PowerUp_SO : ScriptableObject
    {
        public string powerupName;
        public float duration;
        public int Score = 100; 

        public virtual void Execute(GameObject player)
        {

        }

    }
    
}