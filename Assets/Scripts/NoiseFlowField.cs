using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFlowField : MonoBehaviour
{
    FastNoise fastNoise;
    public Vector3Int gridSize;
    public float increment;
    public Vector3 offset, offsetSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        fastNoise = new FastNoise();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0;y <gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {

                }
            }
        }
    }
}
