using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    private List<GameObject> buttons = new List<GameObject>();

    [SerializeField]
    private GameObject choiceBox;
    [SerializeField]
    private GameObject choice;
    public DialogueSO dialogue;  // Assurez-vous que ceci est assigné via l'Inspector ou par un autre moyen
    public float textSpeed;

    private int index;
    private Coroutine typingCoroutine; // Pour garder une référence à la coroutine en cours
    private bool hasOptions; // Indicateur si la ligne actuelle a des options
    private bool IsDialogueActive = false;

    void Start()
    {
        textComponent.text = string.Empty;
    }

    public void GoToNextDialogue()
    {
        if (textComponent.text == dialogue.lines[index].Text)
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines(); // Arrêter toutes les coroutines, y compris le typage en cours
            textComponent.text = dialogue.lines[index].Text; // Affiche le texte complet immédiatement
        }
    }

    public void StartDialogue(DialogueSO dialogue)
    {
        if (IsDialogueActive) return;
        IsDialogueActive=true;
        ClearButtons();  // Nettoyer tous les anciens boutons d'option
        textComponent.text = string.Empty;
        FindAnyObjectByType<PlayerAttack>().gameObject.GetComponent<PlayerAttack>().enabled = false;
        gameObject.SetActive(true);
        this.dialogue = dialogue;
        index = 0;
        StartTyping(); // Démarrer le typage
    }

    IEnumerator TypeLine()
    {
        textComponent.ForceMeshUpdate();  // Forcer la mise à jour du texte pour éviter des artefacts
        textComponent.text = string.Empty; // Assurez-vous que le texte est vide avant de commencer à typer

        yield return null; // Attendre un frame pour garantir que le texte est correctement mis à jour

        foreach (char c in dialogue.lines[index].Text.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void StartTyping()
    {
        // Si une coroutine est déjà en cours, l'arrêter avant d'en démarrer une nouvelle
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        hasOptions = false; // Réinitialiser l'indicateur pour chaque nouvelle ligne
        typingCoroutine = StartCoroutine(TypeLine());

        if (dialogue.lines[index].Options != null && dialogue.lines[index].Options.Count > 0)
        {
            hasOptions = true;
            choiceBox.SetActive(true);
            choice.SetActive(true);

            foreach (var item in dialogue.lines[index].Options)
            {
                GameObject Button = Instantiate(choice, choiceBox.transform);
                Button.GetComponentInChildren<TMP_Text>().text = item.Option;
                buttons.Add(Button);

                // Ajouter un listener à chaque bouton
                Button.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(item.ReplyIndex));
            }
            choice.SetActive(false);
        }
    }

    void NextLine()
    {
        if (index < dialogue.lines.Count - 1)
        {
            if (!hasOptions)
            {
                index++;
                StartTyping();
            }
        }
        else
        {
            Exit();
        }
    }

    void OnChoiceSelected(int replyIndex)
    {
        if (replyIndex == -1)
        {
            Exit();
            return;
        }

        index = replyIndex;
        ClearButtons();  // Nettoyer les boutons d'options
        StartTyping(); // Continuer à la ligne suivante
    }

    void ClearButtons()
    {
        foreach (var button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();
    }

    void Exit()
    {
        IsDialogueActive = false;
        StopAllCoroutines();
        FindAnyObjectByType<PlayerAttack>().gameObject.GetComponent<PlayerAttack>().enabled = true;
        gameObject.SetActive(false);
    }
}
