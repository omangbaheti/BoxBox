using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Vector3 inputValues;
    [SerializeField] private Vector2 boxDimensions;
    [SerializeField] private GameObject targetTile;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private bool canMove = true;
    private bool isHorizontal = false;
    [SerializeField] private CalculatePivots[] pivots;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(transform.rotation * transform.localScale);
        SetInputActions();
    }

    private void SetInputActions()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        moveAction.performed += MoveActionOnPerformed;
        
    }

    private void MoveActionOnPerformed(InputAction.CallbackContext input)
    {
        Vector2 inputVector = input.ReadValue<Vector2>();
        Vector3 inputVector3D = new(inputVector.x, 0f, inputVector.y);
        CheckValidity(inputVector3D);
    }

    private void CheckValidity(Vector3 input)
    {
        Ray downwardRay = new Ray(transform.position, Vector3.down);
        Physics.Raycast(downwardRay, out var tileHit, 0.5f);
        GameObject tile = tileHit.transform.gameObject;
        if (!tile.TryGetComponent(out Tile tileData)) return;
        targetTile = tileData.CheckTargetTiles(gameObject, input, isHorizontal);
        if(targetTile == null) return;
        StartCoroutine(AnimateMove(input));
    }

    private IEnumerator AnimateMove(Vector3 input)
    {
        if(!canMove) yield break;
        if(input.magnitude > 1) yield break;
        canMove = false;
        Vector3 targetPosition = new Vector3(targetTile.transform.position.x, transform.position.y, targetTile.transform.position.z);
        Vector3 rotationAngle = isHorizontal ?new Vector3(0, 90, 0) :Vector3.zero ;
        isHorizontal = !isHorizontal;
        if (!Mathf.Approximately(input.x, 0f)) 
            rotationAngle *= input.x;
        else 
            rotationAngle *= input.z;
        transform.DOMove( targetPosition , 0.1f);
        yield return transform.DORotate(rotationAngle, 0.1f, RotateMode.Fast).WaitForCompletion();
        canMove = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    

    
}
