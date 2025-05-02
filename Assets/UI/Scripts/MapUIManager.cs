using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUIManager : MonoBehaviour
{
    [SerializeField] private GameObject mapUI;
    private bool isMapOpen = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || (Input.GetKeyDown(KeyCode.M) && isMapOpen))
        {
            CloseMap();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            OpenMap();
        }
    }

    private void CloseMap()
    {
        mapUI.SetActive(false);
        isMapOpen = false;
    }

    private void OpenMap()
    {
        mapUI.SetActive(true);
        isMapOpen = true;
    }
}
