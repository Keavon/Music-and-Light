using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for generating random numbers.
/// </summary>
public class Randomization : MonoBehaviour {

	/// <summary>
	/// Random number generator.
	/// </summary>
	private static System.Random r = new System.Random();

	/// <summary>
	/// Generates a random integer.
	/// </summary>
	/// <returns>The integer.</returns>
	/// <param name="min">Minimum value (inclusive).</param>
	/// <param name="max">Maximum value (exclusive).</param>
	public static int RandomInt (int min, int max) { //Min is inclusive, Max is exclusive
		return r.Next (min, max);
	}

	/// <summary>
	/// Generates a random percentage.
	/// </summary>
	/// <returns>The percentage.</returns>
	public static int RandomPercentage () {
		return r.Next (0, 100);
	}

	/// <summary>
	/// Generates a random percentage with a 60% change to be between 30 and 70, and only a 20% chance to be below 30 or above 70..
	/// </summary>
	/// <returns>The percentage.</returns>
	public static int RandomPercentageCentered () {
		int temp = r.Next (0, 5);
		if (temp == 1){
			return r.Next (0, 30);
		}
		if (temp == 4){
			return r.Next (70, 100);
		}
		return r.Next (30, 70);
	}

	/// <summary>
	/// Generates a random boolean value by generating a random number between 1 and 10, then seeing if it was even.
	/// </summary>
	/// <returns><c>true</c>, if the number generated was even, <c>false</c> otherwise.</returns>
	public static bool RandomBool () {
		int temp = r.Next (1, 11);
		if (temp % 2 == 0) {
			return true;
		}
		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="min">Mimimum value (inclusive).</param>
	/// <param name="max">Maximum value (exclusive).</param>
	/// <returns></returns>
	public static float RandomFloat (float min, float max) {
		return (float) r.NextDouble () * (max - min) + min;
	}
}
