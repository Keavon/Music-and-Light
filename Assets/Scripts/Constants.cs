using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{

    /// <summary>
    /// Distance from origin (camera) to where the tunnel spawns.
    /// </summary>
    public static readonly float tunnelLength = 80;

    /// <summary>
    /// Speed multiplier for objects.
    /// </summary>
    public static readonly float objectSpeedMultiplier = 10;

    /// <summary>
    /// How long it takes for an object to get from the spawner to the camera.
    /// </summary>
    public static readonly float timeBetweenSpawnerAndCamera = tunnelLength / objectSpeedMultiplier;

}
