using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class HourglassController : MonoBehaviour
{
    [SerializeField] private int _timerSeconds;
    [SerializeField] private TextMeshProUGUI _timerText;
    private TimeSpan _time;
    private bool _goalReached;
    private CancellationTokenSource _cts = new();
    private void Start()
    {
        EventManager.AddListener(GameEvents.RoundStarted, OnRoundStarted);
        EventManager.AddListener(GameEvents.ScoreGoalReached, OnScoreGoalReached);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(GameEvents.RoundStarted, OnRoundStarted);
        EventManager.RemoveListener(GameEvents.ScoreGoalReached, OnScoreGoalReached);
    }

    private void OnScoreGoalReached(Dictionary<string, object> arg0)
    {
        _cts?.Cancel();
        _goalReached = true;
    }

    private async void OnRoundStarted(Dictionary<string, object> arg0)
    {
        _time = TimeSpan.FromSeconds(_timerSeconds);
        _timerText.text = _time.ToString(@"mm\:ss");
        while (_time > TimeSpan.Zero || _goalReached)
        {
            await UniTask.Delay(1000, cancellationToken: _cts.Token);
            _time = _time.Subtract(TimeSpan.FromMilliseconds(1000));
            _timerText.text = _time.ToString(@"mm\:ss");
        }

        if (_time <= TimeSpan.Zero)
        {
            EventManager.RaiseEvent(GameEvents.TimeExpired);
        }
    }

    [ContextMenu("kill time")]
    public void ClearTime()
    {
        _time = _time.Subtract(TimeSpan.FromSeconds(_timerSeconds));
    }
}
