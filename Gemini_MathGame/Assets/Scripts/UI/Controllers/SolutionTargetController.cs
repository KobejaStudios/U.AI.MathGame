using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SolutionTargetController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _solutionTargetText;
    [SerializeField] private NumberBubbleDisplay _leftBubble;
    [SerializeField] private NumberBubbleDisplay _rightBubble;
    [SerializeField] private RoundOverBannerController _roundOverBanner;

    [Header("Right/Wrong Icon")]
    [SerializeField] private Image _rightWrongIcon;
    [SerializeField] private Sprite _checkmarkSprite;
    [SerializeField] private Sprite _xSprite;

    private EquationStringState _equationStringState;
    private string _solutionTarget;
    private string _result;
    private string _firstSlot;
    private string _secondSlot;

    private CancellationTokenSource _cts = new();
    private CancellationTokenSource _bannerCts = new();
    private void Start()
    {
        EventManager.AddListener(GameEvents.BubbleClicked, OnNumberBubbleClicked);
        EventManager.AddListener(GameEvents.IntNumberGenerationComplete, OnNumbersGenerated);
        EventManager.AddListener(GameEvents.RoundWon, OnRoundWon);
        EventManager.AddListener(GameEvents.RoundLost, OnRoundLost);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(GameEvents.BubbleClicked, OnNumberBubbleClicked);
        EventManager.RemoveListener(GameEvents.IntNumberGenerationComplete, OnNumbersGenerated);
        EventManager.RemoveListener(GameEvents.RoundWon, OnRoundWon);
        EventManager.RemoveListener(GameEvents.RoundLost, OnRoundLost);
    }

    public void ResetController()
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        _bannerCts = new CancellationTokenSource();
        _roundOverBanner.ResetBanner();
        _solutionTargetText.text = "";
        _equationStringState = EquationStringState.Empty;
        _firstSlot = "";
        _secondSlot = "";
    }
    
    private async void OnRoundLost(Dictionary<string, object> arg0)
    {
        _roundOverBanner.UpdateText("Round Lost!");
        await _roundOverBanner.TransitionBannerIn(.7f, _bannerCts.Token);
    }

    private async void OnRoundWon(Dictionary<string, object> arg0)
    {
        // TODO: remove this small hack, this is just for stubbed purposes
        // we will need to improve how we handle win/loss conditions across the board
        _roundOverBanner.UpdateText("Round Won!");
        await _roundOverBanner.TransitionBannerIn(.7f, _bannerCts.Token);
    }

    private void OnNumbersGenerated(Dictionary<string, object> arg0)
    {
        if (arg0.TryGetAs(GameParams.data, out GeneratedNumbersData<int> data))
        {
            _solutionTarget = data.SolutionTarget.ToString();
            ResetEquationStringAndState();
        }
        else
        {
            Debug.LogError($"Could not find value for key: {GameParams.data} in data: {arg0.ToJsonPretty()}");
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

    private async void EquationStringBuilder(int input)
    {
        switch (_equationStringState)
        {
            case EquationStringState.Empty:
                _firstSlot = input.ToString();
                _leftBubble.UpdateText(input);
                _equationStringState = EquationStringState.OneSlotFilled;
                break;
            case EquationStringState.OneSlotFilled:
                _secondSlot = input.ToString();
                _rightBubble.UpdateText(input);
                _solutionTargetText.text = $"= {int.Parse(_firstSlot) + int.Parse(_secondSlot)}";
                _equationStringState = EquationStringState.TwoSlotsFilled;
                await AnimationStub(_cts.Token);
                break;
            case EquationStringState.TwoSlotsFilled:
                _cts.Cancel();
                _cts = new CancellationTokenSource();
                _firstSlot = input.ToString();
                _leftBubble.UpdateText(input);
                _secondSlot = "";
                _rightBubble.UpdateText("");
                _solutionTargetText.text = $"= {_solutionTarget}";
                _equationStringState = EquationStringState.OneSlotFilled;
                break;
            default:
                ResetEquationStringAndState();
                break;
        }
    }

    private async UniTask AnimationStub(CancellationToken ct)
    {
        try
        {
            await UniTask.Delay(1000, cancellationToken: ct);
            ResetEquationStringAndState();
            Debug.Log($"Animation awaiter was not canceled");
        }
        catch (OperationCanceledException e)
        {
            Debug.LogWarning($"Animation awaiter was canceled: {e.Message}. Not throwing an exception here");
            throw;
        }
    }

    private void ResetEquationStringAndState()
    {
        _firstSlot = "";
        _secondSlot = "";
        _solutionTargetText.text = $"= {_solutionTarget}";
        _leftBubble.UpdateText(_firstSlot);
        _rightBubble.UpdateText(_secondSlot);
        _equationStringState = EquationStringState.Empty;
    }
}

public enum EquationStringState
{
    Empty,
    OneSlotFilled,
    TwoSlotsFilled
}
