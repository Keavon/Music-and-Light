using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public Material materialTop;
    public Material materialMiddle;
    public Material materialBottom;
    public Material materialSide;

    [Range(0,1)]
    public float fadeFactor;

    int directionMult = 1;
    bool transitioning;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (fadeFactor == 0)
            {
                transitioning = true;
                directionMult = 1;
            }
            else if (fadeFactor == 1)
            {
                transitioning = true;
                directionMult = -1;
            }
        }
        if (transitioning)
        {
            fadeFactor += Time.deltaTime * directionMult;
            if (fadeFactor >= 1 && directionMult == 1)
            {
                fadeFactor = 1;
                transitioning = false;
            }
            else if (fadeFactor <= 0 && directionMult == -1)
            {
                fadeFactor = 0;
                transitioning = false;
            }
        }
        materialTop.SetFloat("_Factor", fadeFactor);
        materialMiddle.SetFloat("_Factor", fadeFactor);
        materialBottom.SetFloat("_Factor", fadeFactor);
        materialSide.SetFloat("_Factor", fadeFactor);
    }
}
