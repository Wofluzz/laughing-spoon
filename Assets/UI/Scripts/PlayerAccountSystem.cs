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

    private const string PseudoKey = "PlayerPseudo"; // Cl� pour stocker le pseudo

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

        // V�rification que le TMP_InputField est assign�
        if (field == null)
        {
            Debug.LogError("TMP_InputField is not assigned in the inspector");
            return;
        }

        // Initialiser le champ d'entr�e avec le pseudo actuel
        field.text = Pseudo;

        // Abonnez-vous � l'�v�nement onValueChanged pour mettre � jour le pseudo
        field.onValueChanged.AddListener(OnPseudoChanged);
    }

    // Cette m�thode sera appel�e chaque fois que le texte change
    void OnPseudoChanged(string newPseudo)
    {
        Pseudo = newPseudo;

        // Enregistrer le pseudo dans PlayerPrefs
        PlayerPrefs.SetString(PseudoKey, Pseudo);
        PlayerPrefs.Save();

        Debug.Log("Pseudo updated and saved: " + Pseudo);
    }
}
