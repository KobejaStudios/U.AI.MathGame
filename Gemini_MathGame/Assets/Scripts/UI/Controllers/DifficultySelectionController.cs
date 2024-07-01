using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DifficultySelectionController : MonoBehaviour
{
    [Header("Solution Target")] 
    [SerializeField] private TextMeshProUGUI _solutionTargetDisplay;
    [SerializeField] private Slider _solutionTargetSlider;
    
    [Header("Number Set")] 
    [SerializeField] private TextMeshProUGUI _numberSetLengthDisplay;
    [SerializeField] private Slider _numberSetLengthSlider;
    
    [Header("Solution Set")] 
    [SerializeField] private TextMeshProUGUI _solutionSetLengthDisplay;
    [SerializeField] private Slider _solutionSetLengthSlider;

    [Header("Control Buttons")] 
    [SerializeField] private Button _submitButton;

    private void Awake()
    {
        _solutionTargetSlider.onValueChanged.AddListener(x =>
        {
            _solutionTargetDisplay.text = x.ToString(CultureInfo.InvariantCulture);
        });
        _solutionTargetSlider.onValueChanged?.Invoke(_solutionTargetSlider.value);
        
        _numberSetLengthSlider.onValueChanged.AddListener(OnNumberSetLengthChanged);
        _solutionSetLengthSlider.onValueChanged.AddListener(OnSolutionSetLengthChanged);
        
        _submitButton.onClick.AddListener(SubmitGameConfig);
    }

    private void OnNumberSetLengthChanged(float value)
    {
        _numberSetLengthSlider.value = RoundToValue(value, 4);
        _numberSetLengthDisplay.text = _numberSetLengthSlider.value.ToString(CultureInfo.InvariantCulture);

        _solutionSetLengthSlider.maxValue = _numberSetLengthSlider.value;
    }
    
    private void OnSolutionSetLengthChanged(float value)
    {
        _solutionSetLengthSlider.value = RoundToValue(value, 2);
        _solutionSetLengthDisplay.text = _solutionSetLengthSlider.value.ToString(CultureInfo.InvariantCulture);
    }

    private float RoundToValue(float sliderValue, int target)
    {
        return MathF.Round(sliderValue / target) * target;
    }

    private void SubmitGameConfig()
    {
        var config = new GameConfig
            ((int)_solutionTargetSlider.value,
                (int)_numberSetLengthSlider.value,
                (int)_solutionSetLengthSlider.value,
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
