using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyEffectToRandom : MonoBehaviour
{
    int sceneHeight;
    int sceneWidth;
    public GameObject canvas;
    private CubeCanvas cc;
    int numSelected;
    public int maxRandomSelected;
    public List<EffectGroup> effects;


    // Start is called before the first frame update
    void Start()
    {
        
        cc = canvas.GetComponent<CubeCanvas>();
        sceneHeight = cc.CanvasHeight;
        sceneWidth = cc.CanvasWidth;
        numSelected = 0;
    }

    // Update is called once per frame
    void Update()
    {
        int currentRandRow = Randomization.RandomInt(0, sceneHeight);
        int currentRandRowPos = Randomization.RandomInt(0, sceneWidth);
        if (numSelected < maxRandomSelected)
        {
            numSelected++;
            cc.AddEffectGroup(currentRandRow, currentRandRowPos, effects); 
        }
    }
}
