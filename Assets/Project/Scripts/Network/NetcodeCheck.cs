using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetcodeCheck : MonoBehaviour
{
    void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            // Subscribe to connection and disconnection events
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }
    }

    private void OnClientDisconnect(ulong clientId)
    {
        // If the host disconnects, the server will shut down
        if (NetworkManager.Singleton.IsServer && clientId == NetworkManager.ServerClientId)
        {
            Debug.Log("Host has disconnected.");
            Destroy(GameObject.Find("ObjetPersistant"));
            Destroy(FindAnyObjectByType<NetworkManager>().gameObject);

            StartCoroutine(UnloadResources());

            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
            }

            // Charger la scène de manière asynchrone
            SceneManager.LoadSceneAsync(sceneName: "Menu", LoadSceneMode.Single);
        }
        else
        {
            Debug.Log($"Client {clientId} has disconnected.");
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadSceneAsync(sceneName: "Menu", LoadSceneMode.Single);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

    private IEnumerator UnloadResources()
    {
        yield return Resources.UnloadUnusedAssets();
        Debug.Log("Unused assets have been unloaded.");
    }
}
