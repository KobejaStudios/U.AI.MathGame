using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = System.Random;

public class BubblesController : MonoBehaviour
{
    [SerializeField] private NumberBubble[] _bubbles;

    public Dictionary<int, NumberBubble> BubblesMap { get; } = new();
    private Queue<NumberBubble> _bubblesQueue = new();
    private HashSet<int> _solutionNumbers = new();

    #region UnityLifecycle

    private void Start()
    {
        EventManager.AddListener(GameEvents.BubbleClicked, OnBubbleClicked);
        EventManager.AddListener(GameEvents.EvaluateSolution, OnEvaluateSolution);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(GameEvents.BubbleClicked, OnBubbleClicked);
        EventManager.RemoveListener(GameEvents.EvaluateSolution, OnEvaluateSolution);
    }

    #endregion
    
    #region Internal
    
    private void OnEvaluateSolution(Dictionary<string, object> arg0)
    {
        var solution = 0;
        var temp = new Queue<NumberBubble>(_bubblesQueue);
        while (_bubblesQueue.Count > 0)
        {
            var current = _bubblesQueue.Dequeue();
            current.ResetBubbleColor();
            solution += current.Value;
        }
        
        var eventData = new Dictionary<string, object>
        {
            [GameParams.solution] = solution,
            [GameParams.bubbles] = temp
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

    private void ResetBubblesView()
    {
        foreach (var bubble in _bubbles)
        {
            bubble.ResetBubble();
        }
    }

    public void SpawnBubblesInt(GeneratedNumbersData<int> data, Action onComplete = default)
    {
        var count = 0;
        _solutionNumbers = data.CorrectNumbers;
        EventManager.RaiseEvent(GameEvents.PairsGoalDefined, new Dictionary<string, object>
        {
            ["pairsGoal"] = _solutionNumbers.Count / 2
        });
        Debug.Log($"Correct pairs: {_solutionNumbers.Count / 2}\ndata: {_solutionNumbers.ToJsonPretty()}");
        
        // TODO: refactor for duplicate numbers allowed
        
        foreach (var n in data.TotalNumbers)
        {
            _bubbles[count].gameObject.SetActive(true);
            _bubbles[count].UpdateValue(n);
            BubblesMap[_bubbles[count].Value] = _bubbles[count];
            count++;
        }
        onComplete?.Invoke();
    }
    
    private void SpawnBubbles(List<float> numbers)
    {
        
    }

    #endregion

    public async UniTask ResetBubblesViewAsync()
    {
        ResetBubblesView();
        await UniTask.Yield();
    }
}
