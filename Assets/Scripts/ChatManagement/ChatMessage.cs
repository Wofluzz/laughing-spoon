using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] TMP_Text messageText;

    public void SetText(string str)
    { messageText.text = str; }
}
