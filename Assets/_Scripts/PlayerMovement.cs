using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{ 
    [SerializeField] private Transform[] pivots = new Transform[4];
    private PlayerInput playerInput;
    private InputAction moveAction;
    private bool canMove = true;
    private Dictionary<PivotPlacement, Transform> getPivot = new Dictionary<PivotPlacement, Transform>();

    // Start is called before the first frame update
    void Start()
    {
        ConfigurePivots();
    }

    private void OnEnable()
    {
        InputManager.SwipeAction += MoveActionOnPerformed;
    }

    private void Update()
    {
        
    }

    private void OnDisable()
    {
        InputManager.SwipeAction -= MoveActionOnPerformed;
    }
    private void MoveActionOnPerformed(Vector2 input)
    {
        Vector3 inputVector3D = new(input.x, 0f, input.y);
        StartCoroutine(AnimateMove(inputVector3D));
    }
    

    private IEnumerator AnimateMove(Vector3 input)
    {
        if(!canMove) yield break;
        if(input.magnitude > 1) yield break;
        canMove = false;
        Vector3 pivotPosition = CalculatePivotPosition(input);
        float rotationAngle = 90f;
        if (input.x is > 0 or < 0) rotationAngle *= Math.Sign(input.x);
        if (input.z is > 0 or < 0) rotationAngle *= Math.Sign(input.z);
        StartCoroutine(RotateBodyAroundPivot(pivotPosition, 0.1f, rotationAngle));
        canMove = true;
    }

    private IEnumerator RotateBodyAroundPivot(Vector3 position, float transitionTime, float angle)
    {
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
        yield return new WaitForSeconds(0.1f);
    }

    private Vector3 CalculatePivotPosition(Vector3 input)
    {
        if (input == Vector3.right)
            return getPivot[PivotPlacement.BottomRight].position;
        if (input == Vector3.left)
            return getPivot[PivotPlacement.BottomLeft].position;
        if(input == Vector3.forward)
            return getPivot[PivotPlacement.TopRight].position;
        if(input == Vector3.back)
            return getPivot[PivotPlacement.BottomRight].position;
        return  getPivot[PivotPlacement.BottomRight].position;
    }

    private void ConfigurePivots()
    {
        Debug.Log(this.transform.position);
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

