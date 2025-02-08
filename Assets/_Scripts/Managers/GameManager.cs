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
    ConfigurationScreen,
    CreditsScreen,
    LoreScreen,
    TutorialScreen,
    PauseScreen,
    LoseScreen,
    NextLevelScreen,
    WinScreen,
    Game,
    LanguageSelector,
    Exit
}

public class GameManager : MonoBehaviour
{
    private readonly string filePath = Application.dataPath + "/PlayerDataFile.json";

    [Header("Debug")]
    public GameState currentGameState;
    [SerializeField] private bool startFromLevel;
    [SerializeField] private int levelToLoad;

    [Header("References")]
    [SerializeField] LevelManager levelManager;

    [Header("Local References")]
    [SerializeField] NavigationManager navigationManager;
    [SerializeField] HUDManager uiManager;
    [SerializeField] MusicManager musicManager;

    private void Awake()
    {
        PlayerData playerData = LoadFromJson();
        levelManager.currentLevel = playerData.currentLevel;
    }

    private void Start() => SetGameState(currentGameState);
    public void SetGameState(GameState gameState)
    {
        currentGameState = gameState;

        switch (gameState)
        {
            case GameState.Menu:
                musicManager.SetMusicTrack(0);
                levelManager.gameObject.SetActive(false);
                navigationManager.ActivateScreen(0);
                startFromLevel = false;
                break;

            case GameState.Game:
                musicManager.SetMusicTrack(1);
                levelManager.gameObject.SetActive(true);
                if (!startFromLevel)
                {
                    levelManager.LoadLevel(0);
                    navigationManager.ActivateScreen(4);
                }
                else
                {
                    levelManager.LoadLevel(levelToLoad);
                    navigationManager.ActivateScreen(10);
                }
                break;

            case GameState.Exit:
                StartCoroutine(ExitGame());
                break;

            case GameState.LoreScreen:
                navigationManager.ActivateScreen(5);
                break;
            case GameState.TutorialScreen:
                navigationManager.ActivateScreen(10);
                break;

            default:
                break;
        }
        //navigationManager.ActivateScreen(levelManager.currentLevel);

    }

    public void SetLevelToLoad(int index)
    {
        startFromLevel = true;
        levelToLoad = index;
    }

    private IEnumerator ExitGame()
    {
        //---------- PLAYER PROGRESS ------------------------------------------------------------------------------------------------------------------
         PlayerData currentData = new PlayerData
        {
            currentLevel = levelManager.currentLevel,
        };
        SaveToJson(currentData);

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
        PlayerData defaultData = new PlayerData
        {
            currentLevel = 1
        };

        SaveToJson(defaultData);
        return defaultData;
    }

}
