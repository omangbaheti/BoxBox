using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTile : MonoBehaviour
{
    [SerializeField] private bool isTileTriggered;

    public bool IsTileTriggered => isTileTriggered;

    public Action OnTileTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isTileTriggered = true;
            OnTileTriggered?.Invoke();
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isTileTriggered = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isTileTriggered = false;
            OnTileTriggered?.Invoke();
        }
    }
    
    
}
