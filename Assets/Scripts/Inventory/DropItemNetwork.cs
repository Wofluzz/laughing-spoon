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
            SpawnDroppedItem(itemSO, quantity); // Server handles the spawn
        }
        else
        {
            RequestDropItemServerRpc(itemSO.name, quantity); // Non-server clients request it
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyObjectServerRpc()
    {
        NetworkObject.Despawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDropItemServerRpc(string itemSOName, int quantity)
    {
        ItemSO itemSO = FindItemSOByName(itemSOName);
        if (itemSO != null)
        {
            SpawnDroppedItem(itemSO, quantity);
        }
        else
        {
            Debug.LogError("ItemSO not found on the server.");
        }
    }

    private void SpawnDroppedItem(ItemSO itemSO, int quantity)
    {
        Vector3 dropPosition = new Vector3(transform.position.x, transform.position.y - 1, 0);
        GameObject droppedItem = Instantiate(droppedItemPrefab, dropPosition, Quaternion.identity);

        var itemComponent = droppedItem.GetComponent<Item>();
        if (itemComponent != null)
        {
            itemComponent.SetItem(droppedItem, itemSO, quantity);
        }

        var textComponent = droppedItem.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = quantity.ToString();
        }

        droppedItem.GetComponent<NetworkObject>().Spawn();
    }

    private ItemSO FindItemSOByName(string itemName)
    {
        ItemSO[] allItems = FindObjectsOfType<ItemSO>(); // Or another method to manage ItemSO
        foreach (var item in allItems)
        {
            if (item.name == itemName)
            {
                return item;
            }
        }
        return null;
    }
}
