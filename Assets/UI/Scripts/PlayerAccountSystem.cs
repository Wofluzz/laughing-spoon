using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAccountSystem : MonoBehaviour
{
    public static PlayerAccountSystem Singleton;

    public string Pseudo = "player";

    [SerializeField]
    private TMP_InputField field;

    private const string PseudoKey = "PlayerPseudo"; // Clé pour stocker le pseudo

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else if (Singleton != this)
        {
            Destroy(gameObject);
        }

        // Chargement du pseudo depuis PlayerPrefs
        if (PlayerPrefs.HasKey(PseudoKey))
        {
            Pseudo = PlayerPrefs.GetString(PseudoKey);
        }

        // Vérification que le TMP_InputField est assigné
        if (field == null)
        {
            Debug.LogError("TMP_InputField is not assigned in the inspector");
            return;
        }

        // Initialiser le champ d'entrée avec le pseudo actuel
        field.text = Pseudo;

        // Abonnez-vous à l'événement onValueChanged pour mettre à jour le pseudo
        field.onValueChanged.AddListener(OnPseudoChanged);
    }

    // Cette méthode sera appelée chaque fois que le texte change
    void OnPseudoChanged(string newPseudo)
    {
        Pseudo = newPseudo;

        // Enregistrer le pseudo dans PlayerPrefs
        PlayerPrefs.SetString(PseudoKey, Pseudo);
        PlayerPrefs.Save();

        Debug.Log("Pseudo updated and saved: " + Pseudo);
    }
}
