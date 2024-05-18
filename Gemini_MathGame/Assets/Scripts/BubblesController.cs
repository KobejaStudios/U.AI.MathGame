using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class BubblesController : MonoBehaviour
{
    [SerializeField] private NumberBubble[] _bubbles;
    public Dictionary<int, NumberBubble> bubblesMap = new();
    private Queue<NumberBubble> _bubblesQueue = new();
    [SerializeField] private GeminiRequestHandler _geminiRequestHandler;

    private void Awake()
    {
        
    }

    private void Start()
    {
        EventManager.AddListener(GameEvents.BubbleClicked, OnBubbleClicked);
        EventManager.AddListener(GameEvents.EvaluateSolution, OnEvaluateSolution);
        
        Init();
    }

    private void OnEvaluateSolution(Dictionary<string, object> arg0)
    {
        var solution = 0;
        var temp = new Queue<NumberBubble>(_bubblesQueue);
        while (_bubblesQueue.Count > 0)
        {
            var current = _bubblesQueue.Dequeue();
            solution += current.Value;
            Debug.Log($"value: {current.Value} added to sum giving: {solution}");
        }
        
        var eventData = new Dictionary<string, object>
        {
            ["solution"] = solution,
            ["bubbles"] = temp
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

    private async void Init()
    {
        Debug.Log("Sending request from bubble controller");
        var content = await _geminiRequestHandler.AsyncGeminiRequest(PromptBuilder(156));
        Debug.Log($"Received request from bubble controller: {content}");

        var data = ParseResponse(content);
        var solution = "";
        JToken nums = null;

        foreach (var VARIABLE in data)
        {
            solution = VARIABLE.Key;
            nums = VARIABLE.Value;
        }

        GameController.Instance.SetSolution(int.Parse(solution));
        
        var count = 0;
        if (nums != null)
        {
            foreach (var n in nums)
            {
                _bubbles[count].gameObject.SetActive(true);
                _bubbles[count].UpdateValue((int)n);
                bubblesMap[_bubbles[count].Id] = _bubbles[count];
                count++;
            }
        }
        else
        {
            Debug.LogError($"nums is null!!");
        }
    }

    private string PromptBuilder(int target)
    {
        return
            $"can you generate a json object named {target} with a value of 35 distinct random numbers. The numbers should be within a range of 0 - {target}. These will be used for a game where the user must select 2 numbers that sum to {target}";
    }

    private JObject ParseResponse(string content)
    {
        content = content.Replace("`", "").Replace("json", "");
        Debug.Log($"cleaned json: {content}");
        var data = JObject.Parse(content);
        return data;
    }

    private void ProcessResponse(string response)
    {
        
    }
    
    /*
     * "128": {64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 34,26,17
}
     */
    
    [System.Serializable]
    public class CandidateResponse
    {
        public int[] values;
    }
}
