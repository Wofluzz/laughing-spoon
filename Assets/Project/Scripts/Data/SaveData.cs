using Inventory.Model;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public InventorySO InventorySO;
    public Inventory inventory = new Inventory();
    private string saveFilePath;

    public  LastPlayerPositions lastPlayerPositionsParameter;

    private void Awake()
    {
        // Définir le chemin du fichier de sauvegarde
        saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");
    }

    public void SaveGame()
    {
        SaveInventory();
        SaveMoney();
    }

    public void LoadGame()
    {
        LoadInventory();
        LoadMoney();
        LoadPosition();
    }

    private void LoadPosition()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SerializableInventory serializableInventory = JsonUtility.FromJson<SerializableInventory>(json);
            gameObject.transform.position = new Vector3(serializableInventory.lastPlayerPositions.X, serializableInventory.lastPlayerPositions.Y, gameObject.transform.position.z);
        };
    }

        private void SaveInventory()
        {
        // Convertir l'état de l'inventaire en format JSON
        lastPlayerPositionsParameter = new LastPlayerPositions
        {
            X = gameObject.transform.position.x,
            Y = gameObject.transform.position.y
        };
        SerializableInventory serializableInventory = new SerializableInventory(InventorySO.GetCurrentInventoryState(), lastPlayerPositionsParameter);
        string json = JsonUtility.ToJson(serializableInventory, true);

        // Écrire le fichier JSON à l'emplacement spécifié
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Inventaire sauvegardé dans " + saveFilePath);
    }

    private void LoadInventory()
    {
        // Vérifie si le fichier existe avant de charger
        if (File.Exists(saveFilePath))
        {
            InventorySO.Initialize(); // Tu peux enlever cette ligne si elle réinitialise l'inventaire.
            string json = File.ReadAllText(saveFilePath);
            SerializableInventory serializableInventory = JsonUtility.FromJson<SerializableInventory>(json);

            // Ajouter chaque item à l'inventaire
            foreach (var item in serializableInventory.inventoryData)
            {
                ItemSO itemData = ItemDatabase.GetItemById(item.itemId); // Obtenir l'item par ID
                if (itemData != null)
                {
                    InventoryItem inventoryItem = new InventoryItem
                    {
                        item = itemData,
                        quantity = item.quantity,
                        itemState = new List<ItemParameter>() // Initialise avec des paramètres par défaut si nécessaire
                    };
                    InventorySO.AddItem(inventoryItem); // Utiliser la méthode AddItem
                }
                else
                {
                    Debug.LogError($"Item avec l'ID {item.itemId} non trouvé dans la base de données.");
                }
            }

            Debug.Log("Inventaire chargé depuis " + saveFilePath);
        }
        else
        {
            Debug.LogWarning("Aucun fichier de sauvegarde trouvé à " + saveFilePath);
        }
    }



    private void SaveMoney()
    {
        // Sauvegarder les données de l'argent dans le fichier
        PlayerPrefs.SetInt("Money", inventory.Money);
    }

    private void LoadMoney()
    {
        if (PlayerPrefs.HasKey("Money"))
        {
            inventory.Money = PlayerPrefs.GetInt("Money");
        }
        else
        {
            inventory.Money = 0; // Valeur par défaut si aucune sauvegarde n'existe
        }
    }

    [System.Serializable]
    public class Inventory
    {
        public int Money;
        // Retirer le dictionnaire car nous l'obtenons directement de InventorySO
    }

    // Classe pour sérialiser l'inventaire dans un format adapté pour le JSON
    [System.Serializable]
    public class SerializableInventory
    {
        public List<SerializableInventoryItem> inventoryData = new List<SerializableInventoryItem>();

        public LastPlayerPositions lastPlayerPositions;

        // Convertit le dictionnaire d'objets en une liste sérialisable
        public SerializableInventory(Dictionary<int, InventoryItem> inventoryItems, LastPlayerPositions lastPositions)
        {
            foreach (var item in inventoryItems)
            {
                inventoryData.Add(new SerializableInventoryItem(item.Value));
            }
            this.lastPlayerPositions = lastPositions;
        }
    }
    [System.Serializable]
    public struct LastPlayerPositions
    {
        public float X;
        public float Y;
    }

    [System.Serializable]
    public class SerializableInventoryItem
    {
        public int itemId;
        public int quantity;

        // Convertit InventoryItem en SerializableInventoryItem pour la sauvegarde
        public SerializableInventoryItem(InventoryItem inventoryItem)
        {
            itemId = inventoryItem.item.ID;
            quantity = inventoryItem.quantity;
        }
    }
}
