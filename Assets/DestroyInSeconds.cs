using UnityEngine;

public class DestroyInSeconds : MonoBehaviour
{
    [SerializeField]
    private float secondsToDestroy = 1f;

    void Start()
    {
        Destroy(gameObject, secondsToDestroy);
    }
}
