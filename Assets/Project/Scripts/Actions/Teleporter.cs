using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    // Référence vers la position du téléporteur
    public Transform teleporterTransform;

    // Délégué pour l'événement de déblocage d'un nouveau téléporteur
    public delegate void UnlockNewTP(Teleporter teleporter);
    public event UnlockNewTP OnTPActivation;

    // Méthode pour déverrouiller le téléporteur
    public void UnlockTeleporter()
    {
        Debug.Log("Téléporteur déverrouillé: " + gameObject.name);

        // Déclenche l'événement pour informer le TeleporterManager
        OnTPActivation?.Invoke(this);
    }
}
