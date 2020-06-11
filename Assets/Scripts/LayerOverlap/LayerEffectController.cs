using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LayerEffectController : MonoBehaviour
{

    public List<GameObject> walls = new List<GameObject>();

    public List<LayerSpawnerZones> spawners = new List<LayerSpawnerZones>();

    public bool autoFind = true;

    public GameObject layerPrefab;

    public float audioInfluence = 0.0f;
    // Start is called before the first frame update
    void Awake()
    {

        if (layerPrefab == null) {
            //  Debug.LogError("Forgot to set layer prefab");
        }

        if (autoFind) {
            walls = GameObject.FindGameObjectsWithTag("WallContainer").ToList();
        }

        foreach(GameObject wall in walls) {
            wall.AddComponent<LayerSpawnerZones>();
            spawners.Add(wall.GetComponent<LayerSpawnerZones>());
            wall.GetComponent<LayerSpawnerZones>().InitLayer(layerPrefab);
        }

    }

    // Update is called once per frame
    void Update()
    {
        audioInfluence = Mathf.Clamp(Lasp.MasterInput.GetPeakLevel(Lasp.FilterType.LowPass) * 8, 0, 3);
        if (audioInfluence > 0.85)
        {
            Debug.Log("trigger");
            int roundedRandom = (int)Mathf.Round(audioInfluence);
            foreach (LayerSpawnerZones spawner in spawners)
            {
                // float audioRandom = Random.Range(0.0f, audioInfluence);
                // int roundedRandom = (int)Mathf.Round(audioRandom);
                if (roundedRandom == 0)
                {
                    spawner.StartLayer(LayerSpawnerZones.Directions.North, GenerateColor(audioInfluence / 3), roundedRandom / 4);
                    spawner.StartLayer(LayerSpawnerZones.Directions.South, GenerateColor(audioInfluence / 3), roundedRandom / 4);
                }
                else if (roundedRandom == 1)
                {
                    spawner.StartLayer(LayerSpawnerZones.Directions.South, GenerateColor(audioInfluence / 3), roundedRandom * roundedRandom / 2);
                    spawner.StartLayer(LayerSpawnerZones.Directions.North, GenerateColor(audioInfluence / 3), roundedRandom / 2);
                }
                else if (roundedRandom == 2)
                {
                    spawner.StartLayer(LayerSpawnerZones.Directions.North, GenerateColor(audioInfluence / 3.5f), roundedRandom);
                    spawner.StartLayer(LayerSpawnerZones.Directions.West, GenerateColor(audioInfluence / 3.2f), roundedRandom);
                    spawner.StartLayer(LayerSpawnerZones.Directions.East, GenerateColor(audioInfluence / 3.1f), roundedRandom * roundedRandom);
                }
                else if (roundedRandom == 3)
                {
                    spawner.StartLayer(LayerSpawnerZones.Directions.North, GenerateColor(audioInfluence / 3.05f), roundedRandom * roundedRandom / 2);
                    spawner.StartLayer(LayerSpawnerZones.Directions.East, GenerateColor(audioInfluence / 3.5f), roundedRandom);
                    spawner.StartLayer(LayerSpawnerZones.Directions.West, GenerateColor(audioInfluence / 3.9f), roundedRandom * roundedRandom);
                    spawner.StartLayer(LayerSpawnerZones.Directions.South, GenerateColor(audioInfluence / 3.4f), roundedRandom * roundedRandom / 4);
                }
            }
        }
    }


    private Color GenerateColor(float val) { 
        return new Color(Mathf.Clamp((val / 2) + Randomization.RandomFloat(0, 1), 0, 1),
                         Mathf.Clamp((val / 2) + Randomization.RandomFloat(0, 1), 0 , 1),
                         Mathf.Clamp((val / 2) + Randomization.RandomFloat(0, 1), 0 , 1), 1);
    }

}
