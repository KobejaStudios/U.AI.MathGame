using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    
    public TextMeshProUGUI Text => _text;
    public Image Image => _image;
    public RectTransform Transform => (RectTransform)transform;
    public int Id => gameObject.GetInstanceID();
    public int Value { get; set; }
    public BubbleType BubbleType { get; set; }

    private Vector3 _startingPosition;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnClick()
    {
        _startingPosition = this.Transform.position;
        var eventData = new Dictionary<string, object>
        {
            ["id"] = Id,
            ["data"] = this
        };
        _image.color = Color.yellow;
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
}

public enum BubbleType
{
    Int,
    Float
}
