using Inventory.Model;
using Unity.Netcode;
using UnityEngine;

public class PickupSystem : NetworkBehaviour
{
    [SerializeField]
    private InventorySO inventoryData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;

        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            HandlePickup(item);
        }
    }

    private void HandlePickup(Item item)
    {
        int reminder = inventoryData.AddItem(item.InventoryItem, item.Quantity);
        if (reminder == 0)
        {
            // Despawn the item on the server and destroy for all clients
            DespawnItemServerRpc(item.GetComponent<NetworkObject>().NetworkObjectId);
        }
        else
        {
            item.Quantity = reminder;
            item.GetComponent<Item>().Quantity = reminder; // Update quantity if not all picked up
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnItemServerRpc(ulong networkObjectId)
    {
        NetworkObject netObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        if (netObject != null && netObject.IsSpawned)
        {
            netObject.Despawn();
            DestroyItemClientRpc(networkObjectId); // Notify clients to destroy the item
            Debug.Log("Item despawned on the server.");
        }
    }

    [ClientRpc]
    private void DestroyItemClientRpc(ulong networkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject netObject) && netObject.IsSpawned)
        {
            netObject.Despawn(); // Despawn on client side
            Debug.Log("Item destroyed on the client.");
        }
    }
}
