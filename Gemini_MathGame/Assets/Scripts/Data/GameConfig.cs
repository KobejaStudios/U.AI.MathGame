using UnityEngine;

/// <summary>
/// possible future expansion is to create an interface for configs to allow for multiple config definitions
/// like duplicates allowed, etc...
/// </summary>
public class GameConfig
{
    public int SolutionTarget { get; private set; }
    public int NumberSetLength { get; private set; }
    public int CorrectNumbersLength { get; private set; }
    public EquationOperation EquationOperation { get; private set; }
    public BubbleCollectionOrientation BubbleOrientation { get; private set; }
    public bool IsDuplicatesAllowed { get; private set; }
    private const int MultipleOf = 2;

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
        ValidateAndCorrectProperties(solutionTarget, ref numbersSetLength, ref correctNumbers);
        
        SolutionTarget = solutionTarget;
        NumberSetLength = numbersSetLength;
        CorrectNumbersLength = correctNumbers;
        EquationOperation = equationOperation;
        BubbleOrientation = bubbleOrientation;
        IsDuplicatesAllowed = isDuplicatesAllowed;
    }

    private void ValidateAndCorrectProperties(int solutionTarget, ref int numbersSetLength, ref int correctNumbers)
    {
        if (numbersSetLength > solutionTarget)
        {
            Debug.LogWarning($"'numbersSetLength' cannot be greater than 'solutionTarget'. 'numbersSetLength' " +
                             $"has been lowered from: {numbersSetLength} to: {solutionTarget}");
            numbersSetLength = solutionTarget;
        }
        
        if (numbersSetLength % MultipleOf != 0)
        {
            var prev = numbersSetLength;
            numbersSetLength = (int)Mathf.Round(numbersSetLength / (float)MultipleOf) * MultipleOf;
            Debug.LogWarning($"'numbersSetLength' MUST be a multiple of 2. 'correctNumbers' " +
                             $"has been modified from: {prev} to: {numbersSetLength}");
        }

        if (correctNumbers > numbersSetLength)
        {
            Debug.LogWarning($"'correctNumbers' cannot be greater than 'numbersSetLength'. 'correctNumbers' " +
                             $"has been lowered from: {correctNumbers} to: {numbersSetLength}");
            correctNumbers = numbersSetLength;
        }

        if (correctNumbers % MultipleOf != 0)
        {
            var prev = correctNumbers;
            correctNumbers = (int)Mathf.Round(correctNumbers / (float)MultipleOf) * MultipleOf;
            Debug.LogWarning($"'correctNumbers' MUST be a multiple of 2. 'correctNumbers' " +
                             $"has been modified from: {prev} to: {correctNumbers}");
        }
    }
}
