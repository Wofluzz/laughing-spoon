using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public static class EventTriggerUtility
{
    public static void AddEventTrigger(GameObject target, EventTriggerType triggerType, UnityAction action)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>() ?? target.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = triggerType };
        entry.callback.AddListener((eventData) => action()); // Exécute l'action quand l'événement est déclenché
        trigger.triggers.Add(entry);
    }
}
