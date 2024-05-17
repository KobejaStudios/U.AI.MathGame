using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    public int score;

    public void UpdateScoreText(string value)
    {
        _scoreText.text = value;
        score = int.Parse(value);
    }
    
    public void UpdateScoreText(int value)
    {
        _scoreText.text = value.ToString();
        score = value;
    }

    public void IncrementScore(int value)
    {
        score += value;
        _scoreText.text = score.ToString();
    }
}
