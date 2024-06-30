using System;
using UnityEngine;

public class RandomColorSelector : MonoBehaviour
{
    public static RandomColorSelector Instance;
    [SerializeField] private Color[] _colors;

    private System.Random _random = new();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Color GetColor()
    {
        return _colors[_random.Next(_colors.Length)];
    }
}
