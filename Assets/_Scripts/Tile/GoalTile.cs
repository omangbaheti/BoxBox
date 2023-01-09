using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTile : MonoBehaviour
{
    [SerializeField] private bool isTileTriggered;
    public event Action OnTileTriggered;
    public bool[] isRaycastHit = new bool[4];
    public bool IsTileTriggered => isTileTriggered;

    private void Update()
    {
        CheckGoal();
    }

    private void CheckGoal()
    {
        isTileTriggered = false;
        for(int i = 0; i < isRaycastHit.Length; i++)
        {
            var raycastChild = transform.GetChild(i);
            Ray upwardRay = new Ray(raycastChild.position, Vector3.up);
            Debug.DrawRay(raycastChild.position, Vector3.up * 0.4f, Color.red);
            if (Physics.Raycast(upwardRay, out RaycastHit hitInfo, 0.4f))
            {
                if (hitInfo.transform.CompareTag("Player"))
                {
                    isRaycastHit[i] = true;
                }
                else
                {
                    isRaycastHit[i] = false;
                }
            }
            else
            {
                isRaycastHit[i] = false;
            }
        }

        foreach (bool touch in isRaycastHit)
        {
            if(!touch) return;
        }

        isTileTriggered = true;
        OnTileTriggered?.Invoke();
    }
}
