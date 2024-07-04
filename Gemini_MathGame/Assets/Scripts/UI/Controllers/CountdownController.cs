using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour
{
    [SerializeField] private Image _overlay;
    [SerializeField] private CanvasGroup _overlayCanvasGroup;
    [SerializeField] private TextMeshProUGUI _countdownText;

    public async UniTask Async_StartCountdown(Queue<string> countdownSteps, int delayStepMs)
    {
        FadeInOverlay(.2f);
        await UniTask.RunOnThreadPool(async () =>
        {
            while (countdownSteps.Count > 0)
            {
                if (countdownSteps.Count == 1)
                {
                    delayStepMs /= 2;
                }
                
                var current = countdownSteps.Dequeue();
                
                await UniTask.SwitchToMainThread();
                _countdownText.text = current;
                
                await UniTask.SwitchToThreadPool();
                await UniTask.Delay(delayStepMs);
            }
        });
        FadeOutOverlay(.2f);
    }

    private void FadeInOverlay(float duration)
    {
        _overlayCanvasGroup.alpha = 0f;
        _overlay.gameObject.SetActive(true);

        _countdownText.text = "";
        _countdownText.gameObject.SetActive(true);
        
        LMotion
            .Create(_overlayCanvasGroup.alpha, 1f, duration)
            .BindToCanvasGroupAlpha(_overlayCanvasGroup);
    }
    
    private void FadeOutOverlay(float duration)
    {
        LMotion
            .Create(_overlayCanvasGroup.alpha, 1f, duration)
            .WithOnComplete(() =>
            {
                _overlay.gameObject.SetActive(false);
                _countdownText.gameObject.SetActive(false);
            })
            .BindToCanvasGroupAlpha(_overlayCanvasGroup);
    }
}
