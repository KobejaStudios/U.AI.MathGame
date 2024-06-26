using System.Collections.Generic;

public class GameConfig
{
    public int SolutionTarget { get; private set; }
    public int NumberSetLength { get; private set; }
    public int CorrectNumbersLength { get; private set; }
    public EquationOperation EquationOperation { get; private set; }
    public BubbleCollectionOrientation BubbleOrientation { get; private set; }
    public bool IsDuplicatesAllowed { get; private set; }

    public GameConfig(
        int solutionTarget,
        int numbersSetLength,
        int correctNumbers,
        EquationOperation equationOperation,
        BubbleCollectionOrientation bubbleOrientation,
        bool isDuplicatesAllowed
        ) 
    {
        SolutionTarget = solutionTarget;
        NumberSetLength = numbersSetLength;
        CorrectNumbersLength = correctNumbers;
        EquationOperation = equationOperation;
        BubbleOrientation = bubbleOrientation;
        IsDuplicatesAllowed = isDuplicatesAllowed;
    }
}
