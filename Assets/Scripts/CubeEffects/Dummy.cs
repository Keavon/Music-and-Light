using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{

    [SerializeField]
    private CubeCanvas cubeCanvas;

    // Start is called before the first frame update
    void Start()
    {
       // Debug.Log("Started");
        int height = cubeCanvas.CanvasHeight;
        int width = cubeCanvas.CanvasWidth;

        Vector3 newPos = new Vector3(cubeCanvas.transform.position.x, cubeCanvas.transform.position.y, transform.position.z);

        if (width % 2 != 0)
        {
            newPos.x += (width - 1) / 2;
            newPos.x += 0.5f;
        }
        else
        {
            newPos.x += width / 2;
        }

        if (height % 2 != 0)
        {
            newPos.y += (height - 1) / 2;
            newPos.y += 0.5f;
        }
        else
        {
            newPos.y += height / 2;
        }

        newPos.x *= -1;

        transform.position = newPos;
        //Debug.Log(newPos);
        //Debug.Log(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Updating");
    }
}