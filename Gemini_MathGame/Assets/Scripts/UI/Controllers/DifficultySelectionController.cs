using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelectionController : MonoBehaviour
{
    [Header("Solution Target")] 
    [SerializeField] private TextMeshProUGUI _valueDisplay;
    [SerializeField] private Slider _solutionTargetSlider;

    [Header("Control Buttons")] 
    [SerializeField] private Button _submitButton;

    private void Awake()
    {
        _solutionTargetSlider.onValueChanged.AddListener(x =>
        {
            _valueDisplay.text = x.ToString(CultureInfo.InvariantCulture);
        });
        _solutionTargetSlider.onValueChanged?.Invoke(_solutionTargetSlider.value);
        
        _submitButton.onClick.AddListener(SubmitGameConfig);
    }

    private void SubmitGameConfig()
    {
        var config = new GameConfig
            ((int)_solutionTargetSlider.value,
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
