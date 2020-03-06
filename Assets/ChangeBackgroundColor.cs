using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBackgroundColor : MonoBehaviour
{
    public Camera cam;
    public Color col1 = Color.red;
    public Color col2 = Color.blue;
    public float duration = 3.0f;

    public float audioInfluence;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.PingPong(Time.time, duration) / duration;
        audioInfluence = Mathf.Clamp(Lasp.MasterInput.GetPeakLevel(Lasp.FilterType.LowPass) * 5, 0, 1);
        cam.backgroundColor = new Color(audioInfluence, Mathf.Clamp((float)(audioInfluence - 0.5), 0, 1), Mathf.Clamp((float)(audioInfluence - 0.2), 0, 1));
        //cam.backgroundColor = Color.Lerp(col1, col2, t);
    }
}
