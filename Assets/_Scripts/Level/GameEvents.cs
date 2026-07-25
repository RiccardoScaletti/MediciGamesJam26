// GameEvents.cs
using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static event Action LevelCompleted;

    public static void RaiseLevelCompleted()
    {
        LevelCompleted?.Invoke();
    }
}