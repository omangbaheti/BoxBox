using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public enum Corner
{
    NULL = -1,
    BottomLeft = 0,
    BottomRight = 1,
    TopLeft = 2,
    TopRight = 3
}

public class CalculatePivots : MonoBehaviour
{
    [SerializeField] private GameObject playerController;
    [SerializeField] private Corner corner;
    [SerializeField] private Dictionary<Corner, Vector3> cornerToPosition = new Dictionary<Corner, Vector3>()
    {
        { Corner.BottomLeft, new Vector3(-1, 0, -1) },
        { Corner.BottomRight, new Vector3(1, 0, -1) },
        { Corner.TopLeft, new Vector3(-1, 0, 1) },
        { Corner.TopRight, new Vector3(1, 0, 1) }
    };

    void Start()
    {
        Debug.Log(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        AssignPivotCorner();
    }

    private void AssignPivotCorner()
    {
        Vector3 playerPosition = playerController.transform.position;
        if (transform.position.x < playerPosition.x)
            corner = transform.position.z < playerPosition.z ? Corner.BottomLeft : Corner.TopLeft;
        else
            corner = transform.position.z < playerPosition.z ? Corner.BottomRight : Corner.TopRight;
    }
}
