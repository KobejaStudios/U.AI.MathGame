using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelectionController : MonoBehaviour
{
    [Header("Solution Target")] 
    [SerializeField] private TextMeshProUGUI _valueDisplay;
    [SerializeField] private Slider _slider;

    private void Awake()
    {
        _slider.onValueChanged.AddListener(x =>
        {
            _valueDisplay.text = x.ToString(CultureInfo.InvariantCulture);
        });
        _slider.onValueChanged?.Invoke(_slider.value);
    }

    private void SubmitGameConfig()
    {
        var config = new GameConfig
            (200,
            32,
            16,
            EquationOperation.Addition,
            BubbleCollectionOrientation.Shuffled,
            false
            );
        
        EventManager.RaiseEvent(GameEvents.GameConfigDefined, new Dictionary<string, object>
        {
            [GameParams.gameConfig] = config
        });
    }
}
