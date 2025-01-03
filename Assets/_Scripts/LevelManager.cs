using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static event Action OnPlayerWin;
    private int moveCounter = 0;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private int[] targetMoves = new []{0,0,0};
    [SerializeField] private int nextLevelIndex   ;
    [SerializeField] private GoalTile[] goalTiles;
    [SerializeField] private GameObject StarsUI;
    void Start()
    {
        goalTiles = FindObjectsOfType<GoalTile>();
        foreach (GoalTile tile in goalTiles)
        {
            tile.OnTileTriggered += TriggerPlayerWinCoroutine;
        }
        winScreen.gameObject.SetActive(false);
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
                var currentStar = StarsUI.transform.GetChild(i);
                var starSprite = currentStar.GetComponent<Image>(); 
                starSprite.DOFade(0, 0.35f).SetEase(Ease.InQuad); ;
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
        bool isWin = false;
        new WaitForSeconds(0.4f);
        foreach (GoalTile tile in goalTiles)
        {
            if (!tile.IsTileTriggered)
            {
                isWin = false;
                break;
            }

            isWin = true;
        }
        Debug.Log("PlayerWin");
        if (isWin)
        {
            OnPlayerWin?.Invoke();
            winScreen.gameObject.SetActive(true);
        }

        yield return null;
    }
    
    
    public void TriggerLevelChange()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        InputManager.SwipeAction -= MoveActionOnPerformed;
        foreach (GoalTile tile in goalTiles)
        {
            tile.OnTileTriggered -= TriggerPlayerWinCoroutine;
        }
    }
}
