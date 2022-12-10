using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Vector3[] directions;
    private Dictionary<Vector3, Vector3> inputToDirection;
    [SerializeField] private List<GameObject> tiles;
    private void Start() 
    {
        directions = new Vector3[] { transform.right, 
                                    -transform.right, 
                                     transform.forward, 
                                    -transform.forward};
        inputToDirection = new Dictionary<Vector3, Vector3>()
        {
            { Vector3.right, transform.right },
            { Vector3.left, -transform.right },
            { Vector3.forward, transform.forward },
            { Vector3.back, -transform.forward }
        };

        foreach (Vector3 direction in directions)
        {
            GameObject adjacentTile = ReturnAdjacentTile(direction);
            if(adjacentTile != null) tiles.Add(adjacentTile);
        }
    }

    public GameObject CheckTargetTiles(GameObject playerController, Vector3 input, bool isHorizontal)
    {
        Vector3 cubeScale = playerController.transform.localScale;
        Vector3Int tileDimensions = isHorizontal ? new(Mathf.RoundToInt(cubeScale.x), 0, Mathf.RoundToInt(cubeScale.z)) : new(Mathf.RoundToInt(cubeScale.z), 0, Mathf.RoundToInt(cubeScale.x));
        Debug.Log(tileDimensions);
        int tileLength = !Mathf.Approximately(input.x, 0)? tileDimensions.x: tileDimensions.z;
        GameObject landingTile = ReturnLandingTile(tileLength, input);
        return landingTile;
        for (int i = 0; i < tileDimensions.x; i++)
        {
            for (int j = 0; i < tileDimensions.z; j++)
            {
                //TODO : verify tiles around landing tile
            }
        }
        
    }

    public GameObject ReturnLandingTile(int tileLength, Vector3 input)
    {
        Tile tileIterator = this;
        GameObject landingTile = new GameObject();
        for (int i = 0; i < tileLength; i++)
        {
            landingTile = tileIterator.ReturnAdjacentTile(inputToDirection[input]);
            tileIterator = landingTile.GetComponent<Tile>();
        }
        return landingTile;
    }

    private GameObject ReturnAdjacentTile(Vector3 direction)
    {
        Ray directionalRay = new Ray(transform.position, direction);
        Physics.Raycast(directionalRay, out var tileHit, 1.1f);
        if (tileHit.transform == null) return null;
        return tileHit.transform.gameObject;
    }
    
}
