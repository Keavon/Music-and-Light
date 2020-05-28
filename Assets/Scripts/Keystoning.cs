using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Keystoning : MonoBehaviour {
	public float scaleFactor = 0.001f;
	public RenderTexture[] virtualProjectorRenderTextures;
	public int virtualProjectorCameraNumber = 1;
	public GameObject virtualProjectorCamerasParent;
	public float camMoveSpeed = 1;
	public float camRotateSpeed = 1;
	public float camRollSpeed = 1;
	public float camZoomSpeed = 1;

	Camera uiCamera;
	Mesh mesh;
	Material mat;
	Vector3[] vertices;
	Vector3[] framingCorners;
	Vector3[] featherCorners;
	LineRenderer framingSidesLineRenderer;
	LineRenderer keystoneDotsLineRenderer;
	LineRenderer featherDotsLineRenderer;
	Camera cam;
	GameObject virtualProjectorCamera1;
	GameObject virtualProjectorCamera2;
	GameObject virtualProjectorCamera3;
	float radius = 0.02f;
	float thickness = 0.2f;
	int selectedCornerPointIndex;
	int selectedFeatherPointIndex;
	bool nearbyCornerPoint = false;
	bool nearbyFeatherPoint = false;
	bool draggingCorner = false;
	bool draggingFeather = false;
	bool mouseIsOverArea = false;
	bool draggingArea = false;
	Vector3 draggingAreaMouseLastWorldPosition;
	Vector3 mouseWorldPosition;
	Vector2 nativeResolution;
	Vector2 lastMousePosition;
	Vector2 lastMouseDelta;
	GameObject outputCamera;
	GameObject framingSides;
	GameObject keystoneDots;
	GameObject featherDots;

	void Start() {
		uiCamera = transform.parent.Find("Management Window Camera").GetComponent<Camera>();
		mesh = GetComponent<MeshFilter>().mesh;
		mat = GetComponent<MeshRenderer>().material;

		nativeResolution = new Vector2(mat.mainTexture.width, mat.mainTexture.height);
		virtualProjectorCamera1 = virtualProjectorCamerasParent.transform.Find("Projector Camera 1").gameObject;
		virtualProjectorCamera2 = virtualProjectorCamerasParent.transform.Find("Projector Camera 2").gameObject;
		virtualProjectorCamera3 = virtualProjectorCamerasParent.transform.Find("Projector Camera 3").gameObject;

		outputCamera = transform.Find("Output Camera").gameObject;
		framingSides = transform.Find("Framing Sides").gameObject;
		keystoneDots = transform.Find("Keystone Dots").gameObject;
		featherDots = transform.Find("Feather Dots").gameObject;

		vertices = mesh.vertices;
		ResetVertices();
		framingCorners = new Vector3[4];
		ResetFramingCorners();
		featherCorners = new Vector3[4];
		ResetFeatherCorners();

		cam = outputCamera.GetComponent<Camera>();
		cam.orthographicSize = scaleFactor * nativeResolution.x * 0.5f;

		framingSidesLineRenderer = framingSides.GetComponent<LineRenderer>();
		framingSidesLineRenderer.widthMultiplier = thickness;
		framingSidesLineRenderer.positionCount = 4;
		framingSidesLineRenderer.useWorldSpace = false;
		framingSidesLineRenderer.loop = true;

		keystoneDotsLineRenderer = keystoneDots.GetComponent<LineRenderer>();
		keystoneDotsLineRenderer.widthMultiplier = thickness;
		keystoneDotsLineRenderer.positionCount = 12;
		keystoneDotsLineRenderer.useWorldSpace = false;
		keystoneDotsLineRenderer.loop = true;

		featherDotsLineRenderer = featherDots.GetComponent<LineRenderer>();
		featherDotsLineRenderer.widthMultiplier = thickness;
		featherDotsLineRenderer.positionCount = 12;
		featherDotsLineRenderer.useWorldSpace = false;
		featherDotsLineRenderer.loop = true;

		// Build the rectangular frame for this video display
		for (int i = 0; i < framingCorners.Length; i++) {
			framingSidesLineRenderer.SetPosition(i, framingCorners[i]);
		}

		// Set up default virtual projector and output display number
		cam.targetDisplay = virtualProjectorCameraNumber;
		mat.mainTexture = virtualProjectorRenderTextures[virtualProjectorCameraNumber - 1];

		for (int i = 1; i < Display.displays.Length; i++) {
			Display.displays[i].Activate();
		}
	}

	void Update() {
		// Dragging display vertices (left mouse button)
		if (Input.GetKeyDown(KeyCode.Mouse0) && nearbyCornerPoint) draggingCorner = true;
		if (Input.GetKeyUp(KeyCode.Mouse0)) draggingCorner = false;
		if (draggingCorner) vertices[selectedCornerPointIndex] = mouseWorldPosition - transform.position;

		// Dragging feathering cage (right mouse button)
		if (Input.GetKeyDown(KeyCode.Mouse1) && nearbyFeatherPoint) draggingFeather = true;
		if (Input.GetKeyUp(KeyCode.Mouse1)) draggingFeather = false;
		if (draggingFeather) featherCorners[selectedFeatherPointIndex] = mouseWorldPosition - transform.position - vertices[selectedFeatherPointIndex];

		// Panning entire display (middle mouse button)
		if (Input.GetKeyDown(KeyCode.Mouse2) && mouseIsOverArea) {
			draggingArea = true;
			draggingAreaMouseLastWorldPosition = mouseWorldPosition;
		}
		if (Input.GetKeyUp(KeyCode.Mouse2)) {
			draggingArea = false;
		}
		if (draggingArea) {
			Vector3 newPositionDelta = mouseWorldPosition - draggingAreaMouseLastWorldPosition;
			draggingAreaMouseLastWorldPosition = mouseWorldPosition;
			for (int i = 0; i < vertices.Length; i++) {
				vertices[i] += newPositionDelta;
			}
		}

		// Resetting one or all views
		if (Input.GetKeyDown(KeyCode.R) && (mouseIsOverArea || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl))) {
			ResetVertices();
			ResetFeatherCorners();
		}

		// Output switching
		if (mouseIsOverArea && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))) cam.targetDisplay = 1;
		if (mouseIsOverArea && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))) cam.targetDisplay = 2;
		if (mouseIsOverArea && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))) cam.targetDisplay = 3;

		// Corner dot appearing when near cursor
		DetectMouseNearKeystoneCorner();
		DetectMouseNearFeatherCorner();

		// Send vertices to shader
		mesh.vertices = vertices;
		mat.SetVector("_v0", vertices[0]);
		mat.SetVector("_v1", vertices[1]);
		mat.SetVector("_v2", vertices[2]);
		mat.SetVector("_v3", vertices[3]);

		// Send feathering cage points to shader
		Vector3[] feathering = GetFeatherCorners();
		mat.SetVector("_f0", feathering[0]);
		mat.SetVector("_f1", feathering[1]);
		mat.SetVector("_f2", feathering[2]);
		mat.SetVector("_f3", feathering[3]);

		GameObject virtualCam = virtualProjectorCamera1;
		if (virtualProjectorCameraNumber == 1) virtualCam = virtualProjectorCamera1;
		if (virtualProjectorCameraNumber == 2) virtualCam = virtualProjectorCamera2;
		if (virtualProjectorCameraNumber == 3) virtualCam = virtualProjectorCamera3;

		// Hold shift to slow down movement by 10x
		float speedFactor = 1.0f;
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			speedFactor = 0.1f;
		}

		// Move camera left/right
		if (mouseIsOverArea && Input.GetKey(KeyCode.A)) {
			Vector3 newPosition = virtualCam.transform.localPosition + new Vector3(Time.deltaTime * camMoveSpeed * speedFactor, 0, 0);
			virtualCam.transform.localPosition = newPosition;
		}
		if (mouseIsOverArea && Input.GetKey(KeyCode.D)) {
			Vector3 newPosition = virtualCam.transform.localPosition - new Vector3(Time.deltaTime * camMoveSpeed * speedFactor, 0, 0);
			virtualCam.transform.localPosition = newPosition;
		}

		// Move camera up/down
		if (mouseIsOverArea && Input.GetKey(KeyCode.Q)) {
			Vector3 newPosition = virtualCam.transform.localPosition - new Vector3(0, Time.deltaTime * camMoveSpeed * speedFactor, 0);
			virtualCam.transform.localPosition = newPosition;
		}
		if (mouseIsOverArea && Input.GetKey(KeyCode.E)) {
			Vector3 newPosition = virtualCam.transform.localPosition + new Vector3(0, Time.deltaTime * camMoveSpeed * speedFactor, 0);
			virtualCam.transform.localPosition = newPosition;
		}

		// Move camera forward/backward
		if (mouseIsOverArea && Input.GetKey(KeyCode.W)) {
			Vector3 newPosition = virtualCam.transform.localPosition - new Vector3(0, 0, Time.deltaTime * camMoveSpeed * speedFactor);
			virtualCam.transform.localPosition = newPosition;
		}
		if (mouseIsOverArea && Input.GetKey(KeyCode.S)) {
			Vector3 newPosition = virtualCam.transform.localPosition + new Vector3(0, 0, Time.deltaTime * camMoveSpeed * speedFactor);
			virtualCam.transform.localPosition = newPosition;
		}

		// Pan camera left/right
		if (mouseIsOverArea && Input.GetKey(KeyCode.LeftArrow)) {
			virtualCam.transform.localRotation *= Quaternion.Euler(Vector3.down * (Time.deltaTime * camRotateSpeed * speedFactor));
		}
		if (mouseIsOverArea && Input.GetKey(KeyCode.RightArrow)) {
			virtualCam.transform.localRotation *= Quaternion.Euler(Vector3.up * (Time.deltaTime * camRotateSpeed * speedFactor));
		}

		// Pan camera up/down
		if (mouseIsOverArea && Input.GetKey(KeyCode.UpArrow)) {
			virtualCam.transform.localRotation *= Quaternion.Euler(Vector3.left * (Time.deltaTime * camRotateSpeed * speedFactor));
		}
		if (mouseIsOverArea && Input.GetKey(KeyCode.DownArrow)) {
			virtualCam.transform.localRotation *= Quaternion.Euler(Vector3.right * (Time.deltaTime * camRotateSpeed * speedFactor));
		}

		// Roll camera clockwise/counterclockwise
		if (mouseIsOverArea && Input.GetKey(KeyCode.LeftBracket)) {
			virtualCam.transform.localRotation *= Quaternion.Euler(Vector3.forward * (Time.deltaTime * camRollSpeed * speedFactor));
		}
		if (mouseIsOverArea && Input.GetKey(KeyCode.RightBracket)) {
			virtualCam.transform.localRotation *= Quaternion.Euler(Vector3.back * (Time.deltaTime * camRollSpeed * speedFactor));
		}

		// Zoom camera in/out
		if (mouseIsOverArea && Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus)) {
			virtualCam.GetComponent<Camera>().fieldOfView -= Time.deltaTime * camZoomSpeed * speedFactor;
		}
		if (mouseIsOverArea && Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus)) {
			virtualCam.GetComponent<Camera>().fieldOfView += Time.deltaTime * camZoomSpeed * speedFactor;
		}
	}

	void OnGUI() {
		UpdateWorldSpaceCursorPoint();

		selectedCornerPointIndex = NearestPointTo(mouseWorldPosition, vertices);
		Vector3 selectedCornerPoint = vertices[selectedCornerPointIndex];
		nearbyCornerPoint = Vector3.Distance(selectedCornerPoint, mouseWorldPosition - transform.position) < 0.05f;

		selectedFeatherPointIndex = NearestPointTo(mouseWorldPosition, GetFeatherCorners());
		Vector3 selectedFeatherPoint = GetFeatherCorners()[selectedFeatherPointIndex];
		nearbyFeatherPoint = Vector3.Distance(selectedFeatherPoint, mouseWorldPosition - transform.position) < 0.05f;

		bool inLeft = InsideTriangle(mouseWorldPosition - transform.position, vertices[0], vertices[1], vertices[2]);
		bool inRight = InsideTriangle(mouseWorldPosition - transform.position, vertices[1], vertices[3], vertices[2]);
		mouseIsOverArea = inLeft || inRight;
	}

	public void ResetVertices() {
		float x = nativeResolution.x * scaleFactor * 0.5f;
		float y = nativeResolution.y * scaleFactor * 0.5f;

		vertices[0].x = -x;
		vertices[0].y = -y;

		vertices[1].x = x;
		vertices[1].y = -y;

		vertices[2].x = -x;
		vertices[2].y = y;

		vertices[3].x = x;
		vertices[3].y = y;
	}

	public void ResetFramingCorners() {
		float x = nativeResolution.x * scaleFactor * 0.5f;
		float y = nativeResolution.y * scaleFactor * 0.5f;

		framingCorners[0].x = x;
		framingCorners[0].y = y;

		framingCorners[1].x = x;
		framingCorners[1].y = -y;

		framingCorners[2].x = -x;
		framingCorners[2].y = -y;

		framingCorners[3].x = -x;
		framingCorners[3].y = y;
	}

	public void ResetFeatherCorners() {
		featherCorners[0].x = 0;
		featherCorners[0].y = 0;

		featherCorners[1].x = 0;
		featherCorners[1].y = 0;

		featherCorners[2].x = 0;
		featherCorners[2].y = 0;

		featherCorners[3].x = 0;
		featherCorners[3].y = 0;
	}

	public Vector3[] GetFeatherCorners() {
		Vector3[] newArray = new Vector3[4];
		for (int i = 0; i < 4; i++) {
			newArray[i] = featherCorners[i] + vertices[i];
		}
		return newArray;
	}

	public void UpdateWorldSpaceCursorPoint() {
		Event currentEvent = Event.current;

		float mouseX = currentEvent.mousePosition.x;
		float mouseY = uiCamera.pixelHeight - currentEvent.mousePosition.y;
		float depth = Mathf.Abs(uiCamera.transform.position.z - transform.position.z);

		Vector2 newMousePosition = new Vector2(mouseX, mouseY);
		lastMouseDelta = lastMousePosition - newMousePosition;
		lastMousePosition = newMousePosition;

		mouseWorldPosition = uiCamera.ScreenToWorldPoint(new Vector3(mouseX, mouseY, depth));
	}

	public int NearestPointTo(Vector3 point, Vector3[] candidates) {
		// Convert point to local space
		Vector3 localPoint = transform.InverseTransformPoint(point);

		float minDistanceSqr = Mathf.Infinity;
		Vector3 nearestVertex = Vector3.zero;
		int chosenIndex = 0;

		// Scan all points to find nearest
		for (int i = 0; i < candidates.Length; i++) {
			Vector3 diff = localPoint - candidates[i];
			float distSqr = diff.sqrMagnitude;
			if (distSqr < minDistanceSqr) {
				minDistanceSqr = distSqr;
				nearestVertex = candidates[i];
				chosenIndex = i;
			}
		}

		// Convert nearest vertex back to world space
		return chosenIndex;
	}

	bool InsideTriangle(Vector2 pt, Vector2 a, Vector2 b, Vector2 c) {
		float area = 0.5f *(-b.y*c.x + a.y*(-b.x + c.x) + a.x*(b.y - c.y) + b.x*c.y);
		float s = 1 / (2 * area) * (a.y*c.x - a.x*c.y + (c.y - a.y)*pt.x + (a.x - c.x)*pt.y);
		float t = 1 / (2 * area) * (a.x*b.y - a.y*b.x + (a.y - b.y)*pt.x + (b.x - a.x)*pt.y);
		return (s >= 0) && (t >= 0) && (1 - s - t >= 0);
	}

	void DetectMouseNearKeystoneCorner() {
		if (nearbyCornerPoint) {
			keystoneDotsLineRenderer.enabled = true;
			DrawCircle(vertices[selectedCornerPointIndex], keystoneDotsLineRenderer);
		}
		else {
			keystoneDotsLineRenderer.enabled = false;
		}
	}

	void DetectMouseNearFeatherCorner() {
		if (nearbyFeatherPoint) {
			featherDotsLineRenderer.enabled = true;
			DrawCircle(GetFeatherCorners()[selectedFeatherPointIndex], featherDotsLineRenderer);
		}
		else {
			featherDotsLineRenderer.enabled = false;
		}
	}

	void DrawCircle(Vector3 center, LineRenderer renderer) {
		float angle = 0;
		for (int i = 0; i < renderer.positionCount; i++) {
			float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius + center.x;
			float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius + center.y;

			renderer.SetPosition(i, new Vector3(x, y, 0));

			angle += (360f / renderer.positionCount);
		}
	}
}
