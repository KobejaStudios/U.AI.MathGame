using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig", fileName = "GameConfigSO")]
public class GameConfigSO : ScriptableObject
{
    public NetworkMode networkMode;
    public EquationOperation equationOperation;
}

public enum EquationOperation
{
    Addition,
    Subtraction,
    Multiplication,
    Division
}

public enum NetworkMode
{
    Offline,
    Online
}
