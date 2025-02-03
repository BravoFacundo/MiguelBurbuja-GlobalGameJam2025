using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
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
    }

    public void LoadNextLevel()
    {
        Utilities.DeleteAllChildrens(grid);
        if (maxLevelReached > levelPrefabs.Count) maxLevelReached = 1;
        GameObject newLevel = Instantiate(levelPrefabs[maxLevelReached - 1], grid);
    }

    public void PlayerReachedGoal()
    {
        currentLevel++;
        maxLevelReached = Mathf.Clamp(maxLevelReached + 1, 1, levelPrefabs.Count);
        if (maxLevelReached > levelPrefabs.Count) gameManager.SetGameState(GameState.WinScreen);
        else gameManager.SetGameState(GameState.NextLevelScreen);
    }
    public IEnumerator PlayerDie()
    {
        yield return new WaitForSeconds(.75f);
        gameManager.SetGameState(GameState.LoseScreen);
    }

}
