using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CubeCanvas : MonoBehaviour
{

    /// <summary>
    /// Wrapper class to be able to setup a 3d list in the editor.
    /// </summary>
    /// <typeparam name="T">Type of the list.</typeparam>
    [System.Serializable]
    private class ListWrapper<T>
    {
        public List<T> list;
    }

    /// <summary>
    /// Wrapper for a list of effect groups.
    /// </summary>
    [System.Serializable]
    private class EffectListWrapper : ListWrapper<EffectGroup> { }

    /// <summary>
    /// Wrapper for a list of lists of effect groups.
    /// </summary>
    [System.Serializable]
    private class ListEffectListWrapper : ListWrapper<EffectListWrapper> { }

    /// <summary>
    /// Cube game object that should spawn.
    /// </summary>
    [SerializeField]
    private GameObject cube;

    /// <summary>
    /// Placeholder gameobject to help with scaling.
    /// </summary>
    [SerializeField]
    private GameObject placeholder;

    /// <summary>
    /// Size of the cubes.
    /// </summary>
    [SerializeField]
    private int cubeSize = 1;

    /// <summary>
    /// Default cube size inside of the cube size.
    /// </summary>
    [SerializeField]
    private float cubePercentage = 0.9f;

    /// <summary>
    /// Scale for the object;
    /// </summary>
    private Vector3 scale;

    /// <summary>
    /// Number of cubes that fit into the canvas vertically.
    /// </summary> 
    [SerializeField]
    private int canvasHeight = 5;

    /// <summary>
    /// Number of cubes that fit into the canvas vertically.
    /// </summary> 
    public int CanvasHeight
    {
        get
        {
            return canvasHeight;
        }
    }

    /// <summary>
    /// Number of cubes that fit into the canvas horizontally.
    /// </summary>
    [SerializeField]
    private int canvasWidth = 10;

    /// <summary>
    /// Number of cubes that fit into the canvas horizontally.
    /// </summary>
    public int CanvasWidth
    {
        get
        {
            return canvasWidth;
        }
    }

    /// <summary>
    /// Array of cubes. Note: arranged height first, then width for ease of use.
    /// </summary>
    private GameObject[][] cubes;

    /// <summary>
    /// Speed of the cubes
    /// </summary>
    [SerializeField]
    private float cubeSpeed = 1f;


    [SerializeField]
    /// <summary>
    /// Time between spawning objects
    /// </summary>
    private float spawnTimeStep = 1f;

    /// <summary>
    /// Will cubes be spawned externally.
    /// </summary>
    [SerializeField]
    private bool spawnSeperately = false;

    /// <summary>
    /// List of effect group distribution set in the editor.
    /// </summary>
    [SerializeField]
    private List<ListEffectListWrapper> groupDistributionEditor;

    /// <summary>
    /// List of effect group distribution.
    /// </summary>
    private List<List<List<EffectGroup>>> groupDistribution = new List<List<List<EffectGroup>>>();

    // Start is called before the first frame update
    void Start()
    {
        SetupEffectGroups();
        cubes = new GameObject[canvasHeight][];
        // if (groupDistribution.Count() != canvasHeight) {
        //     Debug.LogWarning("Group dist size height doesn't match given canvas size");
        // }    

        for (int i = 0; i < canvasHeight; i++)
        {
            cubes[i] = new GameObject[canvasWidth];
            // if (groupDistribution[i].Count != canvasWidth) {
            //     Debug.LogWarning ("Group dist size width doesn't match given canvas size");
            // }
        }

        scale = new Vector3(cubeSize * cubePercentage, cubeSize * cubePercentage, cubeSize * cubePercentage);

        MoveToDestination.SetSpeed(cubeSpeed);
    }

    private float t = 0;

    // Update is called once per frame
    void Update()
    {
        if (!spawnSeperately && t > spawnTimeStep)
        {
            SpawnObject(new List<EffectGroup>());
            t = 0;
        }
        t += Time.deltaTime;
    }

    /// <summary>
    /// Spawns an object at a random position. Calls EditEffectGroup with the list of effects on the appropriate position for the spawned object.
    /// </summary>
    /// <param name="effects">What effects to add to/remove from the object.</param>
    /// <returns></returns>
    public bool SpawnObject(List<EffectGroup> effects)
    {
        int rowToSpawn = SelectRow();

        if (rowToSpawn == -1)
        {
            return false;
        }

        int rowPosition = GetRowLengths()[rowToSpawn];
        Vector3 spawnPos = GetObjectInitialPosition(rowToSpawn);
        Vector3 destPos = GetObjectFinalPosition(rowToSpawn, rowPosition);
        List<EffectGroup> groups = GetEffectGroup(rowToSpawn, rowPosition);

        cubes[rowToSpawn][rowPosition] = GameObject.Instantiate(placeholder, spawnPos, Quaternion.identity, transform);

        // cubes[rowToSpawn][rowPosition] = GameObject.Instantiate(cube, spawnPos, Quaternion.identity, transform);
        GameObject go = cubes[rowToSpawn][rowPosition];
        go.transform.localPosition = spawnPos;
        GameObject temp = GameObject.Instantiate(cube, new Vector3(), Quaternion.identity, go.transform);
        temp.transform.localPosition = new Vector3(0, 0, -(transform.position.z + (cubeSize / 2f)));

        go.transform.localScale = scale;

        go.AddComponent<MoveToDestination>();
        MoveToDestination mtd = go.GetComponent<MoveToDestination>();
        mtd.SetDestination(destPos);

        if (effects.Count > 0)
        {
            EditEffectGroup(rowToSpawn, rowPosition, effects);
        }

        foreach (EffectGroup eg in groups)
        {
            eg.AddObjectToGroup(go);
        }

        return true;
    }

    /// <summary>
    /// Edit the list of effects on a block and update the effect groups.
    /// Any effect not already applied will be added.
    /// If an effect in effects is already applied, it will be removed.
    /// </summary>
    /// <param name="row">Row the block is in.</param>
    /// <param name="rowPos">Where in the row the block is.</param>
    /// <param name="effects">What effects to add to/remove from the block.</param>
    public void EditEffectGroup(int row, int rowPos, List<EffectGroup> effects)
    {
        foreach (EffectGroup eg in effects)
        {
            if (groupDistribution[row][rowPos].Contains(eg))
            {
                eg.RemoveObjectFromGroup(cubes[row][rowPos]);
            }
            else
            {
                eg.AddObjectToGroup(cubes[row][rowPos]);
            }
        }
        //                                                      (effects - currentEffects)                    U          (currentEffects - effects)             remove duplicates
        groupDistribution[row][rowPos] = (List<EffectGroup>)(effects.Except(groupDistribution[row][rowPos]).Union(groupDistribution[row][rowPos].Except(effects))).Distinct();
    }

    /// <summary>
    /// Add effects to the list of effects on a block and update the effect groups.
    /// </summary>
    /// <param name="row">Row the block is in.</param>
    /// <param name="rowPos">Where in the row the block is.</param>
    /// <param name="effects">What effects to add to the block.</param>
    public void AddEffectGroup(int row, int rowPos, List<EffectGroup> effects)
    {
        groupDistribution[row][rowPos] = (groupDistribution[row][rowPos].Union(effects).Distinct()).ToList<EffectGroup>();
        foreach (EffectGroup eg in effects)
        {
            Debug.Log(eg);
            eg.AddObjectToGroup(cubes[row][rowPos]);
        }
    }

    /// <summary>
    /// Remove effects from the list of effects on a block and update the effect groups.
    /// </summary>
    /// <param name="row">Row the block is in.</param>
    /// <param name="rowPos">Where in hte row the block is.</param>
    /// <param name="effects">What effects to remove from the block.</param>
    public void RemoveEffectGroup(int row, int rowPos, List<EffectGroup> effects)
    {
        groupDistribution[row][rowPos] = (List<EffectGroup>)(groupDistribution[row][rowPos].Except(effects).Distinct());
        foreach (EffectGroup eg in effects)
        {
            eg.RemoveObjectFromGroup(cubes[row][rowPos]);
        }
    }

    /// <summary>
    /// Get the effects of a blocka the given position
    /// </summary>
    /// <param name="row">Row the block is in.</param>
    /// <param name="rowPosition">Where in the row the block is.</param>
    /// <returns>List of effects on the block at the current position.</returns>
    public List<EffectGroup> GetEffectGroup(int row, int rowPosition)
    {
        return groupDistribution[row][rowPosition];
    }

    /// <summary>
    /// Convert the lists used to set up effect groups in the editor to regular lists.
    /// </summary>
    private void SetupEffectGroups()
    {
        List<List<EffectListWrapper>> outer = new List<List<EffectListWrapper>>();
        for (int i = 0; i < canvasHeight; i++)
        {
            if (i < groupDistributionEditor.Count())
            {
                outer.Add(groupDistributionEditor[i].list);
            }
            groupDistribution.Add(new List<List<EffectGroup>>());
        }

        for (int j = 0; j < groupDistribution.Count(); j++)
        {
            if (j < outer.Count())
            {
                for (int k = 0; k < canvasWidth; k++)
                {
                    if (k < outer[j].Count())
                    {
                        groupDistribution[j].Add(outer[j][k].list);
                    }
                    else
                    {
                        groupDistribution[j].Add(new List<EffectGroup>());
                    }
                }
            }
            else
            {
                for (int l = 0; l < canvasWidth; l++)
                {
                    groupDistribution[j].Add(new List<EffectGroup>());
                }
            }
        }
    }

    /// <summary>
    /// Check if all spawned objects have finished moving. Ignores any remaining gaps in the canvas.
    /// </summary>
    /// <returns>Returns true if all objects are done moving, and false otherwise.</returns>
    public bool CheckAllObjectsDoneMoving()
    {
        GameObject obj;

        for (int i = 0; i < canvasHeight; i++)
        {
            for (int j = 0; j < canvasWidth; j++)
            {
                obj = cubes[i][j];
                if (obj != null && !obj.GetComponent<MoveToDestination>().IsMoveFinished)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Get the object's initial position.
    /// </summary>
    /// <param name="row">Row the object is in.</param>
    /// <returns>Coordinate of the object's initial position.</returns>
    private Vector3 GetObjectInitialPosition(int row)
    {
        return new Vector3(row * cubeSize, canvasWidth * cubeSize, 0);
    }

    /// <summary>
    /// Get the object's final position.
    /// </summary>
    /// <param name="row">Row the object is in.</param>
    /// <param name="rowPosition">The object's final position in the row.</param>
    /// <returns>Coordinate of the object's final position.</returns>
    private Vector3 GetObjectFinalPosition(int row, int rowPosition)
    {
        return new Vector3(row * cubeSize, rowPosition * cubeSize, transform.position.z + (cubeSize / 2f));
    }

    /// <summary>
    /// Select what row to spawn the next object in.
    /// </summary>
    /// <returns>Index of the row to spawn the next object in.</returns>
    private int SelectRow()
    {
        int[] rowLengths = GetRowLengths();
        List<int> shortestRows = new List<int>();
        int minRow = GetMinIndex(rowLengths);
        int minVal = rowLengths[minRow];

        if (minVal == canvasWidth)
        {
            return -1;
        }

        shortestRows.Add(minRow);
        for (int i = 0; i < rowLengths.Length; i++)
        {
            if (i != minRow)
            {
                if (rowLengths[i] < minVal + Randomization.RandomInt(1, 3) && rowLengths[i] < canvasWidth)
                {
                    shortestRows.Add(i);
                }
            }
        }

        return shortestRows[Randomization.RandomInt(0, shortestRows.Count())];
    }

    /// <summary>
    /// Finds the index of the min value in an array of numbers.
    /// </summary>
    /// <param name="list">Array of numbers.</param>
    /// <returns>Index of the min value in the array.</returns>
    private int GetMinIndex(int[] list)
    {
        if (list == null)
        {
            Debug.LogError("list null in min index");
        }

        if (list.Length == 0)
        {
            Debug.LogError("list empty in min index");
        }

        int minIndex = 0;
        int minVal = list[0];

        for (int i = 1; i < list.Length; i++)
        {
            if (list[i] < minVal)
            {
                minVal = list[i];
                minIndex = i;
            }
        }

        return minIndex;
    }


    /// <summary>
    /// Get the lengths of each row.
    /// </summary>
    /// <returns>An array containing the lengths of each row.</returns>
    private int[] GetRowLengths()
    {
        int[] rowLengths = new int[canvasHeight];

        for (int i = 0; i < canvasHeight; i++)
        {
            for (int j = 0; j < canvasWidth; j++)
            {
                if (cubes[i][j] != null)
                {
                    rowLengths[i]++;
                }
            }
        }

        return rowLengths;
    }


}