using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private readonly string filePath = Application.dataPath + "/PlayerDataFile.json";

    [Header("Data")]
    public int maxLevelReached = 1;
    public int currentLevel = 1;

    [Header("References")]
    [SerializeField] GameManager gameManager;

    [Header("Local References")]
    public PlayerController player;
    [SerializeField] private Transform grid;

    [Header("Levels")]    
    [SerializeField] List<GameObject> levelPrefabs;


    private void Awake()
    {
        grid = transform.Find("Grid");
        //player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

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
        Debug.Log($"Loading level {maxLevelReached}");
        if (maxLevelReached > levelPrefabs.Count) maxLevelReached = 1;
        GameObject newLevel = Instantiate(levelPrefabs[maxLevelReached - 1], grid);
    }
  

    public void ReachedGoal()
    {
        currentLevel++;
        maxLevelReached++; Math.Clamp(maxLevelReached, 1, levelPrefabs.Count-1);
        if (maxLevelReached > levelPrefabs.Count) gameManager.SetGameState(GameState.WinScreen);
        else gameManager.SetGameState(GameState.NextLevelScreen);
    }
    public void PlayerDie() => StartCoroutine(DelayedPlayerDie());
    private IEnumerator DelayedPlayerDie()
    {
        yield return new WaitForSeconds(.75f);
        gameManager.SetGameState(GameState.LoseScreen);
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
