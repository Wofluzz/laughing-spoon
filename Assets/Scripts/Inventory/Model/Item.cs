using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Ingame/Items")]
    public class Item : ScriptableObject
    {
        [field: SerializeField]
        public bool IsStackable { get; set; }

        [field: SerializeField]
        public ItemType type { get; set; }

        public int ID => GetInstanceID();

        [field: SerializeField]
        public int MaxStackSize { get; set; } = 1;

        [field: SerializeField]
        public string Name { get; set; }

        [field: SerializeField]
        [field: TextArea]
        public string Description { get; set; }

        [field: SerializeField]
        public Sprite ItemImage { get; set; }
        public enum ItemType
        {
            Collectible, Weapon, Food, Specialized
        }
    }

}
