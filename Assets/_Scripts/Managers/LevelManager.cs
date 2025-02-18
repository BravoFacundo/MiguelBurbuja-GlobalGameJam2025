using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

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
    private void OnDisable() => SendPlayerToPool();

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
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        Destroy(playerPivot.gameObject);
        player.canMove = true;
    }

    public void LoadLevel(int levelNumber)
    {
        int levelIndex = levelNumber - 1;
        Utilities.DeleteAllChildrens(grid);
        Instantiate(levelPrefabs[levelIndex], grid);
        StartCoroutine(SendPlayerToGrid());
    }

    public void PlayerReachedGoal()
    {
        SendPlayerToPool();
        int currentLevel = gameManager.playerData.currentLevel;
        if (currentLevel == levelPrefabs.Count) gameManager.SetGameState(GameState.Win);        
        else
        {
            gameManager.playerData.currentLevel++;
            gameManager.SetGameState(GameState.Game);            
        }
    }
    public IEnumerator PlayerDie()
    {
        yield return new WaitForSeconds(.75f);
        gameManager.SetGameState(GameState.Lose);
    }

}
