using Inventory.Model;
using UnityEngine;

[RequireComponent(typeof(DropItemsNetwork))]
public class DropItemsUI : MonoBehaviour
{
    [SerializeField] private DropItemsNetwork dropItemsNetwork; // Reference to the NetworkBehaviour

    public void DropItemForWorldObjects()
    {
        if (dropItemsNetwork != null)
        {
            // Get the Loot and LootPool from DropItemsNetwork
            ItemSO itemSO = dropItemsNetwork.Loot; // Single item loot
            LootPoolSO lootPoolSO = dropItemsNetwork.LootPool; // Pool of items
            int quantity = dropItemsNetwork.quantity;

            // Handle single item loot
            if (itemSO != null)
            {
                dropItemsNetwork.DropItem(itemSO, quantity); // Call the NetworkBehaviour's method
            }

            // Handle loot pool (if LootPoolSO is not null)
            if (lootPoolSO != null && lootPoolSO.lootPool.Count > 0)
            {
                // Loop through each entry in the loot pool
                foreach (ItemsInLootPool itemInPool in lootPoolSO.lootPool)
                {
                    // For each item in the pool, drop it with its corresponding quantity range
                    int randomQuantity = Random.Range(itemInPool.MinQuantity, itemInPool.MaxQuantity + 1); // Random quantity within the range
                    dropItemsNetwork.DropItem(itemInPool.item, randomQuantity); // Drop the item
                }
            }
            dropItemsNetwork.DestroyObjectServerRpc();
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("DropItemsNetwork component is missing!");
        }
    }
}
