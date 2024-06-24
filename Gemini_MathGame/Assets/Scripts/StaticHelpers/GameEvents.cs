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
    // event that's fired when the player reaches low time, maybe this low time value will be a percentage
    // of the initial time limit or just a flat 10 seconds. Currently using flat 10 seconds
    public const string TimeRunningOut = "TimeRunningOut";
}
