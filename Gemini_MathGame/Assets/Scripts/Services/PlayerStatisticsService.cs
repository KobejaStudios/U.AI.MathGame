using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Statistic
{
    public string name;
    public long amount;

    public Statistic(string statisticName, long statisticAmount)
    {
        name = statisticName;
        amount = statisticAmount;
    }

    public override string ToString()
    {
        return $"[{name}:{amount}]";
    }
}

public static partial class GameParams
{
    public const string statisticName = "statisticName";
    public const string statisticValue = "statisticValue";
    
    public const string gamesCompleted = "gamesCompleted";
    public const string gamesAttempted = "gamesAttempted";
    public const string gamesWon = "gamesWon";
    public const string gamesLost = "gamesLost";
}

public interface IPlayerStatisticsService
{
    void IncrementStatistic(string statisticName, int incrementValue);
    void DecrementStatistic(string statisticName, int decrementValue);
    void SetStatistic(string statisticName, int value);
    int GetStatistic(string statisticName);
    // GetAll();
}

/// <summary>
/// potential move to remote storage in near future, that is when we add the
/// GetAll() method.
/// </summary>
public class PlayerStatisticsService : IPlayerStatisticsService
{
    public void IncrementStatistic(string statisticName, int incrementValue)
    {
        var value = PlayerPrefs.GetInt(statisticName);
        value += incrementValue;
        PlayerPrefs.SetInt(statisticName, value);
        
        EventManager.RaiseEvent(GameEvents.StatisticChanged, new Dictionary<string, object>
        {
            [GameParams.statisticName] = statisticName,
            [GameParams.statisticValue] = value
        });
    }

    public void DecrementStatistic(string statisticName, int decrementValue)
    {
        var value = PlayerPrefs.GetInt(statisticName);
        value -= decrementValue;
        PlayerPrefs.SetInt(statisticName, value);
        
        EventManager.RaiseEvent(GameEvents.StatisticChanged, new Dictionary<string, object>
        {
            [GameParams.statisticName] = statisticName,
            [GameParams.statisticValue] = value
        });
    }

    public void SetStatistic(string statisticName, int value)
    {
        PlayerPrefs.SetInt(statisticName, value);
        
        EventManager.RaiseEvent(GameEvents.StatisticChanged, new Dictionary<string, object>
        {
            [GameParams.statisticName] = statisticName,
            [GameParams.statisticValue] = value
        });
    }

    public int GetStatistic(string statisticName)
    {
        return PlayerPrefs.GetInt(statisticName, 0);
    }
}
