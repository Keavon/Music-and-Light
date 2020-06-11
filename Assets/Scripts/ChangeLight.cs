using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLight : MonoBehaviour
{
    Light myLight;

    void Start()
    {
        myLight = GetComponent<Light>();
        myLight.intensity = 0.0f;
    }

    void Update()
    {
        if (myLight.intensity < 0.55f)
        {
            myLight.intensity += 0.002f;
        }    
    }
}
