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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (direction != null) {
            distanceToMove = (direction * Constants.objectSpeedMultiplier * Time.deltaTime).magnitude;
            totalDistanceMoved += distanceToMove;
            transform.position += direction * Constants.objectSpeedMultiplier * Time.deltaTime;
        }

        if (totalDistanceMoved > Constants.tunnelLength * 1.1) {
            GameObject.Destroy (gameObject);
        }
    }

    /// <summary>
    /// Set the direction for the game object to move in.
    /// </summary>
    /// <param name="dir">Direction for the game object to move in.</param>
    public void SetDirection (Vector3 dir) {
        direction = dir;
    }

}
