using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncRotationRecieve : MonoBehaviour
{

    /// <summary>
    /// How much to rotate each time.
    /// </summary>
    [SerializeField]
    private List<float> rotationAmounts = new List<float> ();

    /// <summary>
    /// How long until the rotate should be done.
    /// </summary>
    [SerializeField]
    private List<float> timeToRotation = new List<float> ();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rotationAmounts.Count != timeToRotation.Count) {
            Debug.LogWarning ("rotationAmounts.Count != timeToRotation.Count -- " + rotationAmounts.Count + " != " + timeToRotation.Count);
            return;
        }
        for (int i = 0; i < timeToRotation.Count; i++) {
            timeToRotation [i] -= Time.deltaTime;
            if (timeToRotation [i] <= 0) {
                transform.RotateAround (new Vector3 (0, 0, 0), Vector3.up, rotationAmounts [i]);
                timeToRotation.RemoveAt (0);
                rotationAmounts.RemoveAt (0);
            }
        }
    }


    public void AddRotation (float rotationAmount) {
        rotationAmounts.Add (rotationAmount);
        timeToRotation.Add (Constants.timeBetweenSpawnerAndCamera);
    }
}
