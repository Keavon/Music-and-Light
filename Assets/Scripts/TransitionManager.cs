using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public Material materialTop;
    public Material materialMiddle;
    public Material materialBottom;
    public Material materialSide;
    public EffectRenderSetup[] effectsSetup;
    [Range(0,1)]
    public float fadeFactor;
    [System.Serializable]
    public struct EffectRenderSetup
    {
        public RenderTexture top;
        public RenderTexture middle;
        public RenderTexture bottom;
        public RenderTexture side;
        public GameObject effectRoot;
    }

    int directionMult = 1;
    bool transitioning = false;

    void Start()
    {
        ChooseNextEffect(true, 0);
        ChooseNextEffect(false, 1);
    }

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
        if (fadeFactor == 0 || fadeFactor == 1)
        {
            if (Input.GetKeyDown(KeyCode.F1)) ChooseNextEffect(fadeFactor == 1, 0);
            if (Input.GetKeyDown(KeyCode.F2)) ChooseNextEffect(fadeFactor == 1, 1);
            if (Input.GetKeyDown(KeyCode.F3)) ChooseNextEffect(fadeFactor == 1, 2);
        }
    }

    void ChooseNextEffect(bool aOrB, int choiceIndex)
    {
        EffectRenderSetup curRenderSetup = effectsSetup[choiceIndex];
        string texture = aOrB ? "_TextureA" : "_TextureB";
        materialTop.SetTexture(texture, curRenderSetup.top);
        materialMiddle.SetTexture(texture, curRenderSetup.middle);
        materialBottom.SetTexture(texture, curRenderSetup.bottom);
        materialSide.SetTexture(texture, curRenderSetup.side);
    }
}
