using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Singleton;
    public delegate void NotifyMessage(string message);
    public event NotifyMessage OnMessage;

    [SerializeField] private GameObject GUI;

    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] CanvasGroup chatContent;
    [SerializeField] TMP_InputField chatInput;

    private bool isChatOpen = false;
    public bool isChatting = false;

    public string playerName;

    private void Awake()
    {   
        ChatManager.Singleton = this;
        playerName = PlayerPrefs.GetString("PlayerPseudo");
    }

    private void Update()
    {
        playerName = PlayerAccountSystem.Singleton.Pseudo;
        if (Input.GetKeyDown(KeyCode.T))
        {
            OpenChatTab();
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) { isChatting = false; isChatOpen = false; }

        if (isChatOpen) GUI.SetActive(true);
        else GUI.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendChatMessage(chatInput.text, playerName);
            chatInput.text = "";
        }
    }

    public void OpenChatTab()
    {
        isChatOpen = true;
        isChatting = true;
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
        OnMessage?.Invoke(message);
    }
}
