using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static Action OnPlayerWin;
    private int moveCounter = 0;
    [SerializeField] private int[] targetMoves = new []{0,0,0};
    [SerializeField] private string nextLevelName;
    [SerializeField] private GoalTile[] goalTiles;
    [SerializeField] private GameObject StarsUI;
    void Start()
    {
        goalTiles = GameObject.FindObjectsOfType<GoalTile>();
        foreach (GoalTile tile in goalTiles)
        {
            tile.OnTileTriggered += TriggerPlayerWinCoroutine;
        }
        StarsUI = GameObject.Find("Stars");
    }

    private void OnEnable()
    {
        InputManager.SwipeAction += MoveActionOnPerformed;
    }

    private void MoveActionOnPerformed(Vector2 obj)
    {
        moveCounter++;
        RecalculateStars();
    }

    private void RecalculateStars()
    {
        for (int i = 0; i < targetMoves.Length; i++)
        {
            if (moveCounter > targetMoves[i])
            {
                Debug.Log("triggered");
                var currentStar = StarsUI.transform.GetChild(i);
                var starSprite = currentStar.GetComponent<RectTransform>();
                LeanTween.alpha(starSprite, 0f, 0.35f).setEaseInQuad();
            }
        }
    }

    private void OnDisable()
    {
        InputManager.SwipeAction -= MoveActionOnPerformed;
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
        OnPlayerWin.Invoke();
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
