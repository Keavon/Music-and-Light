using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LayerEffectController : MonoBehaviour
{

    public List<GameObject> walls = new List<GameObject>();

    public List<LayerSpawner> spawners = new List<LayerSpawner>();

    public bool autoFind = true;

    public GameObject layerPrefab;

    // Start is called before the first frame update
    void Awake()
    {

        if (layerPrefab == null) {
            Debug.LogError("Forgot to set layer prefab");
        }

        if (autoFind) {
            walls = GameObject.FindGameObjectsWithTag("WallContainer").ToList();
        }

        foreach(GameObject wall in walls) {
            wall.AddComponent<LayerSpawner>();
            spawners.Add(wall.GetComponent<LayerSpawner>());
            wall.GetComponent<LayerSpawner>().InitLayer(layerPrefab);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
