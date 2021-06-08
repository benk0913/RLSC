using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public List<EventListenInstance> Events = new List<EventListenInstance>();

    private void OnEnable()
    {
        foreach (EventListenInstance instance in Events)
        {
            CORE.Instance.SubscribeToEvent(instance.EventKey, instance.Event.Invoke);
        }
    }

    private void OnDisable()
    {
        foreach (EventListenInstance instance in Events)
        {
            CORE.Instance.UnsubscribeFromEvent(instance.EventKey, instance.Event.Invoke);
        }
    }
}

[System.Serializable]
public class EventListenInstance
{
    public string EventKey;
    public UnityEvent Event;
}
