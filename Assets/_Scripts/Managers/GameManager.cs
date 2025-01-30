using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Menu, 
    LevelSelector, 
    LoreScreen, 
    TutorialScreen, 
    CreditsScreen,
    ConfigurationScreen, 
    PauseScreen, 
    LoseScreen, 
    NextLevelScreen,  
    WinScreen,
    Game
}

public class GameManager : MonoBehaviour
{
    [Header("Debug")]
    public GameState currentGameState; 
    private Dictionary<GameState, int> gameStateToScreenIndex;

    [Header("References")]
    [SerializeField] GameController gameController;
    [SerializeField] NavigationManager navigationManager;
    [SerializeField] MusicManager musicManager;

    private void Awake()
    {
        InitializeDictionar();
    }

    private void InitializeDictionar()
    {
        gameStateToScreenIndex = new Dictionary<GameState, int>();

        int index = 0;
        foreach (GameState state in Enum.GetValues(typeof(GameState)))
        {
            gameStateToScreenIndex[state] = index;
            index++;
        }
    }

    private void Start()
    {
        SetGameState(currentGameState);
    }

    public void SetGameState(GameState gameState)
    {
        currentGameState = gameState;
        navigationManager.DisableAllScreens();

        if (gameState == GameState.Menu)
        {
            gameController.gameObject.SetActive(false);
            navigationManager.CheckPreviousPlay(gameController.maxLevelReached);
            musicManager.SetMusicTrack(0);
        }
        else if (gameState == GameState.Game)
        {
            gameController.gameObject.SetActive(true);
            gameController.LoadNextLevel();
            musicManager.SetMusicTrack(1);
        }

        if (gameStateToScreenIndex.TryGetValue(gameState, out int screenIndex))
        {
            navigationManager.SetScreen(screenIndex);
        }
    }

    public void SavePlayerProgress() => gameController.SavePlayerProgress();
}
