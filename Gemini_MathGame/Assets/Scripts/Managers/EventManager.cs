using System.Collections.Generic;
using UnityEngine.Events;

public static class EventManager
{
    private static Dictionary<string, UnityEvent<Dictionary<string, object>>> _events = new();

    public static void RaiseEvent(string eventName, Dictionary<string, object> eventData = default)
    {
        if (_events.ContainsKey(eventName))
        {
            _events[eventName]?.Invoke(eventData);
        }
    }

    public static void AddListener(string eventName, UnityAction<Dictionary<string, object>> listener)
    {
        if (!_events.ContainsKey(eventName))
        {
            _events.Add(eventName, new UnityEvent<Dictionary<string, object>>());
        }
        _events[eventName].AddListener(listener);
    }

    public static void RemoveListener(string eventName, UnityAction<Dictionary<string, object>> listener)
    {
        if (_events.ContainsKey(eventName))
        {
            _events[eventName].RemoveListener(listener);
        }
    }
}
