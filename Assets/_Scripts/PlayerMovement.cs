using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform currentPivot;
    [SerializeField] private Transform[] pivots = new Transform[12];
    
    private IEnumerator currentCoroutine;
    private bool canMove = true;
    private Dictionary<PivotPlacement, Transform> getPivot = new Dictionary<PivotPlacement, Transform>();
    private bool isCorrectingPosition;
    private Pose cachedPose = new Pose();
    private Vector3 inputVector3D;
    private Vector2 touchPosition;
    private Vector2 worldToScreenPosition;
    [SerializeField] private bool isHori;
    [SerializeField] private bool isHorizontal => Mathf.RoundToInt(transform.eulerAngles.y) % 180 != 0;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        ConfigurePivots();
        Vector3 debugRay = Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnEnable()
    {
        InputManager.OnTouchBegan += UpdateScreenTouchPosition;
        InputManager.SwipeAction += MoveActionOnPerformed;
    }

    private void OnDisable()
    {
        InputManager.OnTouchBegan -= UpdateScreenTouchPosition;
        InputManager.SwipeAction -= MoveActionOnPerformed;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            StopCoroutine(currentCoroutine);
            StartCoroutine(OnInvalidPosition());
        }
    }
    
    private IEnumerator OnInvalidPosition()
    {
        canMove = false;  isCorrectingPosition = true;
        transform.DOMove(cachedPose.position, 0.2f);
        yield return transform.DORotate(cachedPose.rotation.eulerAngles, 0.2f, RotateMode.Fast).WaitForCompletion();
        ConfigurePivots();
        canMove = true; isCorrectingPosition = false;
    }

    private void UpdateScreenTouchPosition(Vector2 screenTouch)
    {
        touchPosition = screenTouch;
    }
    private void MoveActionOnPerformed(Vector2 input)
    {
        inputVector3D = new(input.x, 0f, input.y);
        ConfigurePivots();
        Move(inputVector3D);
    }

    private void Move(Vector3 input)
    {
        if(!canMove) return;
        canMove = false;
        worldToScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        //Debug.Log((touchPosition - worldToScreenPosition).magnitude);
        Vector3 pivotPosition = FindPivotPosition(input);
        float rotationAngle = DetermineRotationAngle(input);
        currentCoroutine = RotateBodyAroundPivot(pivotPosition, 0.1f, rotationAngle);
        StartCoroutine(currentCoroutine);
    }
    
    private Vector3 FindPivotPosition(Vector3 input)
    {
        if (input == Vector3.right)
        {
            if (!getPivot.ContainsKey(PivotPlacement.Right))
            {
                ConfigurePivots();
            }
            return getPivot[PivotPlacement.Right].position;
        }
        if (input == Vector3.left)
        {
            if (!getPivot.ContainsKey(PivotPlacement.Left))
            {
                ConfigurePivots();
            }
            return getPivot[PivotPlacement.Left].position;
        }
        if (input == Vector3.forward)
        {
            if (!getPivot.ContainsKey(PivotPlacement.Up))
            {
                ConfigurePivots();
            }
            return getPivot[PivotPlacement.Up].position;
        }
        if (input == Vector3.back)
        {
            if (!getPivot.ContainsKey(PivotPlacement.Down))
            {
                ConfigurePivots();
            }
            return getPivot[PivotPlacement.Down].position;
        }

        throw new InvalidOperationException();
    }

    private float DetermineRotationAngle(Vector3 input)
    {
        float rotationAngle = 90;
        if (input == Vector3.right)
        {
            return -rotationAngle;
        }
        if (input == Vector3.left)
        {
            return -rotationAngle;
        }
        if (input == Vector3.forward)
        {
            return -rotationAngle;
        }
        if (input == Vector3.back)
        {
            return -rotationAngle;
        }


        throw new InvalidOperationException();
    }
    

    private IEnumerator RotateBodyAroundPivot(Vector3 position, float transitionTime, float angle)
    {
        cachedPose.position = transform.position;
        cachedPose.rotation = transform.rotation;
        int animationSteps = Mathf.RoundToInt(transitionTime/Time.fixedDeltaTime);
        int currentStep = 0;
        while (currentStep < animationSteps)
        {
            currentStep += 1;
            float rotationAngle = angle * Time.fixedDeltaTime / transitionTime;
            transform.RotateAround(position, Vector3.Cross(inputVector3D, Vector3.up), rotationAngle);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        ConfigurePivots();
        canMove = true;
    }
    
    private void ConfigurePivots()
    {
        getPivot.Clear();
        foreach (Transform pivot in pivots)
        {
            var pivotData = pivot.GetComponent<Pivot>();
            if(!pivotData.isTouchingWall) {pivotData.cornerPlacement = PivotPlacement.NULL;continue;}
            
            if (pivot.position.x -transform.position.x > 0.01)
            {
                Debug.Log("RightPivotDetected");
                getPivot[PivotPlacement.Right] = pivot;
                pivotData.cornerPlacement = PivotPlacement.Right;
                continue;
            }
            if(transform.position.x - pivot.position.x > 0.01)
            {
                Debug.Log("LeftPivotDetected");
                getPivot[PivotPlacement.Left] = pivot;
                pivotData.cornerPlacement = PivotPlacement.Left;
                continue;
            }

            if (pivot.position.z - transform.position.z > 0.01)
            {
                Debug.Log("UpPivotDetected");
                getPivot[PivotPlacement.Up] = pivot;
                pivotData.cornerPlacement = PivotPlacement.Up;
                continue;
            }
            if (transform.position.z - pivot.position.z > 0.01)
            {
                Debug.Log("DownPivotDetected");
                getPivot[PivotPlacement.Down] = pivot;
                pivotData.cornerPlacement = PivotPlacement.Down;
                continue;
            }
        }
    }

}


public enum PivotPlacement
{
    NULL = -1,
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
}


