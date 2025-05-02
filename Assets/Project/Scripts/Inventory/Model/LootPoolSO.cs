using Inventory.Model;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Items/LootPool")]
    public class LootPoolSO : ScriptableObject
    {
        public List<ItemsInLootPool> lootPool;
    }

    [Serializable]
    public struct ItemsInLootPool
    {
        public ItemSO item;
        public int MaxQuantity;
        public int MinQuantity;
    }
}