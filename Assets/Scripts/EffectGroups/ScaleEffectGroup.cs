using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEffectGroup : EffectGroup
{
    /// <summary>
    /// Effect to apply to each Game Object
    /// </summary>
    /// <param name="go">GameObject to effect.</param>
    public override void Effect(GameObject go) {
        // go.transform.localScale = new Vector3 (1, 1, 1) * (0.5f*Mathf.Sin (totalTime*2) + 1);
        float level = Lasp.MasterInput.GetPeakLevel(Lasp.FilterType.LowPass) * 3;
        go.transform.localScale = new Vector3(1+level,1+level,1+level);
    }
}
