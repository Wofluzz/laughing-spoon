using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Inventory.Model;

namespace Inventory.Model
{
    public abstract class ItemSO : ScriptableObject, INetworkSerializable
    {
        [field: Header("Appearance")]
        [field: SerializeField] public Sprite ItemImage { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField][field: TextArea] public string Description { get; set; }
        [field: Header("Données")]
        [field: SerializeField] public bool IsStackable { get; set; }
        [field: SerializeField] public int MaxStackSize { get; set; } = 1;
        [field: SerializeField] public List<ItemParameter> DefaultParametersList { get; set; }

        public int ID => GetInstanceID(); // Unique ID for each ItemSO

        // Custom network serialization
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // Serialize the unique ID or name, which can be used to find the item on the other end
            int itemId = ID;
            serializer.SerializeValue(ref itemId); // We only serialize the ID

            // Other data, if needed (Name, etc.)
            string itemName = Name;
            serializer.SerializeValue(ref itemName);
        }
    }

    [System.Serializable]
    public struct ItemParameter : System.IEquatable<ItemParameter>
    {
        public ItemParameterSO itemParameter;
        public float value;

        public bool Equals(ItemParameter other)
        {
            return other.itemParameter == itemParameter;
        }
    }

    [System.Serializable]
    public struct Loots
    {
        public ItemSO item;
        public int minQuantity;
        public int maxQuantity;
    }
}