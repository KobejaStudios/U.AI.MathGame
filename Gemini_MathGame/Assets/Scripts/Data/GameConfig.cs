using System.Collections.Generic;

public class GameConfig
{
    public int SolutionTarget { get; private set; }
    public int NumberSetLength { get; private set; }
    public int CorrectNumbersLength { get; private set; }
    public EquationOperation EquationOperation { get; private set; }
    public BubbleCollectionOrientation BubbleOrientation { get; private set; }
    public bool IsDuplicatesAllowed { get; private set; }

    /// <summary>
    /// when building a config, we currently do not support duplicate bubbles
    /// which leaves us in a state where the value defined for 'solutionTarget'
    /// becomes the max available value for both 'numbersSetLength' and 'correctNumbers'.
    /// 
    /// With one RULE, 'numbersSetLength' and 'correctNumbers' MUST be multiples of 2.
    /// </summary>
    /// <param name="solutionTarget"> The 'goal' of the game. 2 bubbles must combine to reach this goal. </param>
    /// <param name="numbersSetLength"> The amount of number bubbles we spawn for the game. </param>
    /// <param name="correctNumbers"> The amount of number bubbles that are part of a correct pairing.
    /// A correct pairing is 2 numbers that combine to reach the solutionTarget goal. </param>
    /// <param name="equationOperation"> The mathematical operation used for the game. </param>
    /// <param name="bubbleOrientation"> The sorting of the bubbles for this specific game. </param>
    /// <param name="isDuplicatesAllowed"> The choice to allow multiple bubbles containing the same number. </param>
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
