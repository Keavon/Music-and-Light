using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleCube : MonoBehaviour
{
    public float audioInfluence;
    public Vector3 audioInfluenceVec;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*audioInfluence = Lasp.MasterInput.GetPeakLevel(Lasp.FilterType.LowPass);
        audioInfluenceVec = new Vector3(1 + audioInfluence, 1 + audioInfluence, 1 + audioInfluence);
        transform.localScale = audioInfluenceVec;
        print(audioInfluence);*/
    }

    public void changeScale(float influence)
    {
        audioInfluenceVec = new Vector3(1 + influence, 1 + influence, 1 + influence);
        transform.localScale = audioInfluenceVec;
        print(audioInfluence);
    }
}
