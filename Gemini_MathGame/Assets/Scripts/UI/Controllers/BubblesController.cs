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

    private void ResetBubblesView()
    {
        foreach (var bubble in _bubbles)
        {
            bubble.ResetBubble();
        }
    }
    
    private HashSet<int> SpawnBubbles(string content)
    {
        var data = ParseResponse(content);
        var solution = "";
        JToken nums = null;

        foreach (var kvp in data)
        {
            solution = kvp.Key;
            nums = kvp.Value;
        }

        GameController.Instance.SetSolution(int.Parse(solution));
        
        var count = 0;
        if (nums != null)
        {
            var list = ShuffleValues(ParseJToken(nums));
            var enumerable = list as int[] ?? list.ToArray();
            _solutionNumbers = GetSolutionNumbers(enumerable, int.Parse(solution));
            EventManager.RaiseEvent(GameEvents.PairsGoalDefined, new Dictionary<string, object>
            {
                ["pairsGoal"] = _solutionNumbers.Count / 2
            });
            Debug.Log($"Correct pairs: {_solutionNumbers.Count / 2}\ndata: {_solutionNumbers.ToJsonPretty()}");
            
            foreach (var n in enumerable)
            {
                _bubbles[count].gameObject.SetActive(true);
                _bubbles[count].UpdateValue(n);
                BubblesMap[_bubbles[count].Value] = _bubbles[count];
                count++;
            }
        }
        else
        {
            Debug.LogError($"nums is null!!");
        }

        return _solutionNumbers;
    }
    
    private HashSet<int> GetSolutionNumbers(IEnumerable<int> input, int solutionTarget)
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

    private JObject ParseResponse(string content)
    {
        content = content.Replace("`", "").Replace("json", "");
        Debug.Log($"cleaned json: {content}");
        var data = JObject.Parse(content);
        return data;
    }

    #endregion

    public async UniTask<HashSet<int>> SpawnBubblesAsync(string content)
    {
        return await UniTask.FromResult(SpawnBubbles(content));
    }

    public async UniTask ResetBubblesViewAsync()
    {
        ResetBubblesView();
        await UniTask.Yield();
    }

    public async void TestApiRequest()
    {
        Debug.Log("Sending request from bubble controller");
        var prompt = ServiceLocator.Get<IPromptBuilderService>().BuildPromptSimple(314);
        var content = await ServiceLocator.Get<IGeminiRequestService>().AsyncGeminiRequest(prompt);
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
            var pairs = GetSolutionNumbers(enumerable, int.Parse(solution));
            Debug.Log($"Correct pairs: {pairs.Count / 2}\ndata: {pairs.ToJsonPretty()}");
        }
        else
        {
            Debug.LogError($"nums is null!!");
        }
    }
}
