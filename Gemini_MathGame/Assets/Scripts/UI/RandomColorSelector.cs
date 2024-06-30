using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomColorSelector : MonoBehaviour
{
    public static RandomColorSelector Instance;
    [SerializeField] private Color[] _colors;
    private Queue<Color> _colorsQueue = new();

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

    private void Start()
    {
        for (int i = 0; i < _colors.Length; i++)
        {
            _colorsQueue.Enqueue(_colors[i]);
        }
    }

    public Color GetColor()
    {
        var current = _colorsQueue.Dequeue();
        _colorsQueue.Enqueue(current);
        return current;
    }
}
