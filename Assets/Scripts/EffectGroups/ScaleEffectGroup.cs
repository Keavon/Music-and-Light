using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScaleEffectGroup : EffectGroup
{
        /// <summary>
    /// Add an object to the effect group.
    /// </summary>
    /// <param name="go">Object to add.</param>
    public override void AddObjectToGroup(GameObject go) {
        groupObjects.Add (go);
    }

    /// <summary>
    /// Remove an object from the effect group.
    /// </summary>
    /// <param name="go">Object to remove.</param>
    public override void RemoveObjectFromGroup(GameObject go) {
        groupObjects.Remove(go);
    }
}