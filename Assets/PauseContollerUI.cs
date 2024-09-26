using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseControllerUI : MonoBehaviour
{
    [SerializeField] private Button pauseBtn, continueBtn, offLineBtn, settingsBtn;

    public bool IsPauseUIActive = false;

    [SerializeField] private GameObject panel;

    void Start()
    {
        // Abonnement des listeners une seule fois dans Start()
        pauseBtn.onClick.AddListener(OnPauseButtonClick);
        continueBtn.onClick.AddListener(OnContinueButtonClick);
        offLineBtn.onClick.AddListener(OnOfflineButtonClick);
        settingsBtn.onClick.AddListener(OnSettingsButtonClick);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            IsPauseUIActive = !IsPauseUIActive;
        }

        panel.SetActive(IsPauseUIActive);
    }

    private void OnPauseButtonClick()
    {
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

    private void OnContinueButtonClick()
    {
        panel.SetActive(false);
        IsPauseUIActive = false;
    }

    private void OnOfflineButtonClick()
    {
        Debug.Log("Cette fonctionnalité n'est pas disponible");
    }

    private void OnSettingsButtonClick()
    {
        Debug.Log("Menu Settings");
    }

    private IEnumerator UnloadResources()
    {
        yield return Resources.UnloadUnusedAssets();
        Debug.Log("Unused assets have been unloaded.");
    }
}
