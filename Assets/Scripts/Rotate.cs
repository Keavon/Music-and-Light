using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    /// <summary>
    /// Direction around which to rotate.
    /// </summary>
    [SerializeField]
    private Vector3 direction = Vector3.up;

    private float speed = 30;

    public float Speed {
        get {
            return speed;
        }
        set {
            speed = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround (transform.position, direction, speed * Time.deltaTime);
    }
}
