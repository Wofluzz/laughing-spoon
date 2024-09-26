using Inventory.UI;
using Unity.Netcode;
using UnityEngine;

public class MouseFollower : NetworkBehaviour
{
    [SerializeField]
    private GameObject Gamecanvas;

    private Canvas canvas;

    [SerializeField]
    private UIInventoryItem item;

    private void Awake()
    {
        Canvas canvas = Gamecanvas.GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas component not found on the root GameObject.");
            return;
        }

        item = GetComponentInChildren<UIInventoryItem>();
        if (item == null)
        {
            Debug.LogError("UIInventoryItem component not found in children.");
            return;
        }
    }

    public void SetData(Sprite sprite, int quantity)
    {
        item.SetData(sprite, quantity);
    }

    private void Update()
    {
        Canvas canvas = Gamecanvas.GetComponent<Canvas>();
        // Assurez-vous que cela ne fonctionne que pour le client local
        if (!IsOwner) return;

        if (canvas == null)
        {
            Debug.LogWarning("Canvas is missing.");
            return;
        }

        Vector2 position;
        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            Input.mousePosition,
            null,  // Aucun besoin de caméra pour Screen Space - Overlay
            out position
        );

        if (success)
        {
            // Convertir la position locale en position mondiale
            Vector3 worldPosition = canvas.transform.TransformPoint(position);
            transform.position = worldPosition;
        }
        else
        {
            Debug.LogWarning("Échec de la conversion de la position de la souris en point local.");
        }
    }

    public void Toggle(bool val)
    {
        gameObject.SetActive(val);
    }
}
