﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;




public class LayerSpawner : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {

        List<Transform> temp = transform.GetComponentsInChildren<Transform>().Where(t => t.tag == "Wall").ToList();

        if (temp.Count == 0) {
            //Debug.LogError("No walls, did you forget to tag them?");
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
        if (Input.GetKeyDown(KeyCode.I)) {
            StartLayer(Directions.North);
        }
        if (Input.GetKeyDown(KeyCode.J)) {
            StartLayer(Directions.West);
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            StartLayer(Directions.South);
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            StartLayer(Directions.East);
        }
    }

    private GameObject GenerateLayer(Directions direction) {
        GameObject layer = GameObject.Instantiate(layerPrefab, new Vector3(), Quaternion.identity, transform) as GameObject;
        layer.name = direction.ToString();
        layer.transform.localEulerAngles += new Vector3(90, 0, 0);
        layer.transform.localScale = GenerateDimensions(max.x - min.x, max.y - min.y) / 10;

        switch (direction)
        {
            case Directions.North:
                layer.transform.localPosition = new Vector3(min.x + layer.transform.localScale.x*5, max.y + layer.transform.localScale.z*5, 0.14f);
                layer.transform.localPosition += new Vector3(GeneratePosition(max.x - min.x - layer.transform.localScale.x*10), 0, 0.14f);
                break;
            case Directions.East:
                layer.transform.localPosition = new Vector3(max.x + layer.transform.localScale.x*5, min.y + layer.transform.localScale.z*5, 0.14f);
                layer.transform.localPosition += new Vector3(0, GeneratePosition(max.y - min.y - layer.transform.localScale.z*10), 0.14f);
                break;
            case Directions.South:
                layer.transform.localPosition = new Vector3(min.x + layer.transform.localScale.x*5, min.y - layer.transform.localScale.z*5, 0.14f);
                layer.transform.localPosition += new Vector3(GeneratePosition(max.x - min.x - layer.transform.localScale.x*10), 0, 0.14f);
                break;
            case Directions.West:
                layer.transform.localPosition = new Vector3(min.x - layer.transform.localScale.x*5, min.y + layer.transform.localScale.z*5, 0.14f);
                layer.transform.localPosition += new Vector3(0, GeneratePosition(max.y - min.y - layer.transform.localScale.z*10), 0.14f);
                break;
            default:
                //Debug.LogError("Invalid Direction: Valid Directions are North (1), East (2), South (3), West(4)");
                break;
        }

        layer.GetComponent<MeshRenderer>().material.color = GenerateColor();

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
                md.direction = new Vector3(-1, 0, 0);
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
                md.direction = new Vector3(1, 0, 0);
                break;
            default:
                //Debug.LogError("Invalid Direction: Valid Directions are North (1), East (2), South (3), West(4)");
                break;
        }
        md.enabled = false;

        if (transform.name == "SideScreen" || transform.name == "Test") {
            Vector3 tempPosition = layer.transform.localPosition;
            layer.transform.localPosition = new Vector3(-tempPosition.z, tempPosition.y, tempPosition.x);
        }

        return layer;
    }

    private Vector3 GenerateDimensions(float xMax, float yMax) {
        return new Vector3(Randomization.RandomFloat(0.01f, xMax), 1, Randomization.RandomFloat(0.01f, yMax));
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
        go.GetComponent<MeshRenderer>().material.color = c;
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
        go.GetComponent<MeshRenderer>().material.color = c;
        go.GetComponent<MoveDirection>().enabled = true;
        go.GetComponent<MoveDirection>().localSpeed = speed;
        go.GetComponent<MoveBack>().enabled = true;
        go.GetComponent<FadeOut>().enabled = true;
        layersToSpawn[(int)direction] = GenerateLayer(direction);
    }

}
