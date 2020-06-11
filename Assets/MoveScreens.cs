using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScreens : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(0.0f, -0.5f)]
    public float speed = 0.0f;
    void Start()
    {
        transform.position = new Vector3(0.0f, 0.0f, 4.3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z >= 0.1f)
        {
            transform.position += new Vector3(0.0f, 0.0f, speed);
        }
    }
}
