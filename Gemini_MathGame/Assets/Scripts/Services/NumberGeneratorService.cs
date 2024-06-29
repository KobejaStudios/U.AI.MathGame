using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public interface INumberGeneratorService
{
    List<int> GetNumbers(int solutionTarget, int setLength, int correctNumbers, EquationOperation equationType, BubbleCollectionOrientation bubblesOrientation = BubbleCollectionOrientation.Shuffled, bool isAllowDuplicates = false);
}
public class NumberGeneratorService : INumberGeneratorService
{
    private int _currentResetCount;
    private readonly System.Random _random = new();
    public List<int> GetNumbers(int solutionTarget, int setLength, int correctNumbers, EquationOperation equationType, BubbleCollectionOrientation bubblesOrientation = BubbleCollectionOrientation.Shuffled, bool isAllowDuplicates = false)
    {
        var result = new List<int>();
        switch (equationType)
        {
            case EquationOperation.Addition:
                result = isAllowDuplicates 
                    ? GetAdditionNumberSet(solutionTarget, setLength, correctNumbers) 
                    : GetAdditionNumberList(solutionTarget, setLength, correctNumbers);
                break;
            case EquationOperation.Subtraction:
                break;
            case EquationOperation.Multiplication:
                break;
            case EquationOperation.Division:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(equationType), equationType, null);
        }
        
        return ShuffleValues(result);
    }

    #region Generics

    private List<T> ShuffleValues<T>(IEnumerable<T> values)
    {
        var oldList = new List<T>(values);
        var newList = new List<T>();

        while (oldList.Count > 0)
        {
            var index = _random.Next(0, oldList.Count - 1);
            var current = oldList[index];
            newList.Add(current);
            oldList.RemoveAt(index);
        }

        return newList;
    }

    #endregion

    #region Addition

    private List<int> GetAdditionNumberSet(int solutionTarget, int setLength, int correctNumbers)
    {
        _currentResetCount = 0;
        var remainderLength = setLength - correctNumbers;
        var result = new HashSet<int>();

        // iterate over the correctNumbers int by 2, creating pairs of correct bubbles
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
        }
        
        // iterate over the remainder and just create random bubbles
        for (int i = 0; i < remainderLength; i++)
        {
            var current = Random.Range(0, solutionTarget);
            
            while (result.Contains(current))
            {
                current = Random.Range(0, solutionTarget);
                _currentResetCount++;
            }

            result.Add(current);
        }

        return result.ToList();

        // Debug.Log($"args0: {solutionTarget}, args1: {setLength}, args2: {correctNumbers}, remainderLength: {remainderLength}");
        // Debug.Log($"time: {sw.Elapsed}, length: {result.Count}, resets: {_currentResetCount}\nsb: {sb}");
    }
    
    private List<int> GetAdditionNumberList(int solutionTarget, int setLength, int correctNumbers)
    {
        // TODO: evaluate this list approach, I don't think O(n^2) is the end of the world here when
        // 20 <= n <= 60 but it's always worth checking. Also this algorithm as a whole should be evaluated
        // by others
        _currentResetCount = 0;
        var remainderLength = setLength - correctNumbers;
        var result = new List<int>();
        var solutions = new List<int>();

        // iterate over the correctNumbers int by 2, creating pairs of correct bubbles
        for (int i = 0; i < correctNumbers; i+= 2)
        {
            var current = Random.Range(0, solutionTarget);
            var compliment = solutionTarget - current;
            
            solutions.Add(current);
            solutions.Add(compliment);
        }
        
        // iterate over the remainder and just create random bubbles
        for (int i = 0; i < remainderLength; i++)
        {
            var current = Random.Range(0, solutionTarget);
            
            while (solutions.Contains(current))
            {
                current = Random.Range(0, solutionTarget);
                _currentResetCount++;
            }

            result.Add(current);
        }

        result.AddRange(solutions);
        return result;

        // Debug.Log($"args0: {solutionTarget}, args1: {setLength}, args2: {correctNumbers}, remainderLength: {remainderLength}");
        // Debug.Log($"time: {sw.Elapsed}, length: {result.Count}, resets: {_currentResetCount}\nsb: {sb}");
    }

    #endregion
    
}
