using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatButton : MonoBehaviour
{
    [SerializeField]
    private ChatManager chatManager;

    [SerializeField]
    private GameObject ChatNotifyPoint;

    [Header ("Notification Center")]
    [SerializeField]
    private GameObject NotificationCenterObject;
    [SerializeField]
    private float speed = 2.0f;
    [SerializeField]
    private float timeToStill = 15.0f;


    private Vector3 targetPosition;
    private bool isOpen = false;
    private float timer = 0f;

    void Start()
    {

        targetPosition = NotificationCenterObject.transform.localPosition;
    }

    void Update()
    {
        chatManager = FindAnyObjectByType<ChatManager>();
        chatManager.OnMessage += NotifyFunction;
        gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            chatManager.OpenChatTab();
            ChatNotifyPoint.SetActive(false);
        }
        );

        NotificationCenterObject.transform.localPosition = Vector3.Lerp(NotificationCenterObject.transform.localPosition, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(NotificationCenterObject.transform.localPosition, targetPosition) < 0.01f)
        {
            NotificationCenterObject.transform.localPosition = targetPosition;
        }

        if (isOpen)
        {
            timer += Time.deltaTime;

            if (timer >= timeToStill)
            {
                CloseNitificationCenter();
            }
        }

    }

    private void NotifyFunction(string message)
    {
        ChatNotifyPoint.SetActive(true);

        OpenNotificationCenter(message);
    }

    private void CloseNitificationCenter()
    {
        targetPosition = new Vector3(NotificationCenterObject.transform.localPosition.x + 846 , NotificationCenterObject.transform.localPosition.y, NotificationCenterObject.transform.localPosition.z);
    }

    private void OpenNotificationCenter(string message)
    {
        targetPosition = new Vector3(NotificationCenterObject.transform.localPosition.x - 846, NotificationCenterObject.transform.localPosition.y, NotificationCenterObject.transform.localPosition.z);
        timer = 0f;
        isOpen = true;
        NotificationCenterObject.GetComponentInChildren<TMP_Text>().text = message;
    }
}
