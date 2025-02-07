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

    /// <summary>
    /// desabilita el player y lo mueve a la pool
    /// </summary>
    public void SendPlayerToPool()
    {
        player.canMove = false;
        playerPos.position = PlayerPool;
    }
    /// <summary>
    /// envia al player pivot del grid despues de un delay 
    /// </summary>
    private IEnumerator SendPlayerToGrid()
    {
        yield return new WaitForSeconds(1f);
        Transform playerPivot = grid.Find(grid.GetChild(0).name + "/PlayerPivot");
        playerPos.position = playerPivot.position;
        Destroy(playerPivot.gameObject);
        player.canMove = true;
    }
    /// <summary>
    /// limpia el grid y carga el siguiente nivel ..... si no hay mas niveles carga el win screen
    /// </summary>
    public void LoadLevel(int levelIndex)
    {
        Utilities.DeleteAllChildrens(grid);
        if (maxLevelReached > levelPrefabs.Count) maxLevelReached = 1;
        GameObject newLevel = Instantiate(levelPrefabs[levelIndex], grid);
        StartCoroutine(SendPlayerToGrid());
    }
    /// <summary>
    /// envia al player a la pool, suma al lvl y carga el siguiente nivel
    /// </summary>
    public void PlayerReachedGoal()
    {
        SendPlayerToPool();
        currentLevel++;
        maxLevelReached = Mathf.Clamp(maxLevelReached + 1, 1, levelPrefabs.Count);
        if (maxLevelReached > levelPrefabs.Count) gameManager.SetGameState(GameState.WinScreen);
        else gameManager.SetGameState(GameState.NextLevelScreen);
    }
    /// <summary>
    /// despues de un delay carga el lose screen...
    /// </summary>
    public IEnumerator PlayerDie()
    {
        yield return new WaitForSeconds(.75f);
        gameManager.SetGameState(GameState.LoseScreen);
    }

}
