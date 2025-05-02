using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;

public class GameModeManager : MonoBehaviour
{
    [SerializeField] private Button startSoloButton;    // Reference to the "Start Solo" button
    [SerializeField] private Button hostButton;         // Reference to the "Host Game" button
    [SerializeField] private Button joinButton;         // Reference to the "Join Game" button

    [SerializeField] private TMP_InputField ipInputField;   // Input field for the IP address (for joining)
    [SerializeField] private TMP_InputField portInputField; // Input field for the port (for joining or hosting)

    [SerializeField] private TMP_Text Info;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject NetworkManagerObject;

    public string gameSceneName = "GameScene"; // Name of the scene to load for the game

    void Start()
    {
        // Ensure buttons are assigned in the Inspector
        if (startSoloButton != null)
        {
            startSoloButton.onClick.AddListener(StartGameSolo);
        }

        if (hostButton != null)
        {
            hostButton.onClick.AddListener(StartHost);
        }

        if (joinButton != null)
        {
            joinButton.onClick.AddListener(JoinGame);
        }

        // Set default IP and port values
        if (ipInputField != null)
        {
            ipInputField.text = "127.0.0.1"; // Default IP for localhost
        }

        if (portInputField != null)
        {
            portInputField.text = "7777"; // Default port
        }
    }

    // Start the game solo (without network)
    void StartGameSolo()
    {
        DebugInfo("Starting game in solo mode...");
        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        Instantiate(player);
        NetworkManagerObject.SetActive(false);
    }

    // Start as Host (server + client)
    void StartHost()
    {
        SetIPAndPort();

        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.StartHost();
            DebugInfo("Hosting the game with IP: " + ipInputField.text + " and Port: " + portInputField.text);
            NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("NetworkManager is null or already hosting.");
        }
    }

    // Join an existing game as a client
    void JoinGame()
    {
        SetIPAndPort();

        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StartClient();
            DebugInfo("Joining the game at IP: " + ipInputField.text + " and Port: " + portInputField.text);
            NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("NetworkManager is null or Client is already running.");
        }
    }

    // Set the IP and Port based on the input fields
    void SetIPAndPort()
    {
        if (NetworkManager.Singleton != null)
        {
            var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
            if (transport != null)
            {
                transport.ConnectionData.Address = ipInputField.text;

                if (ushort.TryParse(portInputField.text, out ushort port))
                {
                    transport.ConnectionData.Port = port;
                }
                else
                {
                    Debug.LogWarning("Invalid port number, using default.");
                }
            }
        }
    }

    void DebugInfo(string InfoTxt)
    {
        Info.text = InfoTxt;
    }
}
