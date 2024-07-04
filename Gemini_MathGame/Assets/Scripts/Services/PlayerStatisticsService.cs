using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        var stat = PlayerPrefs.GetInt(statisticName);
        stat += incrementValue;
        PlayerPrefs.SetInt(statisticName, stat);
    }

    public void DecrementStatistic(string statisticName, int decrementValue)
    {
        var stat = PlayerPrefs.GetInt(statisticName);
        stat -= decrementValue;
        PlayerPrefs.SetInt(statisticName, stat);
    }

    public void SetStatistic(string statisticName, int value)
    {
        PlayerPrefs.SetInt(statisticName, value);
    }

    public int GetStatistic(string statisticName)
    {
        return PlayerPrefs.GetInt(statisticName, 0);
    }
}
