using UnityEngine;
using UnityEngine.Serialization;

public class Pivot : MonoBehaviour
{
    [FormerlySerializedAs("pivotPosition")] public PivotPlacement cornerPlacement;
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
