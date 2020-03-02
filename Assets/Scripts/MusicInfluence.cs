using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class MusicInfluence : MonoBehaviour
{
    public VisualEffect myeffect;
    public float audioInfluence;
    // Start is called before the first frame update
    void Start()
    {
        myeffect = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        audioInfluence = Lasp.MasterInput.GetPeakLevel(Lasp.FilterType.LowPass) * 100;
        myeffect.SetFloat("Frequency", audioInfluence);
    }
}
