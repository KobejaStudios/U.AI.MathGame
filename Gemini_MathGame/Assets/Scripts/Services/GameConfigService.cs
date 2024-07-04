using System.Collections.Generic;
using UnityEngine;

public interface IGameConfigService
{
    GameConfig GetGameConfig();
    void EnqueueGameConfig(GameConfig config);
    int GetQueueLength();
}

public class GameConfigService : IGameConfigService
{
    private IPlayerStatisticsService _statistics;
    private Queue<GameConfig> _gameConfigs = new();

    public GameConfigService() : this (ServiceLocator.Get<IPlayerStatisticsService>()) { }

    public GameConfigService(IPlayerStatisticsService statistics)
    {
        _statistics = statistics;
    }

    public GameConfig GetGameConfig()
    {
        if (_statistics.GetStatistic(GameParams.gamesWon) == 0)
        {
            return FtueGameConfig();
        }
        return _gameConfigs.Count > 0 ? _gameConfigs.Dequeue() : GetRandomConfig();
    }

    public void EnqueueGameConfig(GameConfig config)
    {
        _gameConfigs.Enqueue(config);
    }

    public int GetQueueLength()
    {
        return _gameConfigs.Count;
    }

    #region Helpers

    private GameConfig FtueGameConfig()
    {
        return new GameConfig(
            100,
            14,
            12,
            EquationOperation.Addition,
            BubbleCollectionOrientation.Shuffled,
            false
        );
    }
    
    private GameConfig GetRandomConfig()
    {
        return new GameConfig
        (
            Random.Range(10, 200),
            16,
            10,
            EquationOperation.Addition,
            BubbleCollectionOrientation.Shuffled,
            false
        );
    }

    #endregion
}
