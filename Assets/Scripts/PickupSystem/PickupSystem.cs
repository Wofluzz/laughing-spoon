using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
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
        if (item != null )
        {
            int reminder = inventoryData.AddItem(item.InventoryItem, item.Quantity);
            if (reminder == 0)
            {
                item.DestroyItem();
                NetworkObject NetItem = collision.gameObject.GetComponent<NetworkObject>();
                NetItem.Despawn();
            }
            else
            {
                item.Quantity = reminder;
                NetworkObject.GetComponent<Item>().Quantity = reminder;
            }
        }
    }
}
