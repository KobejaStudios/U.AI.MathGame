using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public interface INumberGeneratorService
{
    UniTask<GeneratedNumbersData<int>> Async_GetAdditionNumbersInt(GameConfig gameConfig);
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
    
    private void ShuffleValues<T>(List<T> values)
    {
        var n = values.Count;
        for (int i = n - 1; i > 0; i--)
        {
            var j = _random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }
    }

    #endregion

    #region Addition

    public async UniTask<GeneratedNumbersData<int>> Async_GetAdditionNumbersInt(GameConfig gameConfig)
    {
        return await UniTask.RunOnThreadPool(async () =>
        {
            _currentResetCount = 0;
            var setLength = gameConfig.NumberSetLength;
            var correctNumbers = gameConfig.CorrectNumbersLength;
            var solutionTarget = gameConfig.SolutionTarget;
            var remainderLength = setLength - correctNumbers;
            var maxRandomBound = solutionTarget + 1;
            var hashSet = new HashSet<int>(setLength);
            var remainders = new List<int>();
        

            // iterate over the correctNumbers int by 2, creating pairs of correct bubbles
            for (int i = 0; i < correctNumbers; i += 2)
            {
                var current = _random.Next(maxRandomBound);
                var compliment = solutionTarget - current;

                while (hashSet.Contains(current) || current == compliment)
                {
                    current = _random.Next(maxRandomBound);
                    compliment = solutionTarget - current;
                    _currentResetCount++;
                }

                hashSet.Add(current);
                hashSet.Add(compliment);
            }

            var solutionSet = new HashSet<int>(hashSet);
        
            // iterate over the remainder and just create random bubbles
            for (int i = 0; i < remainderLength; i++)
            {
                var current = _random.Next(maxRandomBound);
            
                while (hashSet.Contains(current))
                {
                    current = _random.Next(maxRandomBound);
                    _currentResetCount++;
                }

                hashSet.Add(current);
                remainders.Add(current);
            }

            var totalNumbers = hashSet.ToList();

            switch (gameConfig.BubbleOrientation)
            {
                case BubbleCollectionOrientation.Shuffled:
                    ShuffleValues(totalNumbers);
                    break;
                case BubbleCollectionOrientation.LowSorted:
                    totalNumbers.Sort((x, y) => x.CompareTo(y));
                    break;
                case BubbleCollectionOrientation.HighSorted:
                    totalNumbers.Sort((x, y) => y.CompareTo(x));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var data = new GeneratedNumbersData<int>
            {
                SolutionTarget = solutionTarget,
                CorrectNumbers = solutionSet,
                IncorrectNumbers = remainders,
                TotalNumbers = totalNumbers
            };

            await UniTask.SwitchToMainThread();
            
            EventManager.RaiseEvent(GameEvents.IntNumberGenerationComplete, new Dictionary<string, object>
            {
                [GameParams.data] = data
            });
            
            return data;
        });
    }

    #endregion
    
}
