using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

public class NumberGeneratorTests
{
    [Test]
    public async void NumberGenerator_StressTest()
    {
        INumberGeneratorService service = new NumberGeneratorService();

        var count = 1;

        while (count > 0)
        {
            count--;
            
            var data = new GameConfig(
                750,
                50,
                20,
                EquationOperation.Addition,
                BubbleCollectionOrientation.Shuffled,
                false
                );
            var result = await service.Async_GetAdditionNumbersInt(data);
            Assert.IsTrue(data.SolutionTarget == result.SolutionTarget);
            Assert.IsTrue(data.NumberSetLength == result.TotalNumbers.Count);
            Assert.IsTrue(data.CorrectNumbersLength == result.CorrectNumbers.Count);
            Assert.IsTrue(data.NumberSetLength - data.CorrectNumbersLength == result.IncorrectNumbers.Count);
            Assert.IsTrue(data.NumberSetLength == result.CorrectNumbers.Count + result.IncorrectNumbers.Count);
        }
    }
    
    [Test]
    public async void NumberGenerator_StressTestAsync()
    {
        INumberGeneratorService service = new NumberGeneratorService();

        var count = int.MaxValue;
        var s = count;

        while (count > 0)
        {
            count--;
            
            var data = new GameConfig(
                750,
                50,
                20,
                EquationOperation.Addition,
                BubbleCollectionOrientation.Shuffled,
                false
            );
            var result = await service.Async_GetAdditionNumbersInt(data);
            Assert.IsTrue(data.SolutionTarget == result.SolutionTarget);
            Assert.IsTrue(data.NumberSetLength == result.TotalNumbers.Count);
            Assert.IsTrue(data.CorrectNumbersLength == result.CorrectNumbers.Count);
            Assert.IsTrue(data.NumberSetLength - data.CorrectNumbersLength == result.IncorrectNumbers.Count);
            Assert.IsTrue(data.NumberSetLength == result.CorrectNumbers.Count + result.IncorrectNumbers.Count);
            Debug.Log($"ran test: {s} times");
        }
    }

    [Test]
    public async void NumberGeneratorResultQueue()
    {
        var queue = new Queue<GeneratedNumbersData<int>>();
        INumberGeneratorService service = new NumberGeneratorService();

        while (queue.Count < 10)
        {
            var data = new GameConfig(
                750,
                50,
                20,
                EquationOperation.Addition,
                BubbleCollectionOrientation.Shuffled,
                false
            );
            queue.Enqueue(await service.Async_GetAdditionNumbersInt(data));
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            Debug.Log($"data {queue.Count}: {current.ToJsonPretty()}");
        }
    }
}
