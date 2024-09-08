using Inventory;
using Inventory.Model;
using UnityEngine;
using Random = System.Random;

public class DropItems : MonoBehaviour
{
    public int quantity;
    public ItemSO Loot;
    public LootPoolSO LootPool;

    [SerializeField]
    private GameObject droppedItem;
    [SerializeField]
    private InventoryController inventoryController;

    private void Awake()
    {
        inventoryController = FindObjectOfType<PlayerController>().gameObject.GetComponent<InventoryController>();
    }

    public void DropItem()
    {
        //audioSource.PlayOneShot(dropClip);
        Random rdn = new Random();
        Vector3 playerPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1);

        if (Loot != null)
        {
            GameObject itemDropped = Instantiate(droppedItem);
            itemDropped.transform.position = playerPos;
            inventoryController.ItemDropping(itemDropped, Loot, quantity);
        }

        if (LootPool != null)
        {
            Debug.Log(LootPool.lootPool);
            foreach (ItemsInLootPool item in LootPool.lootPool)
            {
                Debug.Log(item.item);
                GameObject itemDropped = Instantiate(droppedItem);
                itemDropped.transform.position = playerPos;
                inventoryController.ItemDropping(itemDropped, item.item, (int)rdn.Next(item.MinQuantity, item.MaxQuantity + 1));
            }
        }
    }

    public void DestroyMaterial()
    {
        Destroy(gameObject);
    }
}
