using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string nextLevelName;
    [SerializeField] private GoalTile[] goalTiles;
    void Start()
    {
        goalTiles = GameObject.FindObjectsOfType<GoalTile>();
        foreach (GoalTile tile in goalTiles)
        {
            tile.OnTileTriggered += TriggerPlayerWinCoroutine;
        }
    }
    

    private void TriggerPlayerWinCoroutine()
    {
        StartCoroutine(CheckPlayerWin());
    }

    private IEnumerator CheckPlayerWin()
    {
        yield return new WaitForSeconds(1.2f);
        foreach (GoalTile tile in goalTiles)
        {
            if (!tile.IsTileTriggered)
                break;
        }
        Debug.Log("PlayerWin");
        TriggerLevelChange(nextLevelName);
    }
    
    
    public void TriggerLevelChange(string nextLevel)
    {
        SceneManager.LoadScene(nextLevel);
    }

    private void OnDestroy()
    {
        foreach (GoalTile tile in goalTiles)
        {
            tile.OnTileTriggered -= TriggerPlayerWinCoroutine;
        }
    }
}
