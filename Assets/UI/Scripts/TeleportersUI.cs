using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TeleportersUI : MonoBehaviour
{
    public TeleporterManager teleporterManager; // Référence au gestionnaire de téléporteurs
    public GameObject teleporterButtonPrefab;  // Le prefab pour les boutons de téléporteurs
    public Transform teleporterListContainer;  // Le conteneur où seront placés les boutons

    public TextMeshProUGUI infoText;  // UI Text pour afficher les infos du téléporteur

    // Start is called before the first frame update
    void Start()
    {
        UpdateTeleporterUI();
    }

    // Met à jour l'UI en ajoutant des boutons pour chaque téléporteur déverrouillé
    public void UpdateTeleporterUI()
    {
        // Nettoyer l'UI avant de réajouter les téléporteurs
        foreach (Transform child in teleporterListContainer)
        {
            Destroy(child.gameObject);
        }

        // Créer un bouton pour chaque téléporteur
        foreach (TeleporterManager.TeleporterIdentity teleporter in teleporterManager.Teleporters)
        {
            GameObject newButton = Instantiate(teleporterButtonPrefab, teleporterListContainer);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = teleporter.PlaceName; // Affiche le nom du lieu

            // Ajoute des actions pour survoler ou cliquer sur le bouton
            Button button = newButton.GetComponent<Button>();
            int teleporterId = teleporter.Id;

            // Action de téléportation au clic
            button.onClick.AddListener(() => TeleportTo(teleporterId));

            // Ajoute des listeners pour détecter quand on survole le bouton avec la souris
            EventTriggerUtility.AddEventTrigger(newButton, EventTriggerType.PointerEnter, () => ShowTeleporterInfo(teleporter));
            EventTriggerUtility.AddEventTrigger(newButton, EventTriggerType.PointerExit, HideTeleporterInfo);
        }
    }

    // Action pour téléporter le joueur vers le téléporteur sélectionné
    public void TeleportTo(int teleporterId)
    {
        TeleporterManager.TeleporterIdentity targetTeleporter = teleporterManager.Teleporters[teleporterId];
        Transform player = FindObjectOfType<PlayerController>().transform;
        player.position = targetTeleporter.transform.position;
        Debug.Log("Téléportation vers: " + targetTeleporter.PlaceName);
    }

    // Affiche les infos du téléporteur (nom et description) dans l'UI
    public void ShowTeleporterInfo(TeleporterManager.TeleporterIdentity teleporter)
    {
        infoText.text = $"Lieu : {teleporter.PlaceName}\nDescription : {GetDescription(teleporter.Id)}";
        infoText.gameObject.SetActive(true);
    }

    // Cache les infos du téléporteur
    public void HideTeleporterInfo()
    {
        infoText.text = string.Empty;
        infoText.gameObject.SetActive(false);
    }

    // Simule la récupération de la description du téléporteur par son Id
    private string GetDescription(int teleporterId)
    {
        // Exemples de descriptions pour les téléporteurs, tu peux améliorer ça
        string[] descriptions = {
            "Un lieu mystique entouré de montagnes.",
            "Une petite ville au bord d'un lac.",
            "Une plaine ouverte avec une vue dégagée sur l'horizon."
        };

        if (teleporterId < descriptions.Length)
        {
            return descriptions[teleporterId];
        }
        else
        {
            return "Aucune description disponible.";
        }
    }
}
