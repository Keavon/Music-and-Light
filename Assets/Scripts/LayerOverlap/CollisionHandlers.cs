using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandlers 
{
    public void OnCollisionEnter(Collision collision) {
        GameObject self = collision.collider.gameObject;
        GameObject other = collision.GetContact(0).otherCollider.gameObject;

        OverlapShapeData overlap = self.GetComponent<OverlapShapeData>();

        if (other.tag == "Border") {
            if (!overlap.EnteredScreen) {
                overlap.EnteredScreen = true;
            } else {
                StopMoving(self);
            }
        } if (other.tag == "Window") {
            StopMoving(self);
        } else {
            Debug.LogError ("Error - forgot to tag one of the colliders");
        }
    }

    private void StopMoving (GameObject self) {
        MoveDirection moveDirection = self.GetComponent<MoveDirection>();
        moveDirection.enabled = false;
    }
}
