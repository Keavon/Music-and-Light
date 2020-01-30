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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Spawn();
    }

    /// <summary>
    /// Spawn the object from <see cref="spawnableObjects"/> at index <see cref="objectToSpawn"/> and set the direction for the object.
    /// </summary>
    public void Spawn () {
        GameObject go;
        MoveObject mo;
        Vector3 dir;

        if (objectToSpawn > spawnableObjects.Count) {
            Debug.LogWarning ("Spawn object out of range: objectToSpawn - " + objectToSpawn + "; spawnableObjects.Count - " + spawnableObjects.Count);
            return;
        }

        go = GameObject.Instantiate (spawnableObjects [objectToSpawn], transform.position, transform.rotation);
        mo = go.GetComponent<MoveObject> ();

        if (mo == null) {
            Debug.LogWarning ("Missing 'MoveObject' script on spawned object - " + spawnableObjects [objectToSpawn]);
            GameObject.Destroy (go);
            return;
        }

        dir = -transform.position;
        dir.Normalize ();

        mo.SetDirection (dir);

    }
}
