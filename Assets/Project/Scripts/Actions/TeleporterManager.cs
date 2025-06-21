using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterManager : MonoBehaviour
{
    // Liste des téléporteurs disponibles dans le jeu
    public List<TeleporterIdentity> Teleporters { get; private set; } = new List<TeleporterIdentity>();

    [Serializable]
    public class TeleporterIdentity
    {
        public Transform transform;
        public string PlaceName;
        public int Id;

        public TeleporterIdentity(Transform transform, string placeName, int id)
        {
            this.transform = transform;
            this.PlaceName = placeName;
            this.Id = id;
        }
    }

    private void Start()
    {
        // Abonne tous les téléporteurs à l'événement d'activation
        Teleporter[] allTeleporters = FindObjectsByType<Teleporter>(FindObjectsSortMode.None);
        foreach (Teleporter teleporter in allTeleporters)
        {
            teleporter.OnTPActivation += AddTeleporter;
        }
    }

    // Ajoute un nouveau téléporteur à la liste une fois déverrouillé
    private void AddTeleporter(Teleporter teleporter)
    {
        int newId = Teleporters.Count;
        Teleporters.Add(new TeleporterIdentity(teleporter.transform, teleporter.name, newId));
        Debug.Log("Nouveau téléporteur ajouté : " + teleporter.name);
    }
}
