using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public interface INumberSetGenerator
{
    void GetNumberSet(int solutionTarget, int setLength, int correctNumbers);
}
public class NumberSetGenerator : INumberSetGenerator
{
    private int _currentResetCount;
    public void GetNumberSet(int solutionTarget, int setLength, int correctNumbers)
    {
        _currentResetCount = 0;
        var remainderLength = setLength - correctNumbers;
        var result = new HashSet<int>();
        var sb = new StringBuilder();
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < correctNumbers; i+= 2)
        {
            var current = Random.Range(0, solutionTarget);

            while (result.Contains(current))
            {
                current = Random.Range(0, solutionTarget);
                _currentResetCount++;
            }

            var compliment = solutionTarget - current;
            result.Add(current);
            result.Add(compliment);
            sb.AppendLine(current.ToString());
            sb.AppendLine(compliment.ToString());
        }

        sb.AppendLine();
        sb.AppendLine("====== random numbers below ======");
        sb.AppendLine();
        
        for (int i = 0; i < remainderLength; i++)
        {
            var current = Random.Range(0, solutionTarget);
            
            while (result.Contains(current))
            {
                current = Random.Range(0, solutionTarget);
                _currentResetCount++;
            }

            result.Add(current);
            sb.AppendLine(current.ToString());
        }
        
        Debug.Log($"args0: {solutionTarget}, args1: {setLength}, args2: {correctNumbers}, remainderLength: {remainderLength}");
        Debug.Log($"time: {sw.Elapsed}, length: {result.Count}, resets: {_currentResetCount}\nsb: {sb}");
    }
}
