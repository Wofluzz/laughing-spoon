using UnityEngine;
using UnityEngine.EventSystems;

public class ActiveATH : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField]
    private float speed = 2.0f;
    [SerializeField]
    private float timeToStill = 15.0f; 

    private Vector3 targetPosition;   
    private bool isOpen = false;      
    private float originalY;          
    private float timer = 0f;         

    private void Start()
    {

        targetPosition = transform.localPosition;
        originalY = transform.localPosition.y;
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
        {
            transform.localPosition = targetPosition; 
        }

        if (isOpen)
        {
            timer += Time.deltaTime;

            if (timer >= timeToStill)
            {
                CloseMenu();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isOpen) return;

        Debug.Log("Clic sur l'élément UI");
        targetPosition = new Vector3(transform.localPosition.x, originalY - 156.81f, transform.localPosition.z);
        isOpen = true;
        timer = 0f;
    }

    private void CloseMenu()
    {
        targetPosition = new Vector3(transform.localPosition.x, originalY, transform.localPosition.z);
        isOpen = false;
        timer = 0f; 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isOpen) return;

        Debug.Log("La souris a quitté l'élément UI");
        CloseMenu();
    }
}
