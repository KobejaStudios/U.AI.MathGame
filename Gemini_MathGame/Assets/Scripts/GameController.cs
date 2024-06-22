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
    [SerializeField] private ScoreController _scoreController;
    [SerializeField] private SolutionTargetController _solutionTargetController;
    [SerializeField] private TMP_InputField _inputField;
    
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
        EventManager.AddListener(GameEvents.SolutionEvaluated, SolutionEvaluated);
        EventManager.AddListener(GameEvents.RoundStarted, RoundStart);
        EventManager.AddListener(GameEvents.ScoreAdded, OnScoreAdded);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(GameEvents.BubbleClicked, BubbleClicked);
        EventManager.RemoveListener(GameEvents.SolutionEvaluated, SolutionEvaluated);
        EventManager.RemoveListener(GameEvents.RoundStarted, RoundStart);
        EventManager.RemoveListener(GameEvents.ScoreAdded, OnScoreAdded);
    }

    public void SetSolution(int value)
    {
        SolutionTarget = value;
        EventManager.RaiseEvent(GameEvents.SolutionDefined,
            new Dictionary<string, object> 
            {
                ["solution"] = value
            }
        );
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
    
    private void OnScoreAdded(Dictionary<string, object> arg0)
    {
        if (arg0.TryGetAs("currentScore", out int currentScore) &&
            arg0.TryGetAs("pairsGoal", out int pairsGoal))
        {
            if (currentScore == pairsGoal)
            {
                Debug.Log("Winner!");
            }
        }
    }
    
    private void SolutionEvaluated(Dictionary<string, object> context)
    {
        // if correct, increment score
        var target = SolutionTarget;
        var solution = (int)context["solution"];
        var bubbles = context["bubbles"] as Queue<NumberBubble>;

        if (solution == target)
        {
            EventManager.RaiseEvent(GameEvents.CorrectSolution, context);
            Debug.Log($"Good Job!");
            // TODO: move this logic elsewhere? perhaps a bubbles manager?
            while (bubbles.Count > 0)
            {
                var current = bubbles.Dequeue();
                current.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Failed");
        }
    }

    public Dictionary<int, NumberBubble> GetBubblesMap()
    {
        return _bubblesController.bubblesMap;
    }
    
    private void RoundOver(Dictionary<string, object> context)
    {
        
    }
    
    private void GameOver(Dictionary<string, object> context)
    {
        
    }
}
