using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncRotationSend : MonoBehaviour
{

    /// <summary>
    /// Object to sync the rotation to.
    /// </summary>
    [SerializeField]
    private SyncRotationRecieve reciver;

    // =========== Temp Variables ===========

    /// <summary>
    /// Time between toggling the toggle.
    /// </summary>
    private float tempTimeDelay = 4.5f;

    /// <summary>
    /// Temp time tracker.
    /// </summary>
    private float tempTime = 0; 

    /// <summary>
    /// Toggle between rotating and not.
    /// </summary>
    private bool tempToggleRotate = false;

    // ======================================

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tempToggleRotate) {
            Rotate (20 * Time.deltaTime);
        }

        tempTime += Time.deltaTime;
        if (tempTime > tempTimeDelay) {
            tempTime = 0;
            tempToggleRotate = !tempToggleRotate;
        }
        
    }

    /// <summary>
    /// Rotate the object and tell the <see cref="reciver" /> about the rotation. 
    /// </summary>
    /// <param name="amount">Amount to rotate.</param>
    private void Rotate (float amount) {
        reciver.AddRotation (amount);
        transform.RotateAround(new Vector3 (0, 0, 0), Vector3.up, amount);
    }

}
