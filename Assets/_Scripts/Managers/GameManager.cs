using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//diferentes estados del juego?
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
    // Estado actual del juego
    public GameState currentGameState; 

    [Header("References")]
    // Referencia al controlador del juego
    [SerializeField] GameController gameController; 
    // Referencia al controlador del jugador
    PlayerController player; 
    
    // Referencia al gestor de navegación
    private NavigationManager navigationManager; 
    // Referencia al gestor de la interfaz de usuario
    private UIManager uiManager; 

    // Diccionario para mapear estados del juego a índices de pantallas
    private Dictionary<GameState, int> gameStateToScreenIndex;

    private void Awake()
    {
        // Obtener referencias a los gestores de navegación e interfaz de usuario
        Transform parentObj = transform.parent;
        navigationManager = parentObj.GetComponentInChildren<NavigationManager>();
        uiManager = parentObj.GetComponentInChildren<UIManager>();

        // Inicializar el diccionario
        gameStateToScreenIndex = new Dictionary<GameState, int>
        {
            { GameState.Menu, 0 },
            { GameState.LevelSelector, 1 },
            { GameState.LoreScreen, 2 },
            { GameState.TutorialScreen, 3 },
            { GameState.CreditsScreen, 4 },
            { GameState.ConfigurationScreen, 5 },
            { GameState.PauseScreen, 6 },
            { GameState.LoseScreen, 7 },
            { GameState.NextLevelScreen, 8 },
            { GameState.WinScreen, 9 },
            { GameState.Game, 10 }
        };
    }

    private void Start()
    {
        // Inicializar el controlador del jugador y establecer el estado inicial del juego
        player = gameController.player;
        SetGameState(currentGameState);
    }

    // Método para establecer el estado del juego
    public void SetGameState(GameState gameState)
    {
        currentGameState = gameState;
        navigationManager.DisableAllScreens();

        if (gameState == GameState.Menu)
        {
            gameController.gameObject.SetActive(false);
            navigationManager.CheckPreviousPlay(gameController.maxLevelReached);
        }
        else if (gameState == GameState.Game)
        {
            gameController.gameObject.SetActive(true);
            gameController.LoadNextLevel();
        }

        // Activar la pantalla correspondiente según el estado del juego
        if (gameStateToScreenIndex.TryGetValue(gameState, out int screenIndex))
        {
            navigationManager.SetScreen(screenIndex);
        }
    }

    // Método para guardar el progreso del jugador
    public void SavePlayerProgress() => gameController.SavePlayerProgress();
}
