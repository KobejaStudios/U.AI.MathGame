using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

public class RoundOverBannerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private Vector3 _startingPosition;
    private RectTransform _rectTransform;
    private float _rectHeight;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectHeight = _rectTransform.rect.height;
        _startingPosition = _rectTransform.position;
    }

    public async UniTask TransitionBannerIn(float duration, CancellationToken ct)
    {
        var position = _rectTransform.position;
        var target = position.y - _rectHeight;
        await LMotion.Create(position.y, target, duration)
            .BindToPositionY(_rectTransform)
            .ToValueTask(ct);
    }
    
    public async UniTask TransitionBannerOut(float duration, CancellationToken ct)
    {
        var position = _rectTransform.position;
        var target = position.y + _rectHeight;
        await LMotion.Create(position.y, target, duration)
            .BindToPositionY(_rectTransform)
            .ToValueTask(ct);
    }

    public void ResetBanner()
    {
        _rectTransform.position = _startingPosition;
        UpdateText("");
    }

    public void UpdateText(string value)
    {
        _text.text = value;
    }
}
