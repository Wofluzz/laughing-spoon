using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Inventory2D.Model;
using System;

namespace Inventory2D.Model
{
    [CreateAssetMenu(menuName = "2D Game Objects/Items/2D Items")]
    public class ItemSO_2D : ScriptableObject
    {
        [field: Header("Appearance")]
        [field: SerializeField] public Sprite ItemImage { get; set; }
        [field: SerializeField] public RuntimeAnimatorController ItemAnimator { get; set; }
        [field: Header("Données")]
        [field: SerializeField] public List<PowerUp_SO> PowerUp { get; set; } //Juste un enum 

        public int ID => GetInstanceID(); // Unique ID for each ItemSO
    }
}