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
    public GameObject[] waterVideoPlayers;
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
        public bool effectBlendOverMode;
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
            if (!Input.GetKey(KeyCode.LeftShift) && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))) {
                ChooseNextEffect(fadeFactor == 1, 0);
            }
            if (!Input.GetKey(KeyCode.LeftShift) && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))) {
                ChooseNextEffect(fadeFactor == 1, 1);

                waterVideoPlayers[0].GetComponent<UnityEngine.Video.VideoPlayer>().frame = 0;
                waterVideoPlayers[1].GetComponent<UnityEngine.Video.VideoPlayer>().frame = 0;
                waterVideoPlayers[2].GetComponent<UnityEngine.Video.VideoPlayer>().frame = 0;
                waterVideoPlayers[3].GetComponent<UnityEngine.Video.VideoPlayer>().frame = 0;
            }
            if (!Input.GetKey(KeyCode.LeftShift) && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))) {
                ChooseNextEffect(fadeFactor == 1, 2);
            }
        }
    }

    void ChooseNextEffect(bool aOrB, int choiceIndex)
    {
        EffectRenderSetup curRenderSetup = effectsSetup[choiceIndex];
        string texture = aOrB ? "_TextureA" : "_TextureB";
        string blendOverChannelCurrent = aOrB ? "_BlendOverWithA" : "_BlendOverWithB";
        materialTop.SetTexture(texture, curRenderSetup.top);
        materialMiddle.SetTexture(texture, curRenderSetup.middle);
        materialBottom.SetTexture(texture, curRenderSetup.bottom);
        materialSide.SetTexture(texture, curRenderSetup.side);

        materialTop.SetFloat("_BlendOverWithA", 0.0f);
        materialTop.SetFloat("_BlendOverWithB", 0.0f);

        materialMiddle.SetFloat("_BlendOverWithA", 0.0f);
        materialMiddle.SetFloat("_BlendOverWithB", 0.0f);

        materialBottom.SetFloat("_BlendOverWithA", 0.0f);
        materialBottom.SetFloat("_BlendOverWithB", 0.0f);

        materialSide.SetFloat("_BlendOverWithA", 0.0f);
        materialSide.SetFloat("_BlendOverWithB", 0.0f);

        if (effectsSetup[choiceIndex].effectBlendOverMode) {
            materialTop.SetFloat(blendOverChannelCurrent, 1.0f);
            materialMiddle.SetFloat(blendOverChannelCurrent, 1.0f);
            materialBottom.SetFloat(blendOverChannelCurrent, 1.0f);
            materialSide.SetFloat(blendOverChannelCurrent, 1.0f);
        }
    }
}
