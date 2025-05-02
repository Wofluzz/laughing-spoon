using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.U2D;

public class InteractionInstigator : NetworkBehaviour
{
    private List<Interactable> m_NearbyInteractables = new List<Interactable>();

    [SerializeField]
    private bool m_2DGM_Switch; // Permet l'interaction sans touche en mode 2D

    [SerializeField]
    public TextMeshProUGUI textComponent;

    public bool HasNearbyInteractables()
    {
        return m_NearbyInteractables.Count != 0;
    }

    private void Update()
    {
        //if (!IsOwner) return;
        if (!m_2DGM_Switch)
        {
            if (HasNearbyInteractables() && Input.GetButtonDown("Submit"))
            {
                //textComponent.text = string.Empty;
                m_NearbyInteractables[0].DoInteraction();
            }
        } else
        {
            if (HasNearbyInteractables())
            {
                //textComponent.text = string.Empty;
                m_NearbyInteractables[0].DoInteraction();
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable == null) return;
        //textComponent.text = "Appuyez sur <sprite name=Space> pour interagir";
        if (interactable != null)
        {
            m_NearbyInteractables.Add(interactable);
        }            
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable == null) return;
        //textComponent.text = string.Empty;
        if (interactable != null)
        {
            m_NearbyInteractables.Remove(interactable);
        }
       
    }
}