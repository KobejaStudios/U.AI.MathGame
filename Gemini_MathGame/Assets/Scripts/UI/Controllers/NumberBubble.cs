using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NumberBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private Sprite[] _spriteSheet;
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _baseColorIfShader;

    [SerializeField] private Material _mat;

    private bool _isShader => _mat != null;
    
    public TextMeshProUGUI Text => _text;
    public Image Image => _image;
    public RectTransform Transform => (RectTransform)transform;
    public int Id => gameObject.GetInstanceID();
    public int Value { get; set; }
    public BubbleType BubbleType { get; set; }
    public BubbleState State { get; set; }

    private Vector3 _startingPosition;

    #region Unity Lifecycle

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ResetBubbleColor();
        RandomizeShaderProperties();
    }

    #endregion

    #region Shader

    private static readonly int RainbowBrightness = Shader.PropertyToID(ShaderProperties.RainbowBrightness);
    private static readonly int RainbowSpeed = Shader.PropertyToID(ShaderProperties.RainbowSpeed);
    private static readonly int RainbowDensity = Shader.PropertyToID(ShaderProperties.RainbowDensity);
    private static readonly int RainbowCenter = Shader.PropertyToID(ShaderProperties.RainbowCenter);
    private void SetRainbowBrightness(float value)
    {
        if (_mat != null)
        {
            _mat.SetFloat(RainbowBrightness, value);
        }
    }

    private void SetRainbowSpeed(float value)
    {
        if (_mat != null)
        {
            _mat.SetFloat(RainbowSpeed, value);
        }
    }

    private void SetRainbowDensity(float value)
    {
        if (_mat != null)
        {
            _mat.SetFloat(RainbowDensity, value);
        }
    }

    private void SetRainbowCenter(Vector4 value)
    {
        if (_mat != null)
        {
            _mat.SetVector(RainbowCenter, value);
        }
    }

    private void RandomizeShaderProperties()
    {
        if (_mat == null) return;
        var brightness = Random.Range(1.5f, 2f);
        var speed = Random.Range(0.03f, 0.05f);
        var density = Random.Range(0.4f, 0.7f);
        var centerX = Random.Range(0f, 3f);
        var centerY = Random.Range(0f, 3f);

        SetRainbowBrightness(brightness);
        SetRainbowDensity(density);
        SetRainbowSpeed(speed);
        SetRainbowCenter(new Vector4(centerX, centerY));
    }

    #endregion

    private void OnClick()
    {
        if (State == BubbleState.Clicked) return;
        State = BubbleState.Clicked;
        _startingPosition = this.Transform.position;
        var eventData = new Dictionary<string, object>
        {
            ["id"] = Id,
            ["data"] = this
        };
        if (!_isShader)
        {
            _image.color = Color.yellow;
        }

        EventManager.RaiseEvent(GameEvents.BubbleClicked, eventData);
    }

    public void UpdateValue(int value)
    {
        Value = value;
        _text.text = value.ToString();
    }

    public void UpdateValue(float value)
    {
        _text.text = value.ToString(CultureInfo.InvariantCulture);
    }

    public void ClearBubbleState()
    {
        State = BubbleState.NotClicked;
    }

    public void ResetBubbleColor()
    {
        Image.color = _isShader ? _baseColorIfShader : _baseColor;
    }

    public void SetBubbleAlpha(float value)
    {
        _canvasGroup.alpha = value;
    }

    public void ResetBubble()
    {
        Value = 0;
        State = BubbleState.NotClicked;
        Image.color = _isShader ? _baseColorIfShader : _baseColor;
        _text.text = Value.ToString();
        _canvasGroup.alpha = 1f;
        gameObject.SetActive(false);
    }

    [ContextMenu("animate")]
    public async void AnimationBubblePop()
    {
        var counter = 0;
        while (counter < _spriteSheet.Length)
        {
            _image.sprite = _spriteSheet[counter];
            await UniTask.Delay(25);
            counter++;
        }
    }
}

public enum BubbleType
{
    Int,
    Float
}

public enum BubbleState
{
    NotClicked,
    Clicked
}
