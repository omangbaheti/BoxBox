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
        isTouchingWall = true;
    }
    
    private void OnTriggerStay(Collider other)
    {
        isTouchingWall = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isTouchingWall = false;
    }
}
