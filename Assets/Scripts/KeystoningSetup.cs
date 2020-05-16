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

	GUIStyle guiStyle;
	GUIStyleState guiStyleState;

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

		guiStyleState = new GUIStyleState();
		guiStyleState.textColor = Color.white;
		guiStyle = new GUIStyle();
		guiStyle.fontSize = 7;
		guiStyle.normal = guiStyleState;
	}

	void OnGUI() {
		GUI.Label(new Rect(5, 5, 10000, 20), "LMB drag corners, MMB drag rect, RMB drag vignette corners, SHIFT+R reset all, R reset, 123 pick output display, SHIFT slow 10x, WASDQE dolly, ARROWS pan, [] roll, +- zoom", guiStyle);
	}
}
