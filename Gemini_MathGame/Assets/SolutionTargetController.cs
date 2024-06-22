using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SolutionTargetController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _solutionTargetText;

    private EquationStringState _equationStringState;
    private string _solutionTarget;
    private string _result;
    private string _firstSlot;
    private string _secondSlot;

    private void Start()
    {
        // TODO: sub to events for bubble clicked, solution defined, solution evaluated
        EventManager.AddListener(GameEvents.SolutionEvaluated, OnSolutionEvaluated);
        EventManager.AddListener(GameEvents.BubbleClicked, OnNumberBubbleClicked);
        EventManager.AddListener(GameEvents.SolutionDefined, OnSolutionDefined);
    }
    
    private void OnDestroy()
    {
        EventManager.RemoveListener(GameEvents.SolutionEvaluated, OnSolutionEvaluated);
        EventManager.RemoveListener(GameEvents.BubbleClicked, OnNumberBubbleClicked);
        EventManager.RemoveListener(GameEvents.SolutionDefined, OnSolutionDefined);
    }

    private void OnSolutionDefined(Dictionary<string, object> arg0)
    {
        if (arg0.TryGetAs("solution", out string solution))
        {
            _solutionTarget = solution;
            _equationStringState = EquationStringState.EmptySolutionUpdate;
            EquationStringBuilder(0);
        }
        else
        {
            Debug.LogError($"Could not find value for key: 'solution' in data: {arg0.ToJsonPretty()}");
        }
    }

    private void OnNumberBubbleClicked(Dictionary<string, object> arg0)
    {
        if (arg0.TryGetAs("data", out NumberBubble data))
        {
            EquationStringBuilder(data.Value);
        }
        else
        {
            Debug.LogError($"Could not find value for key: 'data' in data: {arg0.ToJsonPretty()}");
        }
    }

    private void OnSolutionEvaluated(Dictionary<string, object> arg0)
    {
        _equationStringState = EquationStringState.TwoSlotsFilled;
        EquationStringBuilder(0);
    }

    private void EquationStringBuilder(int input)
    {
        switch (_equationStringState)
        {
            case EquationStringState.Empty:
                _firstSlot = input.ToString();
                _solutionTargetText.text = $"<u>{input}</u> + ___ = {_solutionTarget}";
                _equationStringState = EquationStringState.OneSlotFilled;
                break;
            case EquationStringState.OneSlotFilled:
                _secondSlot = input.ToString();
                _solutionTargetText.text = $"<u>{_firstSlot}</u> + <u>{input}</u> = {_solutionTarget}";
                _equationStringState = EquationStringState.TwoSlotsFilled;
                break;
            case EquationStringState.TwoSlotsFilled:
                _firstSlot = "";
                _secondSlot = "";
                _solutionTargetText.text = $"___ + ___ = {_solutionTarget}";
                _equationStringState = EquationStringState.Empty;
                break;
            case EquationStringState.EmptySolutionUpdate:
                _firstSlot = "";
                _secondSlot = "";
                _solutionTargetText.text = $"___ + ___ = {_solutionTarget}";
                _equationStringState = EquationStringState.Empty;
                break;
            default:
                _solutionTargetText.text = $"___ + ___ = {_solutionTarget}";
                _equationStringState = EquationStringState.Empty;
                break;
        }
    }
}

public enum EquationStringState
{
    Empty,
    EmptySolutionUpdate,
    OneSlotFilled,
    TwoSlotsFilled
}
