using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LayerSpawnerZones : MonoBehaviour
{

    public enum Directions {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }


    private GameObject layerPrefab;

    private GameObject[] layersToSpawn = new GameObject[4];

    private Vector2 min;

    private Vector2 max;

    public List<Transform> spawnZones = new List<Transform>();
    
    public List<Transform> szNorth = new List<Transform>();
    public List<Transform> szSouth = new List<Transform>();
    public List<Transform> szEast = new List<Transform>();
    public List<Transform> szWest = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        spawnZones = transform.GetComponentsInChildren<Transform>().Where(t => t.tag.Contains("SpawnZone")).ToList();

        szNorth = spawnZones.Where(t => t.tag.Contains("North")).ToList();
        szSouth = spawnZones.Where(t => t.tag.Contains("South")).ToList();
        szEast = spawnZones.Where(t => t.tag.Contains("East")).ToList();
        szWest = spawnZones.Where(t => t.tag.Contains("West")).ToList();

        List<Transform> temp = transform.GetComponentsInChildren<Transform>().Where(t => t.tag == "Wall").ToList();

        if (temp.Count == 0) {
            Debug.LogError("No walls, did you forget to tag them?");
        }

        if (transform.name == "SideScreen" || transform.name == "Test") {
            min = new Vector2(temp[0].localPosition.z, temp[0].localPosition.y) - new Vector2(temp[0].localScale.z, temp[0].localScale.y)/2f;
            max = new Vector2(temp[0].localPosition.z, temp[0].localPosition.y) + new Vector2(temp[0].localScale.z, temp[0].localScale.y)/2f;
        } else {
            min = new Vector2(temp[0].localPosition.x, temp[0].localPosition.y) - new Vector2(temp[0].localScale.x, temp[0].localScale.y)/2f;
            max = new Vector2(temp[0].localPosition.x, temp[0].localPosition.y) + new Vector2(temp[0].localScale.x, temp[0].localScale.y)/2f;
        }

        Vector2 tempMin = new Vector2();
        Vector2 tempMax = new Vector2();

        for (int i = 1; i < temp.Count; i++) {
            if (transform.name == "SideScreen" || transform.name == "Test") {
                tempMin = new Vector2(temp[i].localPosition.z, temp[i].localPosition.y) - new Vector2(temp[i].localScale.z, temp[i].localScale.y)/2f;
                tempMax = new Vector2(temp[i].localPosition.z, temp[i].localPosition.y) + new Vector2(temp[i].localScale.z, temp[i].localScale.y)/2f;
            } else {
                tempMin = new Vector2(temp[i].localPosition.x, temp[i].localPosition.y) - new Vector2(temp[i].localScale.x, temp[i].localScale.y)/2f;
                tempMax = new Vector2(temp[i].localPosition.x, temp[i].localPosition.y) + new Vector2(temp[i].localScale.x, temp[i].localScale.y)/2f;
            }

            if (tempMin.x < min.x) {
                min.x = tempMin.x;
            }
            if (tempMin.y < min.y) {
                min.y = tempMin.y;
            }
            if (tempMax.x > max.x) {
                max.x = tempMax.x;
            }
            if (tempMax.y > max.y) {
                max.y = tempMax.y;
            }
        }

        GameObject go;
        for (int i = 0; i < layersToSpawn.Length; i++) {
            layersToSpawn[i] = GenerateLayer((Directions)i);
            go = layersToSpawn[i];
            

        }
        // if (transform.name == "SideScreen" || transform.name == "Test") {
        //     //AAAAAAAAHHHHHHHHHHHH
        //     Vector3 tempPosition;
        //     foreach (GameObject layer in layersToSpawn) {
        //         tempPosition = layer.transform.localPosition;
        //         layer.transform.localPosition = new Vector3(-tempPosition.z, tempPosition.y, tempPosition.x);
        //     }
        // }

        

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) {
            StartLayer(Directions.North);
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            StartLayer(Directions.West);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            StartLayer(Directions.South);
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            StartLayer(Directions.East);
        }
    }

    private GameObject GenerateLayer(Directions direction) {
        // Debug.Log (direction);
        Transform selectedZone = null;
        switch (direction) {
            case Directions.North:
                selectedZone = szNorth[Randomization.RandomInt(0, szNorth.Count)];
                break;
            case Directions.South:
                selectedZone = szSouth[Randomization.RandomInt(0, szSouth.Count)];
                break;
            case Directions.East:
                selectedZone = szEast[Randomization.RandomInt(0, szEast.Count)];
                break;
            case Directions.West:
                selectedZone = szWest[Randomization.RandomInt(0, szWest.Count)];
                break;
            default:
                Debug.LogError("Invalid Direction: Valid Directions are North (1), East (2), South (3), West(4)");
                break;
        }

        GameObject layer = GameObject.Instantiate(layerPrefab, new Vector3(), Quaternion.identity, selectedZone) as GameObject;
        layer.name = direction.ToString();
        layer.transform.localEulerAngles += new Vector3(90, 0, 0);

        BoxCollider area = selectedZone.GetComponent<BoxCollider>();

        layer.transform.localScale = GenerateDimensions(area) / 10;

        //layer.transform.localScale = new Vector3 (.1f, .1f, .1f);

        //layer.transform.localPosition = new Vector3(1, 1, 1);

        switch (direction) {
            case Directions.North:
                if (transform.name == "SideScreen" || transform.name == "Test") {
                    layer.transform.localPosition = new Vector3(0, -(area.size.y - (layer.transform.localScale.z*10))/2, 0);
                    float sides = (area.size.z - (layer.transform.localScale.x*10))/2;
                    layer.transform.localPosition += new Vector3(0, 0, Randomization.RandomFloat(-sides, sides));
                } else {
                    layer.transform.localPosition = new Vector3(0, -(area.size.y - (layer.transform.localScale.z*10))/2, 0);
                    float sides = (area.size.x - (layer.transform.localScale.x*10))/2;
                    layer.transform.localPosition += new Vector3(Randomization.RandomFloat(-sides, sides), 0, 0);
                }
                break;
            case Directions.South:
                if (transform.name == "SideScreen" || transform.name == "Test") {
                    layer.transform.localPosition = new Vector3(0, (area.size.y - (layer.transform.localScale.z*10))/2, 0);
                    float sides = (area.size.z - (layer.transform.localScale.x*10))/2;
                    layer.transform.localPosition += new Vector3(0, 0, Randomization.RandomFloat(-sides, sides));
                } else {
                    layer.transform.localPosition = new Vector3(0, (area.size.y - (layer.transform.localScale.z*10))/2, 0);
                    float sides = (area.size.x - (layer.transform.localScale.x*10))/2;
                    layer.transform.localPosition += new Vector3(Randomization.RandomFloat(-sides, sides), 0, 0);
                }
                break;
            case Directions.East:
                if (transform.name == "SideScreen" || transform.name == "Test") {
                    layer.transform.localPosition = new Vector3(0, 0, (area.size.z - (layer.transform.localScale.x*10))/2);
                    float sides = (area.size.y - (layer.transform.localScale.z*10))/2;
                    layer.transform.localPosition += new Vector3(0, Randomization.RandomFloat(-sides, sides), 0);
                } else {
                    layer.transform.localPosition = new Vector3((area.size.x - (layer.transform.localScale.x*10))/2, 0, 0);
                    float sides = (area.size.y - (layer.transform.localScale.z*10))/2;
                    layer.transform.localPosition += new Vector3(0, Randomization.RandomFloat(-sides, sides), 0);
                }
                break;
            case Directions.West:
                if (transform.name == "SideScreen" || transform.name == "Test") {
                    layer.transform.localPosition = new Vector3(0, 0, -(area.size.z - (layer.transform.localScale.x*10))/2);
                    float sides = (area.size.y - (layer.transform.localScale.z*10))/2;
                    layer.transform.localPosition += new Vector3(0, Randomization.RandomFloat(-sides, sides), 0);
                } else {
                    layer.transform.localPosition = new Vector3(-(area.size.x - (layer.transform.localScale.x*10))/2, 0, 0);
                    float sides = (area.size.y - (layer.transform.localScale.z*10))/2;
                    layer.transform.localPosition += new Vector3(0, Randomization.RandomFloat(-sides, sides), 0);
                } 
                break;
            default:
                Debug.LogError("Invalid Direction: Valid Directions are North (1), East (2), South (3), West(4)");
                break;
        }

        // layer.transform.localScale = GenerateDimensions(max.x - min.x, max.y - min.y) / 10;

        // switch (direction)
        // {
        //     case Directions.North:
        //         layer.transform.localPosition = new Vector3(min.x + layer.transform.localScale.x*5, max.y + layer.transform.localScale.z*5, 0.14f);
        //         layer.transform.localPosition += new Vector3(GeneratePosition(max.x - min.x - layer.transform.localScale.x*10), 0, 0.14f);
        //         break;
        //     case Directions.East:
        //         layer.transform.localPosition = new Vector3(max.x + layer.transform.localScale.x*5, min.y + layer.transform.localScale.z*5, 0.14f);
        //         layer.transform.localPosition += new Vector3(0, GeneratePosition(max.y - min.y - layer.transform.localScale.z*10), 0.14f);
        //         break;
        //     case Directions.South:
        //         layer.transform.localPosition = new Vector3(min.x + layer.transform.localScale.x*5, min.y - layer.transform.localScale.z*5, 0.14f);
        //         layer.transform.localPosition += new Vector3(GeneratePosition(max.x - min.x - layer.transform.localScale.x*10), 0, 0.14f);
        //         break;
        //     case Directions.West:
        //         layer.transform.localPosition = new Vector3(min.x - layer.transform.localScale.x*5, min.y + layer.transform.localScale.z*5, 0.14f);
        //         layer.transform.localPosition += new Vector3(0, GeneratePosition(max.y - min.y - layer.transform.localScale.z*10), 0.14f);
        //         break;
        //     default:
        //         Debug.LogError("Invalid Direction: Valid Directions are North (1), East (2), South (3), West(4)");
        //         break;
        // }

        Color c = GenerateColor();
        Material m = layer.GetComponent<MeshRenderer>().material;
        SetColor(c, m);


        layer.AddComponent<CollisionHandlers>();
        layer.AddComponent<OverlapShapeData>();
        layer.AddComponent<FadeOut>();
        layer.GetComponent<FadeOut>().enabled = false;
        layer.AddComponent<MoveBack>();
        layer.GetComponent<MoveBack>().enabled = false;
        layer.AddComponent<MoveDirection>();
        MoveDirection md = layer.GetComponent<MoveDirection>();
        switch (direction) {
            case Directions.North:
                md.direction = new Vector3(0, -1, 0);
                break;
            case Directions.East:
                // if (name == "SideScreen" || name == "Test") {
                //     md.direction = new Vector3(0, 0, -1);
                // } else {
                //     md.direction = new Vector3(-1, 0, 0);
                // }
                md.direction = new Vector3(1, 0, 0);
                break;
            case Directions.South:
                md.direction = new Vector3(0, 1, 0);
                break;
            case Directions.West:
                // if (name == "SideScreen" || name == "Test") {
                //     md.direction = new Vector3(0, 0, 1);
                // } else {
                //     md.direction = new Vector3(1, 0, 0);
                // }
                md.direction = new Vector3(-1, 0, 0);
                break;
            default:
                Debug.LogError("Invalid Direction: Valid Directions are North (1), East (2), South (3), West(4)");
                break;
        }
        md.enabled = false;

        // if (transform.name == "SideScreen" || transform.name == "Test") {
        //     Vector3 tempPosition = layer.transform.localPosition;
        //     layer.transform.localPosition = new Vector3(-tempPosition.z, tempPosition.y, tempPosition.x);
        // }

        return layer;
    }

    private Vector3 GenerateDimensions(float xMax, float yMax) {
        return new Vector3(Randomization.RandomFloat(0.01f, xMax), 1, Randomization.RandomFloat(0.01f, yMax));
    }

    private Vector3 GenerateDimensions (BoxCollider area) {
        if (transform.name == "SideScreen" || transform.name == "Test") {
            // Debug.Log(area.size);
            return new Vector3(Randomization.RandomFloat(0.01f, area.size.z), 1, Randomization.RandomFloat(0.01f, area.size.y));
        }
        return new Vector3(Randomization.RandomFloat(0.01f, area.size.x), 1, Randomization.RandomFloat(0.01f, area.size.y));
    }

    private float GeneratePosition(float max) {
        return Randomization.RandomFloat(0, max);
    }

    private Color GenerateColor () {
        return new Color(Randomization.RandomFloat(0, 1), Randomization.RandomFloat(0, 1), Randomization.RandomFloat(0, 1), 1);
    }

    public void InitLayer(GameObject l) {
        layerPrefab = l;
    }

    private void SetColor(Color c, Material m) {
        m.color = c;
        m.EnableKeyword("_EMISSION");
        m.SetColor("_EmissionColor", c);
    }

    public void StartLayer(Directions direction) {
        GameObject go = layersToSpawn[(int)direction];
        go.GetComponent<MoveDirection>().enabled = true;
        go.GetComponent<MoveDirection>().localSpeed = Randomization.RandomFloat(1, 3);;
        go.GetComponent<MoveBack>().enabled = true;
        go.GetComponent<FadeOut>().enabled = true;
        layersToSpawn[(int)direction] = GenerateLayer(direction);
    }

    public void StartLayer(Directions direction, Color c) {
        GameObject go = layersToSpawn[(int)direction];
        SetColor(c, go.GetComponent<MeshRenderer>().material);
        go.GetComponent<MoveDirection>().enabled = true;
        go.GetComponent<MoveBack>().enabled = true;
        go.GetComponent<FadeOut>().enabled = true;
        layersToSpawn[(int)direction] = GenerateLayer(direction);
    }

    public void StartLayer(Directions direction, float speed) {
        GameObject go = layersToSpawn[(int)direction];
        go.GetComponent<MoveDirection>().enabled = true;
        go.GetComponent<MoveDirection>().localSpeed = speed;
        go.GetComponent<MoveBack>().enabled = true;
        go.GetComponent<FadeOut>().enabled = true;
        layersToSpawn[(int)direction] = GenerateLayer(direction);
    }

    public void StartLayer(Directions direction, Color c, float speed) {
        GameObject go = layersToSpawn[(int)direction];
        SetColor(c, go.GetComponent<MeshRenderer>().material);
        go.GetComponent<MoveDirection>().enabled = true;
        go.GetComponent<MoveDirection>().localSpeed = speed;
        go.GetComponent<MoveBack>().enabled = true;
        go.GetComponent<FadeOut>().enabled = true;
        layersToSpawn[(int)direction] = GenerateLayer(direction);
    }

}
