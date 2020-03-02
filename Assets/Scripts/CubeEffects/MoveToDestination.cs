using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToDestination : MonoBehaviour
{

    private class DestinationWrapper {
        public Vector3 destination;
    }

    /// <summary>
    /// Where the object should move to.
    /// </summary>
    private DestinationWrapper destination;

    /// <summary>
    /// Is the move finished?
    /// </summary>
    private bool moveFinished = false;

    /// <summary>
    /// Is the move finished?
    /// </summary>
    public bool IsMoveFinished {
        get {
            return moveFinished;
        }
    }

    /// <summary>
    /// Speed the object should move at.
    /// </summary>
    private static float speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (destination != null && !moveFinished) {
            if (transform.localPosition != destination.destination) {
                moveFinished = false;
                transform.localPosition = Vector3.MoveTowards (transform.localPosition, destination.destination, MoveToDestination.speed * Time.deltaTime);
            } else {
                moveFinished = true;
                destination = null;
            }
        }
    }

    /// <summary>
    /// Sets the destination if it was not already set.
    /// </summary>
    /// <param name="dest">Destination for the object.</param>
    public void SetDestination (Vector3 dest) {
        if (destination == null) {
            destination = new DestinationWrapper();
        }
        destination.destination = dest;
    }
    
    public static void SetSpeed (float speed) {
        MoveToDestination.speed = speed;
    }

}
