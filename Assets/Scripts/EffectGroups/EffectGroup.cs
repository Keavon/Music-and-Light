using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectGroup : MonoBehaviour
{
    protected List<GameObject> groupObjects = new List<GameObject> ();

    protected float totalTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ApplyEffectToGroup();
        totalTime += Time.deltaTime;
    }

    /// <summary>
    /// Apply the effect to the group
    /// </summary>
    public void ApplyEffectToGroup() {
        List<GameObject> remove = new List<GameObject> ();
        foreach (GameObject go in groupObjects) {
            if (go == null) {
                remove.Add (go);
            } else {
                Effect (go);
            }
        }
        foreach (GameObject go in remove) {
            groupObjects.Remove (go);
        }
    }

    /// <summary>
    /// Effect to apply to each Game Object
    /// </summary>
    /// <param name="go">GameObject to effect.</param>
    public abstract void Effect (GameObject go);

    /// <summary>
    /// Add an object to the effect group.
    /// </summary>
    /// <param name="go">Object to add.</param>
    public virtual void AddObjectToGroup(GameObject go) {
        if (go.transform.childCount > 0) {
            groupObjects.Add (go.transform.GetChild(0).gameObject);
        } else {
            groupObjects.Add (go);
        }
    }

    /// <summary>
    /// Remove an object from the effect group.
    /// </summary>
    /// <param name="go">Object to remove.</param>
    public virtual void RemoveObjectFromGroup(GameObject go) {
        if (go.transform.childCount > 0) {
            groupObjects.Remove(go.transform.GetChild(0).gameObject);
        } else {
            groupObjects.Remove(go);
        }
    }
}
