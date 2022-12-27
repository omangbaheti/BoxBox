using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Image swipeImage;
    private bool tutorialComplete = false;
    private bool swipeUpTutorial = false;
    private bool isReady = true;
    private RectTransform _rectTransform;
    private float offset = -300;
    void Start()
    {
        transform.position = startPoint.position;
        _rectTransform = GetComponent<RectTransform>();
        StartCoroutine(StartTween());
        InputManager.SwipeAction += SetSwipeUp;
    }

    private void Update()
    {
        transform.position = swipeUpTutorial ? new Vector2(startPoint.position.x, startPoint.position.y + offset) 
                                                     : new Vector2(startPoint.position.x + offset, startPoint.position.y);
        swipeImage.transform.rotation = swipeUpTutorial
            ? Quaternion.Euler(0, 0, 0)
            : Quaternion.Euler(0, 0, 90); 
        if(tutorialComplete) {gameObject.SetActive(false); swipeImage.gameObject.SetActive(false);}
    }

    private IEnumerator StartTween()
    {
        while (!tutorialComplete)
        {
            yield return DOTween.To(() => offset, newPos => offset = newPos, 300, 1.2f).SetEase(Ease.OutQuad).WaitForCompletion();
            offset = -300f;
        }

        
    }

    private void SetSwipeUp(Vector2 input)
    {
        if (input == Vector2.right)
        {
            swipeUpTutorial = true;
        }

        if (swipeUpTutorial && input == Vector2.up)
        {
            tutorialComplete = true;
        }
    }
    
    

    
}
