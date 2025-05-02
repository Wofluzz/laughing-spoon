using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Inventory.Model;
using System;

namespace Inventory.Model
{
    [CreateAssetMenu(menuName = "2D Game Objects/Items/2D Items")]
    public class ItemSO_2D : ScriptableObject
    {
        [field: Header("Appearance")]
        [field: SerializeField] public Sprite ItemImage { get; set; }
        [field: SerializeField] public Powerup powerup { get; set; }
        [field: Header("Données")]
        [field: SerializeField] public List<ItemParameter> DefaultParametersList { get; set; }

        public int ID => GetInstanceID(); // Unique ID for each ItemSO
    }

    [Serializable]
    public struct Powerup
    {
        public string PowerUpName { get; set; }
        public PowerupType PowerupType { get; set; }

    }

    public enum PowerupType {
        Coin,
        Potato,
        Bonus,
        Malus
    }
}