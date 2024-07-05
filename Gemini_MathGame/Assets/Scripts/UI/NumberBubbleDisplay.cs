using System.Globalization;
using TMPro;
using UnityEngine;

public class NumberBubbleDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public void UpdateText(int value)
    {
        _text.text = value.ToString();
    }
    
    public void UpdateText(float value)
    {
        _text.text = value.ToString(CultureInfo.InvariantCulture);
    }
    
    public void UpdateText(string value)
    {
        _text.text = value;
    }
}
