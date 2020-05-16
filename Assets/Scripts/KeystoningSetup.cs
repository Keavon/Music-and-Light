using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeystoningSetup : MonoBehaviour {
	public GameObject keystonedProjector;
	public GameObject[] virtualProjectorCameras;
	public GameObject virtualProjectorCamerasParent;
	public float camMoveSpeed = 1;
	public float camRotateSpeed = 1;
	public float camRollSpeed = 1;
	public float camZoomSpeed = 1;

	void Start() {
		for (int i = 0; i < 3; i++) {
			GameObject created = Instantiate(keystonedProjector, transform.position + new Vector3(i - 1, 0, 10), Quaternion.identity, transform) as GameObject;
			Keystoning script = created.GetComponent<Keystoning>();
			script.virtualProjectorCameraNumber = i + 1;
			script.virtualProjectorCamerasParent = virtualProjectorCamerasParent;
			script.camMoveSpeed = camMoveSpeed;
			script.camRotateSpeed = camRotateSpeed;
			script.camRollSpeed = camRollSpeed;
			script.camZoomSpeed = camZoomSpeed;
		}
	}
}
