using UnityEngine;
using UnityEngine.Serialization;

public class Pivot : MonoBehaviour
{
    [FormerlySerializedAs("pivotPosition")] public PivotPlacement cornerPlacement;
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
