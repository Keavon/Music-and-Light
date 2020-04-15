using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeystoningSetup : MonoBehaviour {
	public GameObject keystonedProjector;

	void Start() {
		for (int i = 0; i < 3; i++) {
			GameObject created = Instantiate(keystonedProjector, transform.position + new Vector3(i - 1, 0, 10), Quaternion.identity, transform) as GameObject;
			created.GetComponent<Keystoning>().projectorNumber = i + 1;
		}
	}
}
