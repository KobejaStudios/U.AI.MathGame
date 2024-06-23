public static class GameEvents
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
    
    // TODO: not sure I'm handling these win and loss events as ideally as I can, revisit in future
    public const string RoundWon = "RoundWon";
    public const string RoundLost = "RoundLost";
    public const string ScoreGoalReached = "ScoreGoalReached";
    public const string TimeExpired = "TimeExpired";
}
