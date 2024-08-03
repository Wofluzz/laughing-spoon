using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Items/Edible Items")]
    public class EdibleItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        [SerializeField]
        private List<ModifierData> modifierData = new List<ModifierData>();
        public string ActionName => "Consume";

        [field: SerializeField]
        public AudioClip actionSFX {get; private set;}

    public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
        {
            foreach (ModifierData data in modifierData)
            {
                data.statsModifier.AffecrtCharacter(character, data.value);
            }
            return true;
        }
    }

    public interface IDestroyableItem
    {

    }

    public interface IItemAction
    {
        public string ActionName { get; }
        public AudioClip actionSFX { get; }
        bool PerformAction(GameObject character, List<ItemParameter> itemState);
    }

    [Serializable]

    public class ModifierData
    {
        public CharacterStatsModifierSO statsModifier;
        public float value;
    }
}