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
    [SerializeField] private TMP_InputField _inputField;

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
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(GameEvents.BubbleClicked, BubbleClicked);
        EventManager.RemoveListener(GameEvents.SolutionEvaluated, SolutionEvaluated);
        EventManager.RemoveListener(GameEvents.RoundStarted, RoundStart);
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
    
    private void SolutionEvaluated(Dictionary<string, object> context)
    {
        // if correct, increment score
        var target = int.Parse(_inputField.text);
        var solution = (int)context["solution"];

        if (solution == target)
        {
            Debug.Log($"Good Job!");
            _scoreController.IncrementScore(5);
        }
        else
        {
            Debug.Log("Failed");
        }
    }
    
    private void RoundOver(Dictionary<string, object> context)
    {
        
    }
    
    private void GameOver(Dictionary<string, object> context)
    {
        
    }
}
