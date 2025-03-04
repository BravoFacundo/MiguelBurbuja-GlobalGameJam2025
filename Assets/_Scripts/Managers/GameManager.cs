using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Game, Lose, Win, Language }

public class GameManager : MonoBehaviour
{

    [Header("Debug")]
    public GameState currentGameState;
    public string currentScreen;
    public int currentLevel;
    public PlayerData playerData;

    [Header("Local References")]
    [SerializeField] NavigationManager navigationManager;
    [SerializeField] HUDManager uiManager;
    [SerializeField] MusicManager musicManager;

    [Header("References")]
    [SerializeField] LevelManager levelManager;

    private readonly string filePath = Application.dataPath + "/PlayerDataFile.json";

    private void Awake() => playerData = LoadFromJson();
    private void Start() => SetGameState(currentGameState);

    //---------- GAME STATES ------------------------------------------------------------------------------------------------------------------

    public void SetGameState(GameState gameState)
    {
        currentGameState = gameState;
        currentLevel = playerData.currentLevel;

        switch (gameState)
        {
            case GameState.Menu:
                levelManager.gameObject.SetActive(false);
                navigationManager.ActivateScreen("Menu");
                if (playerData.currentLevel != 1) navigationManager.HasPreviousProgress();
                musicManager.SetMusicTrack("Menu");
                musicManager.DisableTenseTrack();
                break;

            case GameState.Game:
                levelManager.gameObject.SetActive(true);
                levelManager.LoadLevel(playerData.currentLevel);
                navigationManager.ActivateScreen("Game");
                musicManager.SetMusicTrack("Game");
                break;

            case GameState.Lose:
                navigationManager.ActivateScreen("Lose");
                break;
            case GameState.Win:
                navigationManager.ActivateScreen("Win");
                break;

            case GameState.Language:
                navigationManager.ActivateScreen("Language");
                currentScreen = "Menu";
                break;
        }        
    }

    public IEnumerator ExitGame()
    {
        SaveToJson(playerData);
        Debug.Log("Exiting Game... Saving Progress");
        yield return new WaitForSeconds(.5f);
        Application.Quit();
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
        PlayerData defaultData = new()
        {
            currentLevel = 1 ,
            maxLevelReach = 1
        };

        SaveToJson(defaultData);
        return defaultData;
    }

}
