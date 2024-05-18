using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SolutionTargetController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _solutionTargetText;

    public void UpdateText(int value)
    {
        _solutionTargetText.text = value.ToString();
    }
}
