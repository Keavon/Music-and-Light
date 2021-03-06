﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAudioReact : MonoBehaviour
{
    public float audioInfluence = 0.0f;
    [Range (0.0f, 1.0f)]
    public float reactPercentage;
    [Range(0.0f, 1.0f)]
    public float audioThresh;
    [Range(0.0f, 10.0f)]
    public float audioClampMax;

    public float baseVelocity = 2.0f;
    float minSpeed = 0.01f;
    float maxSpeed = 4.0f;
    
    private ParticleSystem pSys;
    ParticleSystem.Particle[] mParticles;
    public ParticleSystem particleSubspawn;
    public List<ParticleCollisionEvent> collisionEvents;
    [Range(0.0f, 20.0f)]
    public float velocityFactor = 8.0f;
    private float minStartSize = 0.05f;
    private float maxStartSize = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        pSys = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var main = pSys.main;
        audioInfluence = Mathf.Clamp(Lasp.MasterInput.GetPeakLevel(Lasp.FilterType.LowPass) * 4, 0, audioClampMax);
        if (mParticles == null)
        {
            mParticles = new ParticleSystem.Particle[pSys.main.maxParticles];
        }
        int nump = pSys.GetParticles(mParticles);
        ParticleSystem.Particle newPart;
        Color newColor = new Color(Random.Range(audioInfluence,1.0f)/*Mathf.Clamp(audioInfluence/2.0f + audioInfluence,0,1)*/, Random.Range(0.5f, 0.7f), audioInfluence + Random.Range(0.0f, (1-audioClampMax)));
        for (int i = 0; i < nump; i++)
        {
            if (Random.Range(0.1f, 1.0f) < reactPercentage) continue;
            newPart = mParticles[i];
            newPart.startColor = newColor;
            newPart.startSize = Random.Range(minStartSize, maxStartSize);
            //Debug.Log("Start Size: " + newPart.startSize);
            Vector3 addVelDir = mParticles[i].velocity.normalized;
            //newPart.velocity -= 0.5f * addVelDir * Time.deltaTime;
            //if (audioInfluence > 0.8)
            //{
            //    newPart.velocity += 0.8f *addVelDir * Time.deltaTime;
            //    //Debug.Log("Adding Velocity");
            //}
            if (audioInfluence > audioThresh)
                newPart.velocity = addVelDir * baseVelocity * (audioInfluence * velocityFactor);
            float mag = Vector3.Magnitude(newPart.velocity);
            mag *= (1 - (0.16f * audioInfluence));
            
            if (i % 3 == 0)
            {
                newPart.velocity = Vector3.Normalize(newPart.velocity) * mag * mag / 2;
            }
            else
            {
                newPart.velocity = Vector3.Normalize(newPart.velocity) * mag;
            }   
            if (newPart.velocity.magnitude < minSpeed)
            {
                newPart.velocity = minSpeed * addVelDir;
            }

            if (newPart.velocity.magnitude > maxSpeed)
            {
                newPart.velocity = Random.Range(maxSpeed, maxSpeed + 4.0f) * addVelDir;
            }
            //if (newPart.velocity.magnitude > maxSpeed)
            //    newPart.velocity = maxSpeed * addVelDir;
            //Debug.Log("Current speed: " + newPart.velocity.magnitude);
            mParticles[i] = newPart;
        }
        pSys.SetParticles(mParticles, nump);
        //Debug.Log("NewColor: (" + newColor.r + ", " + newColor.g + ", " + newColor.b + ")");
    }

    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = pSys.GetCollisionEvents(other, collisionEvents);
        int i = 0;
        while (i < numCollisionEvents)
        {
            Vector3 collisionHitLoc = collisionEvents[i].intersection;
            if (audioInfluence > 0.001)
            {
                //Instantiate(particleSubspawn, collisionHitLoc, Quaternion.identity);
                particleSubspawn.transform.position = collisionHitLoc;
                particleSubspawn.Play();
            }
            i++;
        }
    }
}
