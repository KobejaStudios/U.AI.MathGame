using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundStartInfoPopup : PopupBase
{
    [SerializeField] private TextMeshProUGUI _bodyText;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Button _closeButton;

    private void Start()
    {
        _toggle.isOn = PlayerPrefs.GetInt(Prefs.DontShowInfoPopupAgain) == 1;
        _toggle.onValueChanged.AddListener(OnToggleChanged);
        _closeButton.onClick.AddListener(() => Close());
    }

    protected override void ShowImpl()
    {
        if (CurrentContext.TryGetAs(GameParams.solution, out int solutionTarget))
        {
            _bodyText.text = $"The goal is to find the bubbles that add up to {solutionTarget}! Pop the bubbles and find as many solutions as you can";
        }
    }

    private void OnToggleChanged(bool value)
    {
        int result = value ? 1 : 0;
        PlayerPrefs.SetInt(Prefs.DontShowInfoPopupAgain, result);
        Debug.Log(PlayerPrefs.GetInt(Prefs.DontShowInfoPopupAgain));
    }
}
