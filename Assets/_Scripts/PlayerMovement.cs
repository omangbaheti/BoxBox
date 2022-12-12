using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PivotPlacement currentPivot;
    [SerializeField] private Transform[] pivots = new Transform[4];
    
    private PlayerInput playerInput;
    private InputAction moveAction;
    private bool canMove = true;
    private Dictionary<PivotPlacement, Transform> getPivot = new Dictionary<PivotPlacement, Transform>();
    private Pose cachedPose = new Pose();

    // Start is called before the first frame update
    void Start()
    {
        ConfigurePivots();
    }

    private void OnEnable()
    {
        InputManager.SwipeAction += MoveActionOnPerformed;
    }

    private void OnDisable()
    {
        InputManager.SwipeAction -= MoveActionOnPerformed;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided");
        if (collision.gameObject.CompareTag("Wall"))
            StartCoroutine(OnInvalidPosition());
    }
    
    private IEnumerator OnInvalidPosition()
    {
        canMove = false;
        transform.DOMove(cachedPose.position, 0.2f);
        yield return transform.DORotate(cachedPose.rotation.eulerAngles, 0.2f, RotateMode.Fast);
        canMove = true;
    }
    private void MoveActionOnPerformed(Vector2 input)
    {
        Vector3 inputVector3D = new(input.x, 0f, input.y);
        Move(inputVector3D);
    }

    private void Move(Vector3 input)
    {
        if(!canMove) return;
        canMove = false;
        float rotationAngle = 90f;
        
        if (input == Vector3.right || input == Vector3.left)
        {
            rotationAngle *= Math.Sign(input.x);
        }
        else if (input == Vector3.forward || input == Vector3.back)
        {
            rotationAngle *= Math.Sign(-1 * input.z);
        }
        else
        {
            canMove = true;
            return;
        }
        Vector3 pivotPosition = FindPivotPosition(input);
        if (pivotPosition == Vector3.zero)
        {
            canMove = true;
            return;
        }
        StartCoroutine(RotateBodyAroundPivot(pivotPosition, 0.2f, rotationAngle));
    }
    
    private Vector3 FindPivotPosition(Vector3 input)
    {
        Debug.Log(input);
        if (input == Vector3.right)
        {
            var topRightPivot = getPivot[PivotPlacement.TopRight];
            var bottomRightPivot = getPivot[PivotPlacement.BottomRight];
            return SelectPivot(topRightPivot, bottomRightPivot);
        }
        if (input == Vector3.left)
        {
            var topLeftPivot = getPivot[PivotPlacement.TopLeft];
            var bottomLeftPivot = getPivot[PivotPlacement.BottomLeft];
            return SelectPivot(topLeftPivot, bottomLeftPivot);
        }
        if (input == Vector3.forward)
        {
            var topLeftPivot = getPivot[PivotPlacement.TopLeft];
            var topRightPivot = getPivot[PivotPlacement.TopRight];
            return SelectPivot(topLeftPivot, topRightPivot);
        }
        if (input == Vector3.back)
        {
            var bottomLeftPivot = getPivot[PivotPlacement.BottomLeft];
            var bottomRightPivot = getPivot[PivotPlacement.BottomRight];
            return SelectPivot(bottomLeftPivot, bottomRightPivot);
        }
        return Vector3.zero;;
    }
    
    private Vector3 SelectPivot(Transform pivot1, Transform pivot2)
    {
        var pivot1Data = pivot1.GetComponent<Pivot>();
        var pivot2Data = pivot2.GetComponent<Pivot>();
        if (pivot1Data.isTouchingWall && pivot2Data.isTouchingWall)
        {
            // trigger failure
        }
        else if(pivot1Data.isTouchingWall)
        {
            currentPivot = pivot1Data.pivotPosition;
            return pivot1.position;
        }
        else if (pivot2Data.isTouchingWall)
        {
            currentPivot = pivot2Data.pivotPosition;
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
                    //Debug.Log("TopRight");
                    getPivot[PivotPlacement.TopRight] = pivot;
                    pivotData.pivotPosition = PivotPlacement.TopRight;
                }
                else
                {
                    //Debug.Log("BottomRight");
                    getPivot[PivotPlacement.BottomRight] = pivot;
                    pivotData.pivotPosition = PivotPlacement.BottomRight;
                }
            }
            else
            {
                if (pivot.position.z > transform.position.z)
                {
                    //Debug.Log("TopLeft");
                    getPivot[PivotPlacement.TopLeft] = pivot;
                    pivotData.pivotPosition = PivotPlacement.TopLeft;
                    
                }
                else
                {
                    //Debug.Log("BottomLeft");
                    getPivot[PivotPlacement.BottomLeft] = pivot;
                    pivotData.pivotPosition = PivotPlacement.BottomLeft;
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

