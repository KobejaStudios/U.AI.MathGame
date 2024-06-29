using NUnit.Framework;
using UnityEngine;

public class NumberGeneratorTests
{
    [Test]
    public void NumberGenerator_StressTest()
    {
        INumberGeneratorService service = new NumberGeneratorService();

        var count = 10000;

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
            var result = service.GetNumbers(data);
            Assert.IsTrue(data.SolutionTarget == result.SolutionTarget);
            Assert.IsTrue(data.NumberSetLength == result.TotalNumbers.Count);
            Assert.IsTrue(data.CorrectNumbersLength == result.CorrectNumbers.Count);
            Assert.IsTrue(data.NumberSetLength - data.CorrectNumbersLength == result.IncorrectNumbers.Count);
            Assert.IsTrue(data.NumberSetLength == result.CorrectNumbers.Count + result.IncorrectNumbers.Count);
        }
    }
}
