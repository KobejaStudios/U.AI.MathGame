using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;
    
    public TextMeshProUGUI Text => _text;
    public Image Image => _image;
    public RectTransform Transform => (RectTransform)transform;
}
