using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    /// <summary>
    /// Direction around which to rotate.
    /// </summary>
    [SerializeField]
    private Vector3 direction = Vector3.up;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround (transform.position, direction, 30 * Time.deltaTime);
    }
}
