using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataScript
{
    public enum GameState
    {
        HomePage,
        onGame,
        PassedLevel,
        GameOver
    }

    public static HashData animHash;
    
    //WARNING!!! --- do not change in ManagerScript since it will change it in every scene restart
    static GameState gameState = GameState.HomePage;

    public static event Action onGameStarted;
    public static void ChangeState(GameState newState)
    {
        gameState = newState;

        if (newState == GameState.onGame && onGameStarted != null)
            onGameStarted();
    }

    public static GameState GetState()
    {
        return gameState;
    }

    public static int tackledAttackerCount;
    public static int totalAttackerCount;
    public static int goalCount;

    public static int totalDefenderCount;
    public static int slidedDefenderCount;
    
    public static bool isLevelPassed;
    public static bool isLevelAnimPlayed;

    public static int currentLevel;
    public static int maxLevel;
}
