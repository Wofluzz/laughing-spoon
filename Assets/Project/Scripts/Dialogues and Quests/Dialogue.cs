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
    public DialogueSO dialogue;  // Assurez-vous que ceci est assign� via l'Inspector ou par un autre moyen
    public float textSpeed;

    private int index;
    private Coroutine typingCoroutine; // Pour garder une r�f�rence � la coroutine en cours
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
            StopAllCoroutines(); // Arr�ter toutes les coroutines, y compris le typage en cours
            textComponent.text = dialogue.lines[index].Text; // Affiche le texte complet imm�diatement
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
        StartTyping(); // D�marrer le typage
    }

    IEnumerator TypeLine()
    {
        textComponent.ForceMeshUpdate();  // Forcer la mise � jour du texte pour �viter des artefacts
        textComponent.text = string.Empty; // Assurez-vous que le texte est vide avant de commencer � typer

        yield return null; // Attendre un frame pour garantir que le texte est correctement mis � jour

        foreach (char c in dialogue.lines[index].Text.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void StartTyping()
    {
        // Si une coroutine est d�j� en cours, l'arr�ter avant d'en d�marrer une nouvelle
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        hasOptions = false; // R�initialiser l'indicateur pour chaque nouvelle ligne
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

                // Ajouter un listener � chaque bouton
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
        StartTyping(); // Continuer � la ligne suivante
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
