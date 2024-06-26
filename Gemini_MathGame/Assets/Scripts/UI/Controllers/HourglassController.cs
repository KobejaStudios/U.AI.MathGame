using System;
using System.Collections.Generic;
using System.Threading;
using AssetKits.ParticleImage;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HourglassController : MonoBehaviour
{
    [SerializeField] private int _timerSeconds;
    [SerializeField] private TextMeshProUGUI _timerText;

    [Header("Hourglass Components")] 
    [SerializeField] private Image _topHalfImage;
    [SerializeField] private Image _bottomHalfImage;
    [SerializeField] private ParticleImage _sandParticles;
    [SerializeField] private Color _lowTimeColor;
    private float _hourglassFillSegment;
    private CompositeMotionHandle _sandMoveMotionHandles = new();
    
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

    public void ResetController()
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        ResetHourglassFill();
        _time = TimeSpan.FromSeconds(_timerSeconds);
        _timerText.text = _time.ToString(@"mm\:ss");
        _timerText.color = Color.white;
        _goalReached = false;
    }

    private void OnScoreGoalReached(Dictionary<string, object> arg0)
    {
        _cts?.Cancel();
        _goalReached = true;
        StopSandAnimation();
    }

    private async void OnRoundStarted(Dictionary<string, object> arg0)
    {
        _time = TimeSpan.FromSeconds(_timerSeconds);
        _timerText.text = _time.ToString(@"mm\:ss");
        StartSandAnimation(_timerSeconds);
        while (_time > TimeSpan.Zero && !_goalReached)
        {
            await UniTask.Delay(1000, cancellationToken: _cts.Token);
            _time = _time.Subtract(TimeSpan.FromMilliseconds(1000));
            _timerText.text = _time.ToString(@"mm\:ss");
            if (_time.TotalSeconds <= 10)
            {
                _timerText.color = _lowTimeColor;
            }
        }

        if (_time <= TimeSpan.Zero)
        {
            EventManager.RaiseEvent(GameEvents.TimeExpired);
        }
    }

    private void ResetHourglassFill()
    {
        StopSandAnimation();
        _topHalfImage.fillAmount = 1f;
        _bottomHalfImage.fillAmount = 0f;
    }

    [ContextMenu("stop sand")]
    private void StopSandAnimation()
    {
        _sandMoveMotionHandles?.Cancel();
        _sandParticles.Stop();
    }

    private void StartSandAnimation(float duration)
    {
        var topCurrent = _topHalfImage.fillAmount;
        var bottomCurrent = _bottomHalfImage.fillAmount;
        
        _sandParticles.gameObject.SetActive(true);
        _sandParticles.Play();

        LMotion.Create(topCurrent, 0f, duration)
            .WithEase(Ease.Linear)
            .BindToFillAmount(_topHalfImage)
            .AddTo(_sandMoveMotionHandles);

        LMotion.Create(bottomCurrent, 1f, duration)
            .WithEase(Ease.Linear)
            .BindToFillAmount(_bottomHalfImage)
            .AddTo(_sandMoveMotionHandles);
    }

    [ContextMenu("kill time")]
    public void ClearTime()
    {
        _time = _time.Subtract(TimeSpan.FromSeconds(_timerSeconds));
    }
}
