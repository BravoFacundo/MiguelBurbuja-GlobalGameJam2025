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
    [SerializeField] Transform playerPos;
    [SerializeField] Vector3 PlayerPool;
    [SerializeField] Transform grid;

    [Header("Levels")]    
    [SerializeField] List<GameObject> levelPrefabs;

    private void Awake()
    {
        playerPos = player.transform;
        SendPlayerToPool();
    }

    public void SendPlayerToPool()
    {
        player.canMove = false;
        playerPos.position = PlayerPool;
    }
    private IEnumerator SendPlayerToGrid()
    {
        yield return new WaitForSeconds(1f);
        Transform playerPivot = grid.Find(grid.GetChild(0).name + "/PlayerPivot");
        playerPos.position = playerPivot.position;
        Destroy(playerPivot.gameObject);
        player.canMove = true;
    }

    public void LoadNextLevel()
    {
        Utilities.DeleteAllChildrens(grid);
        if (maxLevelReached > levelPrefabs.Count) maxLevelReached = 1;
        GameObject newLevel = Instantiate(levelPrefabs[maxLevelReached - 1], grid);
        StartCoroutine(SendPlayerToGrid());
    }

    public void PlayerReachedGoal()
    {
        SendPlayerToPool();
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
