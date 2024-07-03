using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    [SerializeField] private BubblesController _bubblesController;
    [SerializeField] private SolutionTargetController _solutionTargetController;
    [SerializeField] private HourglassController _hourglassController;
    [SerializeField] private ScoreProgressController _scoreProgressController;

    private HashSet<int> _solutionNumbers = new();
    private List<int> _totalNumbers = new();
    private List<int> _remainderNumbers = new();
    private Queue<GameConfig> _gameConfigs = new();
    
    public int SolutionTarget { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        EventManager.AddListener(GameEvents.BubbleClicked, BubbleClicked);
        EventManager.AddListener(GameEvents.SolutionEvaluated, OnSolutionEvaluated);
        EventManager.AddListener(GameEvents.RoundStarted, RoundStart);
        EventManager.AddListener(GameEvents.ScoreAdded, OnScoreAdded);
        EventManager.AddListener(GameEvents.TimeExpired, OnTimeExpired);
        EventManager.AddListener(GameEvents.IntNumberGenerationComplete, OnNumberGenerationComplete);
        EventManager.AddListener(GameEvents.GameConfigDefined, OnGameConfigDefined);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(GameEvents.BubbleClicked, BubbleClicked);
        EventManager.RemoveListener(GameEvents.SolutionEvaluated, OnSolutionEvaluated);
        EventManager.RemoveListener(GameEvents.RoundStarted, RoundStart);
        EventManager.RemoveListener(GameEvents.ScoreAdded, OnScoreAdded);
        EventManager.RemoveListener(GameEvents.TimeExpired, OnTimeExpired);
        EventManager.RemoveListener(GameEvents.IntNumberGenerationComplete, OnNumberGenerationComplete);
        EventManager.RemoveListener(GameEvents.GameConfigDefined, OnGameConfigDefined);
    }
    
    private void OnGameConfigDefined(Dictionary<string, object> arg0)
    {
        if (arg0.TryGetAs(GameParams.gameConfig, out GameConfig gameConfig))
        {
            _gameConfigs.Enqueue(gameConfig);
            Debug.Log($"config: {gameConfig.ToJsonPretty()} added to queue. Queue length: {_gameConfigs.Count}");
        }
    }
    
    private void OnNumberGenerationComplete(Dictionary<string, object> arg0)
    {
        if (arg0.TryGetAs(GameParams.data, out GeneratedNumbersData<int> data))
        {
            _solutionNumbers = data.CorrectNumbers;
            SolutionTarget = data.SolutionTarget;
            _totalNumbers = data.TotalNumbers;
            _remainderNumbers = data.IncorrectNumbers;
        }
        
        EventManager.RaiseEvent(GameEvents.SolutionDefined, new Dictionary<string, object>
        {
            [GameParams.solution] = SolutionTarget
        });
    }

    public async void StartRound()
    {
        _hourglassController.ResetController();
        _scoreProgressController.ResetController();
        _solutionTargetController.ResetController();
        await _bubblesController.ResetBubblesViewAsync();
        
        var gameConfig = 
            _gameConfigs.Count <= 0 ? GetRandomConfig() : _gameConfigs.Dequeue();
        
        var numbersData = 
            await ServiceLocator.Get<INumberGeneratorService>().Async_GetAdditionNumbersInt(gameConfig);

        _bubblesController.SpawnBubblesInt(numbersData, () =>
        {
            Debug.Log($"done awaiting");
            EventManager.RaiseEvent(GameEvents.RoundStarted);
        });
    }

    private void RoundStart(Dictionary<string, object> context)
    {
        // spawn bubbles
        // init UI
    }
    
    private void BubbleClicked(Dictionary<string, object> context)
    {
        // store data
        // if solution reached, fire "SolutionEvaluated"
    }
    
    private void OnTimeExpired(Dictionary<string, object> arg0)
    {
        // TODO: validation? thoughts about strengthening these win/loss conditions
        // TODO: clean up this approach, was done quick and dirty for feedback purposes
        var temp = new HashSet<int>(_solutionNumbers);
        while (temp.Count > 0)
        {
            var current = temp.Last();
            var compliment = SolutionTarget - current;
            var currentBubble = _bubblesController.BubblesMap[current];
            var complimentBubble = _bubblesController.BubblesMap[compliment];

            if (temp.Contains(compliment))
            {
                temp.Remove(current);
                temp.Remove(compliment);
            }
            
            if (currentBubble.gameObject.activeInHierarchy && complimentBubble.gameObject.activeInHierarchy)
            {
                var color = RandomColorSelector.Instance.GetColor();
                currentBubble.Image.color = color;
                complimentBubble.Image.color = color;
            }
        }
        
        foreach (var n in _remainderNumbers)
        {
            _bubblesController.BubblesMap[n].SetBubbleAlpha(.1f);
        }
        
        EventManager.RaiseEvent(GameEvents.RoundLost);
    }
    
    private void OnScoreAdded(Dictionary<string, object> arg0)
    {
        if (arg0.TryGetAs(GameParams.currentScore, out int currentScore) &&
            arg0.TryGetAs(GameParams.pairsGoal, out int pairsGoal))
        {
            if (currentScore == pairsGoal)
            {
                Debug.Log("Winner!");
                EventManager.RaiseEvent(GameEvents.ScoreGoalReached);
                EventManager.RaiseEvent(GameEvents.RoundWon);
            }
        }
    }
    
    private void OnSolutionEvaluated(Dictionary<string, object> context)
    {
        var target = SolutionTarget;
        var solution = (int)context[GameParams.solution];

        if (context.TryGetAs(GameParams.bubbles, out Queue<NumberBubble> bubbles))
        {
            foreach (var bubble in bubbles)
            {
                bubble.ClearBubbleState();
            }

            if (solution == target)
            {
                EventManager.RaiseEvent(GameEvents.CorrectSolution, context);

                // TODO: move this logic elsewhere? perhaps a bubbles manager?
                while (bubbles.Count > 0)
                {
                    var current = bubbles.Dequeue();
                    current.gameObject.SetActive(false);
                }
            }
        }
    }
    
    private void RoundOver(Dictionary<string, object> context)
    {
        
    }
    
    private void GameOver(Dictionary<string, object> context)
    {
        
    }

    #region Helpers

    private GameConfig GetRandomConfig()
    {
        return new GameConfig
        (
            Random.Range(200, 500),
            32,
            16,
            EquationOperation.Addition,
            BubbleCollectionOrientation.Shuffled,
            false
            );
    }

    #endregion
}
