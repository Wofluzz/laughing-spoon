using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UIController : NetworkBehaviour
{
    public bool IsPlayerUI = true;

    void Update()
    {
        if (!IsOwner) IsPlayerUI = false;
        UIController[] controllers = FindObjectsOfType<UIController>();

        foreach (UIController controller in controllers)
        {
            if (!controller.IsPlayerUI) controller.gameObject.SetActive(false);
        }
    }
}
