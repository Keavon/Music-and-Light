using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateEffectGroup : EffectGroup
{
    public override void Effect(GameObject go) {
        Rotate r = go.GetComponent<Rotate> ();
        if (r != null) {
            float level = Lasp.MasterInput.GetPeakLevel (Lasp.FilterType.LowPass) * 10;
            r.Speed = 20 + level;
        }
    }
}
