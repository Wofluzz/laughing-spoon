using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TeleportersUI : MonoBehaviour
{
    public TeleporterManager teleporterManager; // R�f�rence au gestionnaire de t�l�porteurs
    public GameObject teleporterButtonPrefab;  // Le prefab pour les boutons de t�l�porteurs
    public Transform teleporterListContainer;  // Le conteneur o� seront plac�s les boutons

    public TextMeshProUGUI infoText;  // UI Text pour afficher les infos du t�l�porteur

    // Start is called before the first frame update
    void Start()
    {
        UpdateTeleporterUI();
    }

    // Met � jour l'UI en ajoutant des boutons pour chaque t�l�porteur d�verrouill�
    public void UpdateTeleporterUI()
    {
        // Nettoyer l'UI avant de r�ajouter les t�l�porteurs
        foreach (Transform child in teleporterListContainer)
        {
            Destroy(child.gameObject);
        }

        // Cr�er un bouton pour chaque t�l�porteur
        foreach (TeleporterManager.TeleporterIdentity teleporter in teleporterManager.Teleporters)
        {
            GameObject newButton = Instantiate(teleporterButtonPrefab, teleporterListContainer);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = teleporter.PlaceName; // Affiche le nom du lieu

            // Ajoute des actions pour survoler ou cliquer sur le bouton
            Button button = newButton.GetComponent<Button>();
            int teleporterId = teleporter.Id;

            // Action de t�l�portation au clic
            button.onClick.AddListener(() => TeleportTo(teleporterId));

            // Ajoute des listeners pour d�tecter quand on survole le bouton avec la souris
            EventTriggerUtility.AddEventTrigger(newButton, EventTriggerType.PointerEnter, () => ShowTeleporterInfo(teleporter));
            EventTriggerUtility.AddEventTrigger(newButton, EventTriggerType.PointerExit, HideTeleporterInfo);
        }
    }

    // Action pour t�l�porter le joueur vers le t�l�porteur s�lectionn�
    public void TeleportTo(int teleporterId)
    {
        TeleporterManager.TeleporterIdentity targetTeleporter = teleporterManager.Teleporters[teleporterId];
        Transform player = FindObjectOfType<PlayerController>().transform;
        player.position = targetTeleporter.transform.position;
        Debug.Log("T�l�portation vers: " + targetTeleporter.PlaceName);
    }

    // Affiche les infos du t�l�porteur (nom et description) dans l'UI
    public void ShowTeleporterInfo(TeleporterManager.TeleporterIdentity teleporter)
    {
        infoText.text = $"Lieu : {teleporter.PlaceName}\nDescription : {GetDescription(teleporter.Id)}";
        infoText.gameObject.SetActive(true);
    }

    // Cache les infos du t�l�porteur
    public void HideTeleporterInfo()
    {
        infoText.text = string.Empty;
        infoText.gameObject.SetActive(false);
    }

    // Simule la r�cup�ration de la description du t�l�porteur par son Id
    private string GetDescription(int teleporterId)
    {
        // Exemples de descriptions pour les t�l�porteurs, tu peux am�liorer �a
        string[] descriptions = {
            "Un lieu mystique entour� de montagnes.",
            "Une petite ville au bord d'un lac.",
            "Une plaine ouverte avec une vue d�gag�e sur l'horizon."
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
