using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform currentPivot;
    [SerializeField] private Transform[] pivots = new Transform[4];
    
    private IEnumerator currentCoroutine;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private bool canMove = true;
    private bool isCorrectingPosition = false;
    private Dictionary<PivotPlacement, Transform> getPivot = new Dictionary<PivotPlacement, Transform>();
    private Pose cachedPose = new Pose();
    private Vector3 inputVector3D;
    private Vector2 touchPosition;
    private Vector2 worldToScreenPosition;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        ConfigurePivots();
        Vector3 debugRay = Camera.main.WorldToScreenPoint(transform.position);
        Debug.Log(debugRay);
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
        Move(inputVector3D);
    }

    private void Move(Vector3 input)
    {
        if(!canMove) return;
        canMove = false;
        worldToScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 pivotPosition = FindPivotPosition(input);
        float rotationAngle = DetermineRotationAngle(input);
        if (pivotPosition == Vector3.zero)
        {
            canMove = true;
            return;
        }
        currentCoroutine = RotateBodyAroundPivot(pivotPosition, 0.2f, rotationAngle);
        StartCoroutine(currentCoroutine);
    }
    
    private Vector3 FindPivotPosition(Vector3 input)
    {
        float tolerance = 200;
        if (input == Vector3.right)
        {
            var topRightPivot = getPivot[PivotPlacement.TopRight];
            var bottomRightPivot = getPivot[PivotPlacement.BottomRight];
            bool selectTopPivot = touchPosition.y < worldToScreenPosition.y &&  Mathf.Abs(touchPosition.y - worldToScreenPosition.y) < tolerance; // although it doesnt make sense for touch < worldscreen, that is how users intuitively interact with the cube
            return SelectPivot(topRightPivot, bottomRightPivot, selectTopPivot);
        }
        if (input == Vector3.left)
        {
            var topLeftPivot = getPivot[PivotPlacement.TopLeft];
            var bottomLeftPivot = getPivot[PivotPlacement.BottomLeft];
            bool selectTopPivot = touchPosition.y < worldToScreenPosition.y &&  Mathf.Abs(touchPosition.y - worldToScreenPosition.y) < tolerance;
            return SelectPivot(topLeftPivot, bottomLeftPivot, selectTopPivot);
        }
        if (input == Vector3.forward)
        {
            var topLeftPivot = getPivot[PivotPlacement.TopLeft];
            var topRightPivot = getPivot[PivotPlacement.TopRight];
            bool selectRightPivot = touchPosition.x < worldToScreenPosition.x &&  Mathf.Abs(touchPosition.x - worldToScreenPosition.x) < tolerance;
            return SelectPivot(topRightPivot, topLeftPivot, selectRightPivot);
        }
        if (input == Vector3.back)
        {
            var bottomLeftPivot = getPivot[PivotPlacement.BottomLeft];
            var bottomRightPivot = getPivot[PivotPlacement.BottomRight];
            bool selectRightPivot =  touchPosition.x < worldToScreenPosition.x &&  Mathf.Abs(touchPosition.x - worldToScreenPosition.x) < tolerance;
            return SelectPivot(bottomRightPivot, bottomLeftPivot, selectRightPivot);
        }
        return Vector3.zero;
    }

    private float DetermineRotationAngle(Vector3 input)
    {
        float rotationAngle = 90f;
        if (currentPivot == null) return 0f;
        var currentPivotPlacement = currentPivot.GetComponent<Pivot>().cornerPlacement;
        if (input == Vector3.right)
        {
            if (currentPivotPlacement == PivotPlacement.BottomRight)
                rotationAngle = 90f;
            else if (currentPivotPlacement == PivotPlacement.TopRight)
                rotationAngle = -90f;
            return rotationAngle;
        }

        if (input == Vector3.left)
        {
            if (currentPivotPlacement == PivotPlacement.BottomLeft)
                rotationAngle = -90f;
            else if (currentPivotPlacement == PivotPlacement.TopLeft)
                rotationAngle = 90f;
            return rotationAngle;
        }

        if (input == Vector3.forward)
        {
            if (currentPivotPlacement == PivotPlacement.TopRight)
                rotationAngle = 90f;
            else if (currentPivotPlacement == PivotPlacement.TopLeft)
                rotationAngle = -90f;
            return rotationAngle;
        }

        if (input == Vector3.back)
        {
            if (currentPivotPlacement == PivotPlacement.BottomRight)
                rotationAngle = -90f;
            else if (currentPivotPlacement == PivotPlacement.BottomLeft)
                rotationAngle = 90f;
            return rotationAngle;
        }

        return 0f;
    }
    
    private Vector3 SelectPivot(Transform pivot1, Transform pivot2, bool bigBrainBool)
    {
        var pivot1Data = pivot1.GetComponent<Pivot>();
        var pivot2Data = pivot2.GetComponent<Pivot>();
        if (pivot1Data.isTouchingWall && pivot2Data.isTouchingWall)
        {
            Ray sidewardRay = new Ray(transform.position, inputVector3D);
            Physics.Raycast(sidewardRay, out RaycastHit wallAdjacent, 0.1f);
            if (wallAdjacent.transform != null)
            {
                return Vector3.zero;
            }

            if (bigBrainBool)
            {
                currentPivot = pivot1;
                return pivot1.position;
            }
            else
            {
                currentPivot = pivot2;
                return pivot2.position;
            }
        }
        if(pivot1Data.isTouchingWall)
        {
            currentPivot = pivot1;
            return pivot1.position;
        }
        if (pivot2Data.isTouchingWall)
        {
            currentPivot = pivot2;
            return pivot2.position;
        }
        return Vector3.zero;    
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
            transform.RotateAround(position, Vector3.up, rotationAngle);
            yield return null;
        }
        ConfigurePivots();
        canMove = true;
        yield return null;
    }
    
    private void ConfigurePivots()
    {
        foreach (Transform pivot in pivots)
        {
            var pivotData = pivot.GetComponent<Pivot>();
            if (pivot.position.x > transform.position.x)
            {
                if (pivot.position.z > transform.position.z)
                {
                    getPivot[PivotPlacement.TopRight] = pivot;
                    pivotData.cornerPlacement = PivotPlacement.TopRight;
                }
                else
                {
                    getPivot[PivotPlacement.BottomRight] = pivot;
                    pivotData.cornerPlacement = PivotPlacement.BottomRight;
                }
            }
            else
            {
                if (pivot.position.z > transform.position.z)
                {
                    getPivot[PivotPlacement.TopLeft] = pivot;
                    pivotData.cornerPlacement = PivotPlacement.TopLeft;
                    
                }
                else
                {
                    getPivot[PivotPlacement.BottomLeft] = pivot;
                    pivotData.cornerPlacement = PivotPlacement.BottomLeft;
                }
            }
        }
    }

}

public enum PivotPlacement
{
    TopLeft = 0,
    TopRight = 1,
    BottomLeft = 2,
    BottomRight = 3
}

