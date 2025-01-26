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

    [Header("References")]
    [SerializeField] GameController gameController;
    PlayerController player;
    
    private NavigationManager navigationManager;
    private UIManager uiManager;

    private void Awake()
    {
        Transform parentObj = transform.parent;
        navigationManager = parentObj.GetComponentInChildren<NavigationManager>();
        uiManager = parentObj.GetComponentInChildren<UIManager>();
    }

    private void Start()
    {
        player = gameController.player;
        SetGameState(currentGameState);
    }

    public void SetGameState(GameState gameState)
    {
        currentGameState = gameState;
        
        navigationManager.DisableAllScreens();
        switch (gameState)
        {
            case GameState.Menu:
                gameController.gameObject.SetActive(false);
                navigationManager.SetScreen(0);
                navigationManager.CheckPreviousPlay(gameController.maxLevelReached);
                break;
            case GameState.LevelSelector:
                navigationManager.SetScreen(1);
                break;
            case GameState.LoreScreen:
                navigationManager.SetScreen(2);
                break;
            case GameState.TutorialScreen:
                navigationManager.SetScreen(3);
                break;
            case GameState.CreditsScreen:
                navigationManager.SetScreen(4);
                break;
            case GameState.ConfigurationScreen:
                navigationManager.SetScreen(5);
                break;
            case GameState.PauseScreen:
                navigationManager.SetScreen(6);
                break;
            case GameState.LoseScreen:
                navigationManager.SetScreen(7);
                break;
            case GameState.NextLevelScreen:
                navigationManager.SetScreen(8);
                break;
            case GameState.WinScreen:
                navigationManager.SetScreen(9);
                break;

            case GameState.Game:
                gameController.gameObject.SetActive(true);
                navigationManager.SetScreen(10);
                gameController.LoadNextLevel();
                break;
        }
    }

    public void SavePlayerProgress() => gameController.SavePlayerProgress();

}
