using Inventory2D.Model;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int coinCount = 0, score = 0;

    public PowerUp_SO CurrentPowerUp;

    // Etats du jeu  
    public bool isPaused = false;

    private bool showBoard = false; // Cette variable doit contrôler si le tableau est visible ou non.

    public enum Values
    {
        score,
        coins,
    }
    public static event Action<GameManager.Values> OnValueChanged;

    [SerializeField]
    private TMPro.TextMeshProUGUI coinText, scoreText;
    [SerializeField]
    private GameObject TopUIBoard;
    [SerializeField]
    private GameObject floatingTextPrefab;

    private Coroutine currentBoardAnimationCoroutine; // Pour garder une référence à la coroutine d'animation en cours

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleInfoBoard();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ToggleInfoBoard()
    {
        showBoard = !showBoard; // Inverse l'état du showBoard
        // Déclenche l'animation du panneau basé sur le nouvel état de showBoard
        TriggerBoardAnimation(showBoard);
    }

    void Start()
    {
        coinCount = PlayerPrefs.GetInt("Coins", 0);
        OnValueChanged += (valTypes) => UpdateValuesText(valTypes);
        PlayerMovements.OnPlayerDied += () => StartCoroutine(DelayedEndgame());

        // S'assurer que les textes sont masqués au démarrage si le tableau n'est pas visible
        if (!showBoard)
        {
            scoreText.gameObject.SetActive(false);
            coinText.gameObject.SetActive(false);
        }
    }

    private IEnumerator DelayedEndgame()
    {
        yield return new WaitForSeconds(2f);
        RestartLevel();
    }

    private void UpdateValuesText(Values valTypes)
    {
        // Activer le panneau si une valeur change et qu'il n'est pas déjà affiché
        if (!showBoard) // S'il n'est pas censé être affiché via Tab, on le montre temporairement
        {
            showBoard = true; // On met à jour l'état interne pour l'animation
            TriggerBoardAnimation(true); // Lance l'animation pour montrer le panneau
        }

        switch (valTypes)
        {
            case Values.score:
                scoreText.text = "Score : " + score.ToString("D10");
                scoreText.gameObject.SetActive(true); // S'assurer que le texte du score est visible
                break;
            case Values.coins:
                coinText.text = coinCount.ToString() + " <sprite name=\"PotatoCoin\" index=0>";
                coinText.gameObject.SetActive(true); // S'assurer que le texte des pièces est visible
                break;
            default:
                break;
        }

        // Lance la coroutine pour cacher les compteurs après un délai
        StartCoroutine(HideCounter(valTypes));
    }

    // Nouvelle méthode pour lancer et gérer l'animation du tableau
    private void TriggerBoardAnimation(bool targetShowState)
    {
        // Si une animation est déjà en cours, on l'arrête pour éviter les conflits
        if (currentBoardAnimationCoroutine != null)
        {
            StopCoroutine(currentBoardAnimationCoroutine);
        }
        // Lance la nouvelle coroutine et stocke sa référence
        currentBoardAnimationCoroutine = StartCoroutine(AnimateGameInfosBoard(targetShowState));
    }

    private IEnumerator HideCounter(Values valTypes)
    {
        yield return new WaitForSeconds(10f);
        switch (valTypes)
        {
            case Values.score:
                // Masquer uniquement si le panneau n'est pas explicitement affiché via Tab
                if (!showBoard)
                {
                    scoreText.gameObject.SetActive(false);
                }
                break;
            case Values.coins:
                // Masquer uniquement si le panneau n'est pas explicitement affiché via Tab
                if (!showBoard)
                {
                    coinText.gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }
        // Si les deux sont inactifs et que le panneau n'est pas censé être montré (via Tab), on le cache
        if (!showBoard && !scoreText.gameObject.activeSelf && !coinText.gameObject.activeSelf)
        {
            TriggerBoardAnimation(false); // Lance l'animation pour cacher le panneau
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddCoins(int value)
    {
        coinCount += value;
        OnValueChanged?.Invoke(GameManager.Values.coins);
    }

    public void AddScore(int value)
    {
        score += value;
        OnValueChanged?.Invoke(GameManager.Values.score);
    }

    public void ShowScore(string text, GameObject obj)
    {
        if (floatingTextPrefab)
        {
            GameObject prefab = Instantiate(floatingTextPrefab, obj.transform.position, Quaternion.identity);
            prefab.GetComponentInChildren<TextMesh>().text = text;
        }
    }

    // La coroutine pour l'animation du tableau, maintenant plus générique
    public IEnumerator AnimateGameInfosBoard(bool show)
    {
        // Active les textes au début si le panneau doit apparaître
        if (show)
        {
            scoreText.gameObject.SetActive(true);
            coinText.gameObject.SetActive(true);
        }

        Vector3 currentVelocity = Vector3.zero;
        Vector3 targetPosition;

        // Détermine la position cible en fonction de si le panneau doit être montré ou caché
        if (show)
        {
            targetPosition = new Vector3(TopUIBoard.transform.position.x, TopUIBoard.transform.position.y - 50, TopUIBoard.transform.position.z);
        }
        else
        {
            targetPosition = new Vector3(TopUIBoard.transform.position.x, TopUIBoard.transform.position.y + 50, TopUIBoard.transform.position.z);
        }

        float duration = 0.5f; // Vitesse de l'animation
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            TopUIBoard.transform.position = Vector3.SmoothDamp(
                TopUIBoard.transform.position,
                targetPosition,
                ref currentVelocity,
                duration,
                Mathf.Infinity, // Vitesse maximale (laisser à l'infini pour un amortissement complet)
                Time.deltaTime
            );

            elapsedTime += Time.deltaTime;
            yield return null; // Attendre la prochaine frame
        }

        // S'assurer que la position finale est correcte après l'animation
        TopUIBoard.transform.position = targetPosition;

        // Désactive les textes à la fin si le panneau doit être caché
        if (!show)
        {
            scoreText.gameObject.SetActive(false);
            coinText.gameObject.SetActive(false);
        }

        currentBoardAnimationCoroutine = null; // Réinitialise la référence de la coroutine une fois terminée
    }
}
