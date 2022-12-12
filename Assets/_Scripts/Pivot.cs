using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot : MonoBehaviour
{
    public PivotPlacement pivotPosition;
    public bool isValidMove;

    private void OnTriggerEnter(Collider other)
    {
        isValidMove = false;
    }
    
    private void OnTriggerStay(Collider other)
    {
        isValidMove = false;
    }

    private void OnTriggerExit(Collider other)
    {
        isValidMove = true;
    }
}
