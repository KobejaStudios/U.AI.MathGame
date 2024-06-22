using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = System.Random;

public class BubblesController : MonoBehaviour
{
    [SerializeField] private NumberBubble[] _bubbles;
    public Dictionary<int, NumberBubble> bubblesMap = new();
    private Queue<NumberBubble> _bubblesQueue = new();
    [SerializeField] private GeminiRequestHandler _geminiRequestHandler;

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
            current.Image.color = Color.white;
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
        var content = await _geminiRequestHandler.AsyncGeminiRequest(PromptBuilder(314));
        Debug.Log($"Received request from bubble controller");

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
            var list = ShuffleValues(ParseJToken(nums));
            var enumerable = list as int[] ?? list.ToArray();
            var pairs = GetSolutionPairs(enumerable, int.Parse(solution));
            Debug.Log($"Correct pairs: {pairs.Count / 2}\ndata: {pairs.ToJsonPretty()}");
            
            foreach (var n in enumerable)
            {
                _bubbles[count].gameObject.SetActive(true);
                _bubbles[count].UpdateValue(n);
                bubblesMap[_bubbles[count].Value] = _bubbles[count];
                count++;
            }
        }
        else
        {
            Debug.LogError($"nums is null!!");
        }
    }

    public async void TestApiRequest()
    {
        Debug.Log("Sending request from bubble controller");
        var content = await _geminiRequestHandler.AsyncGeminiRequest(PromptBuilder(314));
        Debug.Log($"Received request from bubble controller");

        var data = ParseResponse(content);
        var solution = "";
        JToken nums = null;

        foreach (var VARIABLE in data)
        {
            solution = VARIABLE.Key;
            nums = VARIABLE.Value;
        }
        
        if (nums != null)
        {
            var list = ShuffleValues(ParseJToken(nums));
            var enumerable = list as int[] ?? list.ToArray();
            var pairs = GetSolutionPairs(enumerable, int.Parse(solution));
            Debug.Log($"Correct pairs: {pairs.Count / 2}\ndata: {pairs.ToJsonPretty()}");
        }
        else
        {
            Debug.LogError($"nums is null!!");
        }
    }

    private HashSet<int> GetSolutionPairs(IEnumerable<int> input, int solutionTarget)
    {
        var result = new HashSet<int>();
        var data = input.ToHashSet();
        while (data.Count > 0)
        {
            var current = data.Last();
            data.Remove(current);
            var target = solutionTarget - current;
            if (data.Contains(target))
            {
                data.Remove(target);
                result.Add(current);
                result.Add(target);
            }
        }

        return result;
    }

    private IEnumerable<int> ParseJToken(JToken value)
    {
        var list = new List<int>();

        foreach (var n in value)
        {
            list.Add((int)n);
        }

        return list;
    }

    private IEnumerable<int> ShuffleValues(IEnumerable<int> values)
    {
        var oldList = values.ToHashSet().ToList();
        var newList = new List<int>();

        while (oldList.Count > 0)
        {
            var random = new Random();
            var index = random.Next(0, oldList.Count - 1);
            var current = oldList[index];
            newList.Add(current);
            oldList.RemoveAt(index);
        }

        return newList;
    }

    private string PromptBuilder(int target, int pairs = 21)
    {
        var value1 = $"Generate a JSON object named '{target}' with a value that's an array of {pairs * 2} numbers between 0 and {target} where {PercentBuilder(pairs)} distinct number pairs within the array sum up to {target} and the remaining numbers are random within the range of 0 - {target}" +
                    $" I only want the JSON object";
        // var value2 = $"Generate a JSON object named '{target}' with a value that's an array of {pairs * 2} numbers between 0 and {target}. Within this array of numbers I want {PercentBuilder(pairs)} distinct number pairs that sum up to {target} and the remaining numbers are random within the range of 0 - {target}" +
        //            $" I only want the JSON object";
        Debug.Log($"Prompt: {value1}");
        return value1;

    }
    
    private string ComplexPromptBuilder(int target, int pairs = 21)
    {
        return
            $"Generate a JSON object named '{target}' with a value that's an array of {pairs * 2} random numbers between 0 and {target}.  Include some pairs within the array that sum up to {target}. The number of pairs should be between {PercentBuilder(pairs)} and {PercentBuilder(pairs)}." +
            $"\n Additionally, generate a secondary JSON object named \"config\" with the following structure:\n\n```json\n{{\n  \"config\": {{\n    \"numValues\": 36,\n    \"numPairs\": 12, // This will be the number between {PercentBuilder(pairs)} and {PercentBuilder(pairs)} that you used for determining the number of correct pairs\n    \"correctPairs\": [\n      // List of pairs that sum up to 117\n    ]\n  }}\n}}";
    }

    private int PercentBuilder(int pairs)
    {
        var input = UnityEngine.Random.Range(0.5f, 0.8f);
        var rand = UnityEngine.Random.Range(input, Math.Min(input * 1.5f, 0.9f));
        var value = Mathf.FloorToInt(rand * pairs);
        Debug.Log($"correct pairs target: {value}, total pairs: {pairs}");
        return value;
    }

    private JObject ParseResponse(string content)
    {
        content = content.Replace("`", "").Replace("json", "");
        Debug.Log($"cleaned json: {content}");
        var data = JObject.Parse(content);
        return data;
    }
}
