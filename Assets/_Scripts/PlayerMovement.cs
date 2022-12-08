using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Vector3 inputValues;
    public Vector3 scaledAngle;

    [SerializeField] private Vector2 boxDimensions;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private bool canMove = true;
    [SerializeField] private List<Vector3> pivots;


    // Start is called before the first frame update
    void Start()
    {
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
        Vector3 inputVector3D = new Vector3(inputVector.x, 0f, inputVector.y);
        StartCoroutine(AnimateMove(inputVector3D));
    }

    private IEnumerator AnimateMove(Vector3 input)
    {
        if(!canMove) yield break;
        if(input.magnitude > 1) yield break;
        transform.DOMove(transform.position + input, 0.1f);
        yield return transform.DORotate(scaledAngle, 0.1f, RotateMode.WorldAxisAdd).SetRelative().WaitForCompletion();
        canMove = true;

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMove(InputValue value)
    {
       
    }

    
}
