using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{

    /// <summary>
    /// Direction for the game object to move in.
    /// </summary>
    private Vector3 direction;

    /// <summary>
    /// Distance to move this frame.
    /// </summary>
    private double distanceToMove;

    /// <summary>
    /// total distance that the object has moved.
    /// </summary>
    private double totalDistanceMoved;

    /// <summary>
    /// Center the object was spawned around
    /// </summary>
    private Vector3 center;

    /// <summary>
    /// RotateAround component
    /// </summary>
    private RotateAround rotateAround;

    // Start is called before the first frame update
    void Start()
    {
        // rotateAround = gameObject.GetComponent<RotateAround> ();
        // if (rotateAround == null) {
        //     gameObject.AddComponent<RotateAround> ();
        //     rotateAround = gameObject.GetComponent<RotateAround> ();
        // }

        // rotateAround.SetDirectionAndCenter (direction, center);
    }

    // Update is called once per frame
    void Update()
    {
        if (direction != null) {
            distanceToMove = (direction * Constants.objectSpeedMultiplier * Time.deltaTime).magnitude;
            totalDistanceMoved += distanceToMove;
            transform.position += direction * Constants.objectSpeedMultiplier * Time.deltaTime;
            center += direction * Constants.objectSpeedMultiplier * Time.deltaTime;
            // rotateAround.UpdateCenter (center);
        }

        if (totalDistanceMoved > Constants.tunnelLength * 1.1) {
            GameObject.Destroy (gameObject);
        }
    }

    /// <summary>
    /// Set the direction for the game object to move in.
    /// </summary>
    /// <param name="dir">Direction for the game object to move in. (Should be a normalized vector)</param>
    /// <param name="cen">"Center" that the object was spawned around.</param>
    public void SetDirectionAndCenter (Vector3 dir, Vector3 cen) {
        direction = dir;
        center = cen;
    }

}
