using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Singleton;

    [SerializeField] private GameObject GUI;

    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] CanvasGroup chatContent;
    [SerializeField] TMP_InputField chatInput;

    private bool isChatOpen = false;
    public bool isChatting = false;


    public string playerName;

    private void Awake()
    {   ChatManager.Singleton = this;   }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            isChatOpen = true;
            isChatting = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) { isChatOpen = false; isChatting = false; }

        if (isChatOpen) GUI.SetActive(true);
        else GUI.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendChatMessage(chatInput.text, playerName);
            chatInput.text = "";
        }
    }

    public void SendChatMessage(string _message, string _fromWho = null)
    {
        if (string.IsNullOrWhiteSpace(_message)) return;
        string S = _fromWho + " > " + _message;
        SendChatMessageServerRpc(S);
    }

    void AddMessage(string msg)
    {
        ChatMessage CM = Instantiate(chatMessagePrefab, chatContent.transform);
        CM.SetText(msg);
    }

    [ServerRpc(RequireOwnership = false)]
    void SendChatMessageServerRpc(string message)
    {
        ReceiveChatMessageClientRpc(message);
    }

    [ClientRpc]
    void ReceiveChatMessageClientRpc(string message)
    {
        ChatManager.Singleton.AddMessage(message);
    }
}
