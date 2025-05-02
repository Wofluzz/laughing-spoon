using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour 
{ 
    private static Dictionary<int, ItemSO> itemLookupById = new Dictionary<int, ItemSO>();

    public static void RegisterItem(ItemSO item)
    {
        if (item == null)
        {
            Debug.LogWarning("Tentative d'enregistrement d'un item nul dans la base de données.");
            return;
        }

        if (!itemLookupById.ContainsKey(item.ID))
        {
            itemLookupById.Add(item.ID, item);
            Debug.Log($"Item enregistré : {item.name} (ID: {item.ID})");
        }
        else
        {
            Debug.LogWarning($"L'item avec l'ID {item.ID} est déjà enregistré dans la base de données.");
        }
    }

    public static ItemSO GetItemById(int id)
    {
        if (itemLookupById.TryGetValue(id, out var item))
        {
            Debug.Log($"Item trouvé : {item.name} (ID: {id})");
            return item;
        }
        else
        {
            Debug.LogWarning($"Aucun item trouvé avec l'ID : {id}");
            return null;
        }
    }

    public static int GetItemCount()
    {
        return itemLookupById.Count;
    }
}
