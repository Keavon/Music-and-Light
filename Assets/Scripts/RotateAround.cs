using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{

    /// <summary>
    /// Center the object was spawned around.
    /// </summary>
    private Vector3 center;

    /// <summary>
    /// Direction the object is moving.
    /// </summary>
    private Vector3 direction;

    /// <summary>
    /// Which direction the obejct should rotate.
    /// </summary>
    [SerializeField]
    private bool rotateClockwise = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (center != null) {
            transform.RotateAround (center, direction, 30 * Time.deltaTime * (rotateClockwise ? -1 : 1));
        }
    }

    /// <summary>
    /// Set the center for the object
    /// </summary>
    /// <param name="dir">Direction the object is moving in.</param>
    /// <param name="cen">Center the object was spawned around.</param>
    public void SetDirectionAndCenter (Vector3 dir, Vector3 cen) {
        direction = dir;
        center = cen;
    }

    /// <summary>
    /// Update the center position.
    /// </summary>
    /// <param name="cen">Updated center position.</param>
    public void UpdateCenter (Vector3 cen) {
        center = cen;
    }
}
