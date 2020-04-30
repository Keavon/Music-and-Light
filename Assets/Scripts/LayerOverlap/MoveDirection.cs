using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDirection : MonoBehaviour
{

    public Vector3 direction = new Vector3 (1, 0, 0);

    public float localSpeed = 1;

    public static float globalSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log ("move");
        // Vector3 pos = transform.localPosition;
        // pos = pos + (direction * Time.deltaTime);
        // transform.localPosition = pos;
        transform.localPosition += direction * Time.deltaTime * localSpeed * globalSpeed;
    }
}
