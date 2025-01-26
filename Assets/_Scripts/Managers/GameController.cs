using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private readonly string filePath = Application.dataPath + "/PlayerDataFile.json";

    public int maxLevelReached = 1;
    public int currentLevel = 1;

    private Transform grid;

    [SerializeField] GameManager GameManager;
    public PlayerController player;
    [SerializeField] List<GameObject> levelPrefabs;


    private void Awake()
    {
        grid = transform.Find("Grid");
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        PlayerData playerData = LoadFromJson();
        maxLevelReached = playerData.maxLevelReached;
        currentLevel = playerData.currentLevel;
    }

    private void OnEnable()
    {
        //LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        Utilities.DeleteAllChildrens(grid);
        GameObject newLevel = Instantiate(levelPrefabs[maxLevelReached - 1], grid);
    }

    public void ReachedGoal()
    {
        //Debug.Log("Reached Goal, loading next level");
        currentLevel++;
        maxLevelReached++; Math.Clamp(maxLevelReached, 1, levelPrefabs.Count-1);
        if (maxLevelReached > levelPrefabs.Count) GameManager.SetGameState(GameState.WinScreen);
        else GameManager.SetGameState(GameState.NextLevelScreen);
    }

    public void SavePlayerProgress()
    {
        PlayerData currentData = new PlayerData();
        currentData.currentLevel = currentLevel;
        currentData.maxLevelReached = maxLevelReached;
        SaveToJson(currentData);
    }

    private void SaveToJson(PlayerData dataToSave)
    {
        try
        {
            string json = JsonUtility.ToJson(dataToSave, true);
            File.WriteAllText(filePath, json);
            Debug.Log("Data saved successfully.");
        }
        catch (IOException ex) { Debug.LogError($"Error saving data: {ex.Message}"); }        
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
            return null;
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
