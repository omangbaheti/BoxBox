using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Pivot : MonoBehaviour
{
    [FormerlySerializedAs("pivotPosition")] public PivotPlacement cornerPlacement;
    public bool isTouchingWall = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if(other.CompareTag("Tile"))
            isTouchingWall = true;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Tile"))
            isTouchingWall = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Tile"))
            isTouchingWall = false;
    }
}
