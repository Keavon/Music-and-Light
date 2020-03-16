using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorEffectGroup : EffectGroup
{
    public int currentNum = 0;
    int maxNum = 4;
    public float audioInfluence;
    /// <summary>
    /// Effect to apply to each Game Object
    /// </summary>
    /// <param name="go">GameObject to effect.</param>
    ///
    public List<Material> materials;
    public override void Effect(GameObject go) {
        audioInfluence = Lasp.MasterInput.GetPeakLevel(Lasp.FilterType.LowPass) * 10;
        if (audioInfluence > 1 && currentNum < maxNum)
        {
            currentNum++;
        }
        else if (audioInfluence > 1 && currentNum >= maxNum)
        {
            currentNum = 0;
        }
        Material currentMaterial = materials[currentNum];
        var goRenderer = go.GetComponent<Renderer>();
        goRenderer.material = currentMaterial;
    }
}
