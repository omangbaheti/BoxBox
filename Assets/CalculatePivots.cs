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
        { Corner.BottomLeft ,  new Vector3(-1,0,-1)},
        { Corner.BottomRight , new Vector3(1,0,-1)},
        { Corner.TopLeft ,     new Vector3(-1,0,1)},
        { Corner.TopRight,     new Vector3(1,0, 1)}
    };
    void Start()
    {
        transform.position = playerController.transform.rotation * (playerController.transform.position + Vector3.Scale(cornerToPosition[corner], playerController.transform.localScale/2));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = CalculatePivot();
    }

    private Vector3 CalculatePivot()
    {
        Vector3 correctedCornerPosition = playerController.transform.rotation * cornerToPosition[corner];
        Vector3 cornerPosition = Vector3.Scale(correctedCornerPosition, playerController.transform.localScale / 2);
        Vector3 positionRelativeToPlayer = playerController.transform.position + cornerPosition;
        //positionRelativeToPlayer = playerController.transform.rotation * positionRelativeToPlayer;
        return positionRelativeToPlayer;
    }
}
