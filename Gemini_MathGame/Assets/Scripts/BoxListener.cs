using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxListener : MonoBehaviour
{
    private void Start()
    {
        EventManager.AddListener(GameEvents.BubbleClicked, OnBoxClicked);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(GameEvents.BubbleClicked, OnBoxClicked);
    }

    private void OnBoxClicked(Dictionary<string, object> arg0)
    {
        arg0.TryGetValue("data", out var data);
        var d = data as NumberBubble;
    }
}
