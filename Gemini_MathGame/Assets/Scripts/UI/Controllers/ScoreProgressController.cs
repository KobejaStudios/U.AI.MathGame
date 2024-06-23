using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreProgressController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _progressBarFill;
    
    private int _pairsGoal;
    private int _currentPairs;
    private float _progressSegment;

    private void Start()
    {
        EventManager.AddListener(GameEvents.CorrectSolution, OnCorrectSolution);
        EventManager.AddListener(GameEvents.PairsGoalDefined, OnPairsGoalDefined);
        InitUI();
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(GameEvents.CorrectSolution, OnCorrectSolution);
        EventManager.RemoveListener(GameEvents.PairsGoalDefined, OnPairsGoalDefined);
    }

    public void ResetController()
    {
        _text.text = "0 / 0";
        _progressBarFill.fillAmount = 0f;
        _pairsGoal = 0;
        _currentPairs = 0;
    }

    private void InitUI()
    {
        UpdateText();
        _progressBarFill.fillAmount = 0f;
    }

    private void UpdateText()
    {
        _text.text = $"{_currentPairs} / {_pairsGoal}";
    }
    
    private void OnPairsGoalDefined(Dictionary<string, object> arg0)
    {
        if (arg0.TryGetAs("pairsGoal", out int pairsGoal))
        {
            _pairsGoal = pairsGoal;
            _progressSegment = 1f / pairsGoal;
        }
        else
        {
            Debug.LogError($"could not find value for key 'pairsGoal' in data: {arg0.ToJsonPretty()}");
        }
        
        UpdateText();
    }

    private void OnCorrectSolution(Dictionary<string, object> arg0)
    {
        var prev = _currentPairs;
        _currentPairs++;
        var eventData = new Dictionary<string, object>
        {
            ["previousScore"] = prev,
            ["currentScore"] = _currentPairs,
            ["pairsGoal"] = _pairsGoal
        };
        
        EventManager.RaiseEvent(GameEvents.ScoreAdded, eventData);
        _progressBarFill.fillAmount += _progressSegment;
        UpdateText();
    }
}
