using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{

    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Color c = meshRenderer.material.color;
        Debug.Log (c.a + " " + c.a * .75f);
        // c.a *= 0.75f * Time.deltaTime;
        // meshRenderer.material.color = c;

        if (c.a < 3) {
            //GameObject.Destroy(gameObject);
        }
    }
}
