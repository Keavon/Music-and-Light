using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAudioReact : MonoBehaviour
{
    public float audioInfluence = 0.0f;
    float minSpeed = 0.1f;
    float maxSpeed = 5.0f;
    float baseVelocity = 5.0f;
    private ParticleSystem pSys;
    ParticleSystem.Particle[] mParticles;
    // Start is called before the first frame update
    void Start()
    {
        pSys = GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        var main = pSys.main;
        audioInfluence = Mathf.Clamp(Lasp.MasterInput.GetPeakLevel(Lasp.FilterType.Bypass) * 2,0,1);
        if (mParticles == null)
        {
            mParticles = new ParticleSystem.Particle[pSys.main.maxParticles];
        }
        int nump = pSys.GetParticles(mParticles);
        ParticleSystem.Particle newPart;
        //Color newColor = new Color(Mathf.Clamp(audioInfluence - Random.Range(0.2f,0.5f),0.0f,1.0f), Mathf.Clamp(audioInfluence - Random.Range(0.2f, 0.5f), 0.0f, 1.0f), audioInfluence);
        for (int i = 0; i < nump; i++)
        {
            newPart = mParticles[i];
            //newPart.startColor = newColor;
            //newPart.startSize = Random.Range(0.05f, 0.15f);
            Vector3 addVelDir = mParticles[i].velocity.normalized;
            //newPart.velocity -= 0.5f * addVelDir * Time.deltaTime;
            //if (audioInfluence > 0.8)
            //{
            //    newPart.velocity += 0.8f *addVelDir * Time.deltaTime;
            //    //Debug.Log("Adding Velocity");
            //}
            newPart.velocity = addVelDir * baseVelocity * audioInfluence;
            if (newPart.velocity.magnitude < minSpeed)
                newPart.velocity = minSpeed * addVelDir;
            //if (newPart.velocity.magnitude > maxSpeed)
            //    newPart.velocity = maxSpeed * addVelDir;
            Debug.Log("Current speed: " + newPart.velocity.magnitude);
            mParticles[i] = newPart;
        }
        pSys.SetParticles(mParticles, nump);
        //Debug.Log("NewColor: (" + newColor.r + ", " + newColor.g + ", " + newColor.b + ")");
    }
}
