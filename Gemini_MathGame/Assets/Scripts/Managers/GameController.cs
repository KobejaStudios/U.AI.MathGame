using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    [SerializeField] private BubblesController _bubblesController;
    [SerializeField] private SolutionTargetController _solutionTargetController;
    [SerializeField] private HourglassController _hourglassController;
    [SerializeField] private ScoreProgressController _scoreProgressController;

    private HashSet<int> _solutionNumbers = new();
    
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
        EventManager.AddListener(GameEvents.NumberGenerationComplete, OnNumberGenerationComplete);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(GameEvents.BubbleClicked, BubbleClicked);
        EventManager.RemoveListener(GameEvents.SolutionEvaluated, OnSolutionEvaluated);
        EventManager.RemoveListener(GameEvents.RoundStarted, RoundStart);
        EventManager.RemoveListener(GameEvents.ScoreAdded, OnScoreAdded);
        EventManager.RemoveListener(GameEvents.TimeExpired, OnTimeExpired);
        EventManager.RemoveListener(GameEvents.NumberGenerationComplete, OnNumberGenerationComplete);
    }
    
    private void OnNumberGenerationComplete(Dictionary<string, object> arg0)
    {
        if (arg0.TryGetAs(GameParams.data, out GeneratedNumbersData<int> data))
        {
            _solutionNumbers = data.CorrectNumbers;
            SolutionTarget = data.SolutionTarget;
        }
    }

    public async void StartRound()
    {
        _hourglassController.ResetController();
        _scoreProgressController.ResetController();
        _solutionTargetController.ResetController();
        await _bubblesController.ResetBubblesViewAsync();
        
        // TODO: build this data from the menus
        var gameData = new GameConfig(
            200,
            40,
            20,
            EquationOperation.Addition,
            BubbleCollectionOrientation.Shuffled,
            false
            );
        
        var numbersData = 
            await ServiceLocator.Get<INumberGeneratorService>().Async_GetNumbersInt(gameData);
        
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
        foreach (var bubble in _solutionNumbers)
        {
            var current = _bubblesController.BubblesMap[bubble];
            if (current.gameObject.activeInHierarchy)
            {
                current.Image.color = Color.green;
            }
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
}
