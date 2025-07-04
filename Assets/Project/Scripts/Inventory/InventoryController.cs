using Inventory.Model;
using Inventory.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Inventory
{
    public class InventoryController : NetworkBehaviour
    {
        public static InventoryController Singleton;

        [SerializeField]
        private UIInventoryPage inventoryUI;

        [SerializeField]
        private InventorySO inventoryData;

        public List<InventoryItem> initialItems = new List<InventoryItem>();

        [SerializeField]
        private AudioClip dropClip;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private GameObject droppedItemPrefab;  // The dropped item prefab

        private Transform player;

        private void Awake()
        {
            player = GetComponent<Transform>();
            InventoryController.Singleton = this;
        }

        public override void OnNetworkSpawn()
        {
            inventoryUI = FindAnyObjectByType<UIInventoryPage>().GetComponent<UIInventoryPage>();
            PrepareUI();
            PrepareInventoryData();
        }

        private void PrepareInventoryData()
        {
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateUI;

            foreach (InventoryItem item in initialItems)
            {
                if (item.IsEmpty)
                    continue;

                inventoryData.AddItem(item);
            }
        }

        private void UpdateUI(Dictionary<int, InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();

            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
            }
        }

        private void PrepareUI()
        {
            inventoryUI.InitInventoryUI(inventoryData.Size);
            inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            inventoryUI.OnSwapItems += HandleSwapItems;
            inventoryUI.OnStartDragging += HandleDragging;
            inventoryUI.OnItemActionRequested += HandleItemActionRequest;
        }

        private void HandleItemActionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty) return;

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                inventoryUI.ShowItemAction(itemIndex);
                inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
            }

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryUI.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
            }
        }

        private void DropItem(int itemIndex, int quantity)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            inventoryData.RemoveItem(itemIndex, quantity);
            inventoryUI.ResetSelection();
            audioSource.PlayOneShot(dropClip);

            Vector3 dropPosition = new Vector3(player.position.x, player.position.y - 3, player.position.z);
            GameObject droppedItem = Instantiate(droppedItemPrefab, dropPosition, Quaternion.identity);

            ItemDropping(droppedItem, inventoryItem.item, inventoryItem.quantity);

            if (IsServer)
            {
                // Directly spawn the object on the server
                droppedItem.GetComponent<NetworkObject>().Spawn();
                Debug.Log("Item dropped by server.");
            }
            else
            {
                // Send an RPC to the server to spawn the item
                DropItemServerRpc(dropPosition, inventoryItem.item.ID, quantity);
            }
        }

        public void ItemDropping(GameObject obj, ItemSO itemSO, int quantity)
        {
            Item item = obj.GetComponent<Item>();
            TMP_Text quantityText = obj.GetComponentInChildren<TMP_Text>();
            if (quantityText != null)
            {
                quantityText.text = quantity.ToString();
            }

            if (item != null)
            {
                item.SetItem(obj, itemSO, quantity);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DropItemServerRpc(Vector3 position, int itemId, int quantity)
        {
            ItemSO itemSO = ItemDatabase.GetItemById(itemId); // Use an item database to retrieve the item by ID

            if (itemSO != null)
            {
                GameObject droppedItem = Instantiate(droppedItemPrefab, position, Quaternion.identity);
                ItemDropping(droppedItem, itemSO, quantity);

                droppedItem.GetComponent<NetworkObject>().Spawn();
                Debug.Log("Item dropped on the server.");
            }
            else
            {
                Debug.LogError("ItemSO not found on the server.");
            }
        }

        private void PerformAction(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty) return;

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryData.RemoveItem(itemIndex, 1);
            }

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(gameObject, inventoryItem.itemState);
                audioSource.PlayOneShot(itemAction.actionSFX);

                if (inventoryData.GetItemAt(itemIndex).IsEmpty)
                {
                    inventoryUI.ResetSelection();
                }
            }
        }

        private void HandleDragging(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty) return;

            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        }

        private void HandleSwapItems(int itemIndex1, int itemIndex2)
        {
            inventoryData.SwapItems(itemIndex1, itemIndex2);
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }

            ItemSO item = inventoryItem.item;
            string description = PrepareDescription(inventoryItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.Name, description);
        }

        private string PrepareDescription(InventoryItem inventoryItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(inventoryItem.item.Description);
            sb.AppendLine();

            for (int i = 0; i < inventoryItem.itemState.Count; i++)
            {
                sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName} " +
                          $": {inventoryItem.itemState[i].value} / {inventoryItem.item.DefaultParametersList[i].value}");
            }

            return sb.ToString();
        }

        private void Update()
        {
            if (!IsOwner) return;

            // Find the first ChatManager instance instead of treating it as an array  
            ChatManager chatManager = FindFirstObjectByType<ChatManager>();
            if (chatManager != null && chatManager.isChatting) return;

            if (Input.GetKeyDown(KeyCode.I))
            {
                if (inventoryUI.isActiveAndEnabled == false)
                {
                    inventoryUI.Show();
                    foreach (var item in inventoryData.GetCurrentInventoryState())
                    {
                        inventoryUI.UpdateData(item.Key,
                                               item.Value.item.ItemImage,
                                               item.Value.quantity);
                    }
                }
                else
                {
                    inventoryUI.Hide();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                inventoryUI.Hide();
            }
        }
    }
}
