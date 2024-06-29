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
    GeneratedNumbersData<int> GetNumbers(GameConfig gameConfig);
}

public struct GeneratedNumbersData<T>
{
    public List<T> TotalNumbers;
    public HashSet<T> CorrectNumbers;
    public List<T> IncorrectNumbers;
    public T SolutionTarget;
}

public class NumberGeneratorService : INumberGeneratorService
{
    private int _currentResetCount;
    private readonly System.Random _random = new();
    public GeneratedNumbersData<int> GetNumbers(GameConfig gameConfig)
    {
        var result = new GeneratedNumbersData<int>();
        switch (gameConfig.EquationOperation)
        {
            case EquationOperation.Addition:
                result = gameConfig.IsDuplicatesAllowed 
                    ? GetAdditionNumberSet(gameConfig.SolutionTarget, gameConfig.NumberSetLength, gameConfig.CorrectNumbersLength) 
                    : GetAdditionNumberList(gameConfig.SolutionTarget, gameConfig.NumberSetLength, gameConfig.CorrectNumbersLength);
                break;
            case EquationOperation.Subtraction:
                break;
            case EquationOperation.Multiplication:
                break;
            case EquationOperation.Division:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameConfig.EquationOperation), gameConfig.EquationOperation, null);
        }
        
        return result;
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

    private GeneratedNumbersData<int> GetAdditionNumberSet(int solutionTarget, int setLength, int correctNumbers)
    {
        _currentResetCount = 0;
        var remainderLength = setLength - correctNumbers;
        var hashSet = new HashSet<int>();
        var remainders = new List<int>();
        var totalNumbers = new List<int>();

        // iterate over the correctNumbers int by 2, creating pairs of correct bubbles
        for (int i = 0; i < correctNumbers; i+= 2)
        {
            var current = Random.Range(0, solutionTarget);

            while (hashSet.Contains(current))
            {
                current = Random.Range(0, solutionTarget);
                _currentResetCount++;
            }

            var compliment = solutionTarget - current;
            hashSet.Add(current);
            hashSet.Add(compliment);
        }

        var solutionSet = new HashSet<int>(hashSet);
        
        // iterate over the remainder and just create random bubbles
        for (int i = 0; i < remainderLength; i++)
        {
            var current = Random.Range(0, solutionTarget);
            
            while (hashSet.Contains(current))
            {
                current = Random.Range(0, solutionTarget);
                _currentResetCount++;
            }

            remainders.Add(current);
        }
        
        totalNumbers.AddRange(solutionSet);
        totalNumbers.AddRange(remainders);

        var data = new GeneratedNumbersData<int>
        {
            SolutionTarget = solutionTarget,
            CorrectNumbers = solutionSet,
            IncorrectNumbers = remainders,
            TotalNumbers = totalNumbers
        };
        
        EventManager.RaiseEvent(GameEvents.NumberGenerationComplete, new Dictionary<string, object>
        {
            [GameParams.data] = data
        });

        return data;

        // Debug.Log($"args0: {solutionTarget}, args1: {setLength}, args2: {correctNumbers}, remainderLength: {remainderLength}");
        // Debug.Log($"time: {sw.Elapsed}, length: {result.Count}, resets: {_currentResetCount}\nsb: {sb}");
    }
    
    private GeneratedNumbersData<int> GetAdditionNumberList(int solutionTarget, int setLength, int correctNumbers)
    {
        // TODO: evaluate this list approach, I don't think O(n^2) is the end of the world here when
        // 20 <= n <= 60 but it's always worth checking. Also this algorithm as a whole should be evaluated
        // by others
        _currentResetCount = 0;
        var remainderLength = setLength - correctNumbers;
        var remainders = new List<int>();
        var solutionSet = new HashSet<int>();
        var totalNumbers = new List<int>();

        // iterate over the correctNumbers int by 2, creating pairs of correct bubbles
        for (int i = 0; i < correctNumbers; i+= 2)
        {
            var current = Random.Range(0, solutionTarget);
            var compliment = solutionTarget - current;
            
            solutionSet.Add(current);
            solutionSet.Add(compliment);
        }
        
        // iterate over the remainder and just create random bubbles
        for (int i = 0; i < remainderLength; i++)
        {
            var current = Random.Range(0, solutionTarget);
            
            while (solutionSet.Contains(current))
            {
                current = Random.Range(0, solutionTarget);
                _currentResetCount++;
            }

            remainders.Add(current);
        }
        
        totalNumbers.AddRange(remainders);
        totalNumbers.AddRange(solutionSet);
        
        var data = new GeneratedNumbersData<int>
        {
            SolutionTarget = solutionTarget,
            CorrectNumbers = solutionSet,
            IncorrectNumbers = remainders,
            TotalNumbers = totalNumbers
        };
        
        EventManager.RaiseEvent(GameEvents.NumberGenerationComplete, new Dictionary<string, object>
        {
            [GameParams.data] = data
        });

        return data;

        // Debug.Log($"args0: {solutionTarget}, args1: {setLength}, args2: {correctNumbers}, remainderLength: {remainderLength}");
        // Debug.Log($"time: {sw.Elapsed}, length: {result.Count}, resets: {_currentResetCount}\nsb: {sb}");
    }

    #endregion
    
}
