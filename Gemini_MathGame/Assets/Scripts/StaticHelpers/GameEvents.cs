public partial class GameEvents
{
    public const string SolutionDefined = "SolutionDefined";
    public const string PairsGoalDefined = "PairsGoalDefined";
    public const string BubbleClicked = "BubbleClicked";
    public const string EvaluateSolution = "EvaluateSolution";
    public const string SolutionEvaluated = "SolutionEvaluated";
    public const string CorrectSolution = "CorrectSolution";
    public const string ScoreAdded = "ScoreAdded";
    public const string StartRound = "StartRound";
    public const string RoundStarted = "RoundStarted";
    public const string GameConfigDefined = "GameConfigDefined";
    public const string IntNumberGenerationComplete = "IntNumberGenerationComplete"; // payload: GeneratedNumbersData<int> data
    
    // TODO: not sure I'm handling these win and loss events as ideally as I can, revisit in future
    public const string StatisticChanged = "StatisticChanged"; // payload: string statisticName, int statisticValue
    public const string RoundWon = "RoundWon";
    public const string RoundLost = "RoundLost";
    public const string ScoreGoalReached = "ScoreGoalReached";
    public const string TimeExpired = "TimeExpired";
    // event that's fired when the player reaches low time, maybe this low time value will be a percentage
    // of the initial time limit or just a flat 10 seconds. Currently using flat 10 seconds
    public const string TimeRunningOut = "TimeRunningOut";
}

public partial class GameParams
{
    public const string gameConfig = "gameConfig";
    public const string solution = "solution";
    public const string bubbles = "bubbles";
    public const string currentScore = "currentScore";
    public const string pairsGoal = "pairsGoal";
    public const string correctNumbers = "correctNumbers";
    public const string incorrectNumbers = "incorrectNumbers";
    public const string totalNumbers = "totalNumbers";
    public const string data = "data";
}
