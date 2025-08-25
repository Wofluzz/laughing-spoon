using Inventory2D.Model;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Scene currentLevelScene;


    public int coinCount = 0, score = 0;

    public PowerUp_SO CurrentPowerUp;

    public bool Checkpoint;
    public GameObject CheckPointObj;

    // Etats du jeu  
    public bool isPaused = false;

    [SerializeField] private TMPro.TextMeshProUGUI coinText, scoreText;
    [SerializeField] private GameObject floatingTextPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Écoute quand une scène est chargée
            SceneManager.sceneLoaded += OnLevelLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene chargée : " + scene.name);

        // Replacer le joueur au Spawn de la scène
        var player = FindAnyObjectByType<PlayerMovements>()?.gameObject;
        var spawn = GameObject.FindGameObjectWithTag("Spawn");

        if (player != null && spawn != null)
        {
            player.transform.position = spawn.transform.position;
        }

        // Reconnecter le HUD
        var coinTextObj = GameObject.Find("CoinText");
        if (coinTextObj != null)
            coinText = coinTextObj.GetComponent<TMPro.TextMeshProUGUI>();

        var scoreTextObj = GameObject.Find("ScoreText");
        if (scoreTextObj != null)
            scoreText = scoreTextObj.GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        if (scoreText != null)
            scoreText.text = score.ToString("D10");

        if (coinText != null)
            coinText.text = coinCount.ToString();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    private void Start()
    {
        coinCount = PlayerPrefs.GetInt("Coins", 0);

        var player = FindAnyObjectByType<PlayerMovements>()?.gameObject;
        if (player != null)
        {
            Vector3 startpos;
            if (!Checkpoint)
                startpos = GameObject.FindGameObjectWithTag("Spawn").transform.position;
            else
                startpos = CheckPointObj.transform.position;

            player.transform.position = startpos;
        }

        PlayerMovements.OnPlayerDied += DeathEndGame;
    }

    private void DeathEndGame()
    {
        StartCoroutine(DelayedEndgame());
    }

    private IEnumerator DelayedEndgame()
    {
        yield return new WaitForSeconds(2f);
        RestartLevel();
    }

    public void RestartLevel()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Additive);
    }

    public void AddCoins(int value) => coinCount += value;
    public void AddScore(int value) => score += value;

    public void ShowScore(string text, GameObject obj)
    {
        if (floatingTextPrefab)
        {
            GameObject prefab = Instantiate(floatingTextPrefab, obj.transform.position, Quaternion.identity);
            prefab.GetComponentInChildren<TextMesh>().text = text;
        }
    }
}
