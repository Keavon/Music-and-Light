using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAudioReact : MonoBehaviour
{
    public float audioInfluence = 0.0f;
    float minSpeed = 0.01f;
    float maxSpeed = 4.0f;
    float baseVelocity = 2.0f;
    private ParticleSystem pSys;
    ParticleSystem.Particle[] mParticles;
    public GameObject particleSubspawn;
    public List<ParticleCollisionEvent> collisionEvents;
    public float velocityFactor = 1.0f;
    private float minStartSize = 0.05f;
    private float maxStartSize = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        pSys = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
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
        Color newColor = new Color(Mathf.Clamp(audioInfluence - Random.Range(0.2f,0.5f),0.0f,1.0f), Mathf.Clamp(audioInfluence - Random.Range(0.2f, 0.5f), 0.0f, 1.0f), audioInfluence);
        for (int i = 0; i < nump; i++)
        {
            newPart = mParticles[i];
            newPart.startColor = newColor;
            newPart.startSize = Random.Range(minStartSize, maxStartSize);
            //Debug.Log("Start Size: " + newPart.startSize);
            //if (newPart.startSize < minStartSize)
            //{
            //    newPart.startSize = minStartSize;
            //}
            //if (newPart.startSize > maxStartSize)
            //{
            //    newPart.startSize = maxStartSize;
            //}
            Vector3 addVelDir = mParticles[i].velocity.normalized;
            //newPart.velocity -= 0.5f * addVelDir * Time.deltaTime;
            //if (audioInfluence > 0.8)
            //{
            //    newPart.velocity += 0.8f *addVelDir * Time.deltaTime;
            //    //Debug.Log("Adding Velocity");
            //}
            newPart.velocity = addVelDir * baseVelocity * (audioInfluence * velocityFactor);
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
                Instantiate(particleSubspawn, collisionHitLoc, Quaternion.identity);
            }
            i++;
        }
    }
}
