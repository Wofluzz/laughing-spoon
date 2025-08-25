using Unity.Cinemachine;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public string LevelName;
    public Transform Player;
    [SerializeField]
    private CinemachineConfiner2D confiner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadSceneAsync(LevelName, LoadSceneMode.Additive).completed += OnLevelLoaded;
    }

    private void OnLevelLoaded(AsyncOperation obj)
    {
        GameObject spawn = GameObject.FindGameObjectWithTag("Spawner");
        GameObject bounds = GameObject.FindGameObjectWithTag("CameraCollider");
        if (spawn != null && Player != null)
        {
            Player.position = spawn.transform.position;
            Player.GetComponent<TrailRenderer>().enabled = true;
        }
        if (bounds)
        {
            confiner.BoundingShape2D = bounds.GetComponent<PolygonCollider2D>();
        }
        GameManager.instance.currentLevelScene = SceneManager.GetSceneByName(LevelName);
    }
}
