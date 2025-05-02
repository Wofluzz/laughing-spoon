using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    // R�f�rence vers la position du t�l�porteur
    public Transform teleporterTransform;

    // D�l�gu� pour l'�v�nement de d�blocage d'un nouveau t�l�porteur
    public delegate void UnlockNewTP(Teleporter teleporter);
    public event UnlockNewTP OnTPActivation;

    // M�thode pour d�verrouiller le t�l�porteur
    public void UnlockTeleporter()
    {
        Debug.Log("T�l�porteur d�verrouill�: " + gameObject.name);

        // D�clenche l'�v�nement pour informer le TeleporterManager
        OnTPActivation?.Invoke(this);
    }
}
