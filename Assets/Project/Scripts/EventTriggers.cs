using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public static class EventTriggerUtility
{
    public static void AddEventTrigger(GameObject target, EventTriggerType triggerType, UnityAction action)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>() ?? target.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = triggerType };
        entry.callback.AddListener((eventData) => action()); // Ex�cute l'action quand l'�v�nement est d�clench�
        trigger.triggers.Add(entry);
    }
}
