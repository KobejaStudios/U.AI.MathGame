using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BubblesController : MonoBehaviour
{
    [SerializeField] private NumberBubble[] _bubbles;
    public Dictionary<int, NumberBubble> bubblesMap = new();
    private Queue<NumberBubble> _bubblesQueue = new();

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        EventManager.AddListener(GameEvents.BubbleClicked, OnBubbleClicked);
        EventManager.AddListener(GameEvents.EvaluateSolution, OnEvaluateSolution);
    }

    private void OnEvaluateSolution(Dictionary<string, object> arg0)
    {
        var solution = 0;
        while (_bubblesQueue.Count > 0)
        {
            var current = _bubblesQueue.Dequeue();
            solution += current.Value;
            Debug.Log($"value: {current.Value} added to sum giving: {solution}");
        }
        
        var eventData = new Dictionary<string, object>
        {
            ["solution"] = solution,
        };
        EventManager.RaiseEvent(GameEvents.SolutionEvaluated, eventData);
    }

    private void OnBubbleClicked(Dictionary<string, object> arg0)
    {
        var bubble = arg0["data"] as NumberBubble;
        _bubblesQueue.Enqueue(bubble);

        if (_bubblesQueue.Count == 2)
        {
            EventManager.RaiseEvent(GameEvents.EvaluateSolution);
        }
    }

    private void Init()
    {
        for (int i = 0; i < _bubbles.Length; i++)
        {
            _bubbles[i].UpdateValue((int)(i * 3.1415f));
            bubblesMap[_bubbles[i].Id] = _bubbles[i];
        }
    }
}
