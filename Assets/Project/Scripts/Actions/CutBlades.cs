using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutBlades : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        List<GameObject> unityGameObjects = new List<GameObject>();
        if (collision.tag == "Player")
        {
           Destroy(gameObject);
           Instantiate(gameObject);
        }
    }
}
