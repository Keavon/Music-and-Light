using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandlers : MonoBehaviour
{
    public void OnTriggerEnter(Collider collider) {
        GameObject self = gameObject;
        GameObject other = collider.gameObject;

        OverlapShapeData overlap = self.GetComponent<OverlapShapeData>();

        if (other.tag == "Border") {
            if (!overlap.EnteredScreen) {
                overlap.EnteredScreen = true;
            } else {
                StopMoving(self);
            }
        } else if (other.tag == "Window") {
            StopMoving(self);
        } else if (other.tag == "Layer") {
            // Ignore
        } else if (other.tag == "Wall") {
            GameObject.Destroy(self);
        } else {
            Debug.LogError ("Error - forgot to tag one of the colliders: " + other.name);
        }
    }

    private void StopMoving (GameObject self) {
        MoveDirection moveDirection = self.GetComponent<MoveDirection>();
        moveDirection.Bounce();
        if (moveDirection.NumBounces >= moveDirection.MaxBounces) {
            //moveDirection.enabled = false;
        }
    }
}
