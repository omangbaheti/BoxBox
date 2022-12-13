using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool touchMode;
    public static event Action<Vector2> SwipeAction;
    public static event Action TapAction;

    [Range(0, 50)][SerializeField] private float swipeRange;
    [Range(0, 50)] [SerializeField] private float tapRange;

    private Vector2 startTouchPosition;
    private Vector2 currentPosition;
    private Vector2 endTouchPosition;
    
    private bool stopTouch = false;
    
    void Update()
    {
        if(touchMode)
            Swipe();
        else
            DebugTouch();
    }

    private void Swipe()
    {
        if (Input.touchCount <= 0) return;
        Touch primaryTouch = Input.GetTouch(0);
        switch (primaryTouch.phase)
        {
            case TouchPhase.Began:
            {
                startTouchPosition = primaryTouch.position;
                break;
            }
            case TouchPhase.Moved:
            {
                currentPosition = primaryTouch.position;
                Vector2 quickDistance = currentPosition - startTouchPosition;
                if (stopTouch) return;
            
                if (quickDistance.sqrMagnitude > swipeRange * swipeRange)
                {
                    SwipeAction?.Invoke(quickDistance.normalized);
                    stopTouch = true;
                }
                break;
            }
            case TouchPhase.Ended:
                stopTouch = false;
                endTouchPosition = Input.GetTouch(0).position;
                Vector2 finalDistance = endTouchPosition - startTouchPosition;
                if(finalDistance.sqrMagnitude<tapRange*tapRange)
                    TapAction?.Invoke();
                break;
        }
    }

    private void DebugTouch()
    {
    #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            SwipeAction?.Invoke(Vector2.left);
            return;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SwipeAction?.Invoke(Vector2.right);
            return;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SwipeAction?.Invoke(Vector2.up);
            return;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SwipeAction?.Invoke(Vector2.down);
        }
    #endif
    }
}
