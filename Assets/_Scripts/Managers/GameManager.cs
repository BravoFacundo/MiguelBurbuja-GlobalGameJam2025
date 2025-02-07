using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    private readonly string filePath = Application.dataPath + "/PlayerDataFile.json";

    [Header("Debug")]
    public GameState currentGameState; 

    [Header("References")]
    [SerializeField] LevelManager levelManager;
    
    [Header("Local References")]
    [SerializeField] NavigationManager navigationManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] MusicManager musicManager;

    private void Awake()
    {
        LoadPlayerProgress();
    }

    private void Start() => SetGameState(currentGameState);

    public void SetGameState(GameState gameState)
    {
        currentGameState = gameState;
        navigationManager.DisableAllScreens();

        if (gameState == GameState.Menu)
        {
            levelManager.gameObject.SetActive(false);
            CheckPlayerProgress(levelManager.maxLevelReached);
            musicManager.SetMusicTrack(0);
        }
        else if (gameState == GameState.Game)
        {
            levelManager.gameObject.SetActive(true);
            levelManager.LoadNextLevel();
            musicManager.SetMusicTrack(1);
        }
        else if (gameState == GameState.Exit) StartCoroutine(ExitGame());

        int screenIndex = navigationManager.GetScreenIndex(gameState);
        navigationManager.SetScreen(screenIndex);
    }

    private IEnumerator ExitGame()
    {
        SavePlayerProgress();
        Debug.Log("Exiting Game... Saving Progress");
        yield return new WaitForSeconds(.5f);

        Application.Quit();
    }

    //---------- PLAYER PROGRESS ------------------------------------------------------------------------------------------------------------------

    public void CheckPlayerProgress(int maxLevelReached)
    {
        if (maxLevelReached == 1) uiManager.NoPreviousProgress();
        else uiManager.HasPreviousProgress();
    }
    public void LoadPlayerProgress()
    {
        PlayerData playerData = LoadFromJson();
        levelManager.maxLevelReached = playerData.maxLevelReached;
        levelManager.currentLevel = playerData.currentLevel;
    }
    public void SavePlayerProgress()
    {
        PlayerData currentData = new PlayerData
        {
            currentLevel = levelManager.currentLevel,
            maxLevelReached = levelManager.maxLevelReached
        };
        SaveToJson(currentData);
    }

    //---------- SAVE DATA ------------------------------------------------------------------------------------------------------------------

    private void SaveToJson(PlayerData dataToSave)
    {
        try
        {
            string json = JsonUtility.ToJson(dataToSave, true);
            File.WriteAllText(filePath, json);
            Debug.Log("Data saved successfully.");
        }
        catch (IOException ex)
        {
            Debug.LogError($"Error saving data: {ex.Message}");
        }
    }

    private PlayerData LoadFromJson()
    {
        try
        {
            if (!File.Exists(filePath) || string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
            {
                Debug.LogWarning("Save file not found or empty. Creating default data.");
                return CreateDefaultData();
            }

            string json = File.ReadAllText(filePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            if (data == null)
            {
                Debug.LogError("Failed to parse JSON. Creating default data.");
                return CreateDefaultData();
            }

            return data;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error loading data: {ex.Message}");
            return CreateDefaultData();
        }
    }

    private PlayerData CreateDefaultData()
    {
        PlayerData defaultData = new PlayerData
        {
            maxLevelReached = 1,
            currentLevel = 1
        };

        SaveToJson(defaultData);
        return defaultData;
    }

}
