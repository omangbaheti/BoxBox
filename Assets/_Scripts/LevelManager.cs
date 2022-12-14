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
            tile.OnTileTriggered += CheckPlayerWin;
        }
    }

    private void TriggerLevelChange()
    {
        SceneManager.LoadScene(nextLevelName);
    }

    private void CheckPlayerWin()
    {
        foreach (GoalTile tile in goalTiles)
        {
            if (!tile.IsTileTriggered)
                return;
        }
        Debug.Log("PlayerWin");
        TriggerLevelChange();
    }

    private void OnDestroy()
    {
        foreach (GoalTile tile in goalTiles)
        {
            tile.OnTileTriggered -= CheckPlayerWin;
        }
    }
}
