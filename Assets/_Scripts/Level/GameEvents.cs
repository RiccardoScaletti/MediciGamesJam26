// GameEvents.cs
using System;

public static class GameEvents
{
    public static event Action LevelCompleted;

    public static void RaiseLevelCompleted()
    {
        LevelCompleted?.Invoke();
    }
}