using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot : MonoBehaviour
{
    public PivotPlacement pivotPosition;
    public bool isTouchingWall = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall"))
            isTouchingWall = true;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Wall"))
            isTouchingWall = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Wall"))
            isTouchingWall = false;
    }
}
