using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabaseInitializer : MonoBehaviour
{
    private void Start()
    {
        // Charger tous les objets ItemSO dans un dossier nomm� "Items" sous "Resources"
        ItemSO[] allItems = Resources.LoadAll<ItemSO>("Items");

        Debug.Log(allItems.Length);

        // Enregistrer chaque item dans la base de donn�es
        foreach (ItemSO item in allItems)
        {
            ItemDatabase.RegisterItem(item);
            ItemDatabase.GetItemCount();
        }

        Debug.Log("Tous les items ont �t� enregistr�s dans la base de donn�es.");
    }
}
