using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDirection : MonoBehaviour
{

    public Vector3 direction = new Vector3 (1, 0, 0);

    public float localSpeed = 1;

    public static float globalSpeed = 10;

    private Rigidbody rd;

    private int maxBounces = 2;

    public int MaxBounces {
        get {
            return maxBounces;
        }
    }

    private int numBounces = 0;

    public int NumBounces {
        get {
            return numBounces;
        }
    }

    private Vector3 curForce;

    // Start is called before the first frame update
    void Start()
    {
        curForce = new Vector3();
        rd = gameObject.GetComponent<Rigidbody>();
        //Debug.Log (rd);
        rd.AddForce(direction * localSpeed * globalSpeed, ForceMode.Force);
        curForce += (direction * localSpeed * globalSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log ("move");
        // Vector3 pos = transform.localPosition;
        // pos = pos + (direction * Time.deltaTime);
        // transform.localPosition = pos;


        // transform.localPosition += direction * Time.deltaTime * localSpeed * globalSpeed;
        if (numBounces > 0 && curForce.magnitude != 0) {
            rd.AddForce(direction * localSpeed * globalSpeed * 0.01f, ForceMode.Force);
            curForce += (direction * localSpeed * globalSpeed * 0.01f);
        }
    }

    void OnDisable() {
        if (rd == null) {
            rd = gameObject.GetComponent<Rigidbody>();
        }
        rd.constraints = rd.constraints | RigidbodyConstraints.FreezePosition;
        // Debug.Log ("disable");
    }

    void OnEnable () {
        if (rd == null) {
            rd = gameObject.GetComponent<Rigidbody>();
        }
        rd.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
        numBounces = 0;
    } 

    public void Bounce() {
        numBounces++;
        if (rd == null) {
            rd = gameObject.GetComponent<Rigidbody>();
        }
        if (numBounces < maxBounces) {
            // Debug.Log(rd);
            rd.AddForce(-direction * localSpeed * globalSpeed * 1.5f, ForceMode.Force);
            curForce += (-direction * localSpeed * globalSpeed * 1.5f);
            // Debug.Log ("b");
        }
        else {
            //rd.AddForce(-curForce, ForceMode.Force);
            rd.constraints = rd.constraints | RigidbodyConstraints.FreezePosition;
        }
    }
}
