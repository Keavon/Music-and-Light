using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBack : MonoBehaviour
{

    public float speed = 0.12f;
    public float speedMin = 0.08f;
    public float speedMax = 0.18f;

    // Start is called before the first frame update
    void Start()
    {
        speed = UnityEngine.Random.Range(speedMin, speedMax);
    }

    // Update is called once per frame
    void Update()
    {
        Transform parent = transform.parent.parent;
        if (parent.name == "SideScreen" || parent.name == "Test") {
            transform.localPosition += new Vector3(speed * Time.deltaTime, 0, 0);
        } else {
            transform.localPosition -= new Vector3(0, 0, speed * Time.deltaTime);
        }
        
    }
}
