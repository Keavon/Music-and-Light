using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{

    /// <summary>
    /// List of objects that can be spawned. Should all be prefabs and have the MoveObject script attatched.
    /// </summary>
    /// <typeparam name="GameObject"></typeparam>
    /// <returns></returns>
    [SerializeField]
    private List<GameObject> spawnableObjects = new List<GameObject> ();

    /// <summary>
    /// Object to spawn from the spawnableObjects list.
    /// </summary>
    [SerializeField]
    private int objectToSpawn = 0;

    /// <summary>
    /// Radius of the tunnel.
    /// </summary>
    [SerializeField]
    private float tunnelRadius = 1.05f;

    // =========== Temp Variables ===========

    /// <summary>
    /// Temp variable to get basic rotating in
    /// </summary>
    private float tempRotate = 0;

    /// <summary>
    /// Temp time delay between spawning objects.
    /// </summary>
    private float tempTimeDelay = .5f;

    /// <summary>
    /// Temp time tracker
    /// </summary>
    private float tempTime = 0;

    private int addToGroup = 1;

    [SerializeField]
    private EffectGroup scaleEffectGroup;

    [SerializeField]
    private EffectGroup rotateEffectGroup;

    // ======================================

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tempTime < tempTimeDelay) {
            tempTime += Time.deltaTime;
            tempRotate += Time.deltaTime;
            return;
        }
        Spawn (tempRotate, addToGroup == 1 ? scaleEffectGroup : rotateEffectGroup);
        Spawn (tempRotate + Mathf.PI * 0.5f, addToGroup == 1 ? rotateEffectGroup : scaleEffectGroup);
        Spawn (tempRotate + Mathf.PI, addToGroup == 1 ? scaleEffectGroup : rotateEffectGroup);
        Spawn (tempRotate + Mathf.PI * 1.5f, addToGroup == 1 ? rotateEffectGroup : scaleEffectGroup);
        addToGroup *= -1;
        tempRotate += Time.deltaTime;
        tempTime = 0;
    }

    /// <summary>
    /// Spawn the object from <see cref="spawnableObjects"/> at index <see cref="objectToSpawn"/> and set the direction for the object.
    /// </summary>
    /// <param name="angle">Angle around the circle to spawn the object at.</param>
    /// <param name="groupToAddTo">Group to add the spawned object to.</param>
    public void Spawn (float angle, EffectGroup groupToAddTo = null) {
        GameObject go;
        MoveObject mo;
        Vector3 dir;

        if (objectToSpawn > spawnableObjects.Count) {
            Debug.LogWarning ("Spawn object out of range: objectToSpawn - " + objectToSpawn + "; spawnableObjects.Count - " + spawnableObjects.Count);
            return;
        }

        go = GameObject.Instantiate (spawnableObjects [objectToSpawn], LocalPosToWorldPos (PositionOnCircle (angle)), transform.rotation);

        mo = go.GetComponent<MoveObject> ();
        if (mo == null) {
            go.AddComponent<MoveObject> ();
            mo = go.GetComponent<MoveObject> ();
        }

        dir = transform.position;
        dir.Normalize ();

        mo.SetDirectionAndCenter (dir, transform.position);

        if (go.GetComponent<Rotate> () == null) {
            go.AddComponent<Rotate> ();
        }

        if (groupToAddTo != null) {
            groupToAddTo.AddObjectToGroup(go);
        }

    }

    /// <summary>
    /// Convert a coordinate relative to the gameobject to world coordinates.
    /// </summary>
    /// <param name="localPos">Coordinate relative to the gameobject</param>
    /// <returns>Corresponding point in world coordinates</returns>
    private Vector3 LocalPosToWorldPos (Vector3 localPos) {
        Vector3 worldOffset = Camera.main.transform.rotation * localPos;
        return Camera.main.transform.position + worldOffset;
    }

    /// <summary>
    /// Get the position of the object on the circle around the spawner
    /// </summary>
    /// <param name="angle">Angle around the circle</param>
    /// <returns>Position around the circle.</returns>
    private Vector3 PositionOnCircle (float angle) {
        return new Vector3 (GetXCircle (angle), GetYCircle (angle), 0);
    }

    /// <summary>
    /// Parametric equation for x coordinate of the circle.
    /// </summary>
    /// <param name="t">t parameter for the parametric equation</param>
    /// <returns>X coordinate.</returns>
    private float GetXCircle (float t) {
        return tunnelRadius * Mathf.Cos (t);
    }

    /// <summary>
    /// Parametric equation for y coordinate of the circle.
    /// </summary>
    /// <param name="t">t parameter for the parametric equation</param>
    /// <returns>Y coordinate</returns>
    private float GetYCircle (float t) {
        return tunnelRadius * Mathf.Sin (t);
    }

}
