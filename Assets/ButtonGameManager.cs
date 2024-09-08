using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ButtonGameManager : MonoBehaviour
{
    public Button startClientButton; // Reference to the "Start Client" button
    public Button startServerButton; // Reference to the "Start Server" button
    public Button launchGameButton;  // Reference to the "Launch Game" button

    public GameObject Panel;

    public string gameSceneName = "Test3D"; // Name of the scene to load

    void Start()
    {
        // Ensure buttons are assigned in the Inspector
        if (startClientButton != null)
        {
            startClientButton.onClick.AddListener(StartClient);
        }

        if (startServerButton != null)
        {
            startServerButton.onClick.AddListener(StartServer);
        }

        if (launchGameButton != null)
        {
            launchGameButton.onClick.AddListener(LaunchGame);
        }
    }

    // Start the client
    void StartClient()
    {
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client started!");
        }
        else
        {
            Debug.LogWarning("NetworkManager is null or Client is already running.");
        }
    }

    // Start the server
    void StartServer()
    {
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.StartServer();
            Debug.Log("Server started!");
        }
        else
        {
            Debug.LogWarning("NetworkManager is null or Server is already running.");
        }
    }

    // Launch the game and change the scene
    void LaunchGame()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // Only the server can change the scene in Netcode for GameObjects
            NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
            Panel.SetActive(false);
            Debug.Log("Scene changing to " + gameSceneName);
        }
        else
        {
            Debug.LogWarning("Only the server can change the scene. Start as a server first.");
        }
    }
}
