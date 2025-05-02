using Inventory.Model;
using Unity.Netcode;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(DropItemsUI))]
public class DropItemsNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject droppedItemPrefab;
    public int quantity;
    public ItemSO Loot;
    public LootPoolSO LootPool;

    public void DropItem(ItemSO itemSO, int quantity)
    {
        if (IsServer)
        {
            // Server handles the spawn
            SpawnDroppedItemClientRpc(itemSO.ID, quantity);
        }
        else
        {
            // Non-server clients request it by sending the item ID
            RequestDropItemServerRpc(itemSO.ID, quantity);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyObjectServerRpc()
    {
        NetworkObject.Despawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDropItemServerRpc(int itemSOId, int quantity)
    {
        ItemSO itemSO = ItemDatabase.GetItemById(itemSOId); // Retrieve the actual ItemSO object by ID

        if (itemSO != null)
        {
            // Notify clients to spawn the dropped item using the item ID
            SpawnDroppedItemClientRpc(itemSO.ID, quantity);
        }
        else
        {
            Debug.LogError("ItemSO not found on the server.");
        }
    }

    [ClientRpc]
    private void SpawnDroppedItemClientRpc(int itemSOId, int quantity)
    {
        // Fetch the actual ItemSO by ID on the client side
        ItemSO itemSO = ItemDatabase.GetItemById(itemSOId);

        if (itemSO != null)
        {
            // Set the drop position below the current object's position
            Vector3 dropPosition = new Vector3(transform.position.x, transform.position.y - 1, 0);
            GameObject droppedItem = Instantiate(droppedItemPrefab, dropPosition, Quaternion.identity);

            // Set the item data on the dropped item
            var itemComponent = droppedItem.GetComponent<Item>();
            if (itemComponent != null)
            {
                itemComponent.SetItem(droppedItem, itemSO, quantity);
            }

            // Update the text for the quantity on the dropped item
            var textComponent = droppedItem.GetComponentInChildren<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = quantity.ToString();
            }

            // Spawn the dropped item as a networked object
            droppedItem.GetComponent<NetworkObject>().Spawn();
        }
        else
        {
            Debug.LogError("Failed to find ItemSO during RPC call.");
        }
    }
}
