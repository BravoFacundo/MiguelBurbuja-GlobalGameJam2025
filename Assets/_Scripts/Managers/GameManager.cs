using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    Game,
    Exit
}

public class GameManager : MonoBehaviour
{
    [Header("Debug")]
    public GameState currentGameState; 
    private Dictionary<GameState, int> gameStateToScreenIndex;

    [Header("References")]
    [SerializeField] GameController gameController;
    
    [Header("Local References")]
    [SerializeField] NavigationManager navigationManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] MusicManager musicManager;

    private void Awake()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        gameStateToScreenIndex = new Dictionary<GameState, int>();

        int index = 0;
        foreach (GameState state in Enum.GetValues(typeof(GameState)))
        {
            gameStateToScreenIndex[state] = index;
            index++;
        }
    }

    private void Start() => SetGameState(currentGameState);

    public void SetGameState(GameState gameState)
    {
        currentGameState = gameState;
        navigationManager.DisableAllScreens();

        if (gameState == GameState.Menu)
        {
            gameController.gameObject.SetActive(false);
            CheckPlayerProgress(gameController.maxLevelReached);
            musicManager.SetMusicTrack(0);
        }
        else if (gameState == GameState.Game)
        {
            gameController.gameObject.SetActive(true);
            gameController.LoadNextLevel();
            musicManager.SetMusicTrack(1);
        }
        else if (gameState == GameState.Exit) StartCoroutine(SavePlayerProgress());

        if (gameStateToScreenIndex.TryGetValue(gameState, out int screenIndex))
        {
            navigationManager.SetScreen(screenIndex);
        }
    }

    public void CheckPlayerProgress(int maxLevelReached)
    {
        if (maxLevelReached == 1) uiManager.NoPreviousProgress();
        else uiManager.HasPreviousProgress();
    }
    private IEnumerator SavePlayerProgress()
    {
        Debug.Log("Exiting Game... Saving Progress");
        gameController.SavePlayerProgress(); //Esto tiene que estar aca en realidad
        yield return new WaitForSeconds(.5f);

        Application.Quit();
    }

}
