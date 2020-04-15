using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Keystoning : MonoBehaviour {
	public float scaleFactor = 0.001f;
	public RenderTexture[] virtualProjectorRenderTextures;
	public int projectorNumber = 1;

	Camera uiCamera;
	Mesh mesh;
	Material mat;
	Vector3[] vertices;
	Vector3[] corners;
	LineRenderer selectionLineRenderer;
	LineRenderer frameLineRenderer;
	Camera cam;
	float radius = 0.02f;
	float thickness = 0.2f;
	int selectedPointIndex;
	bool nearPoint = false;
	bool dragging = false;
	Vector3 mouseWorldPosition;
	Vector2 nativeResolution;
	GameObject selectionLines;
	GameObject frameLines;
	GameObject outputCamera;

	// TODO: Add GUI to control camera X/Y/Z and heading/pitch/roll. Eventually add feathering and bilinear interpolation.

	void Start() {
		uiCamera = transform.parent.Find("Management Window Camera").GetComponent<Camera>();

		mat = GetComponent<MeshRenderer>().material;
		nativeResolution = new Vector2(mat.mainTexture.width, mat.mainTexture.height);

		selectionLines = transform.Find("Selection Lines").gameObject;
		frameLines = transform.Find("Frame Lines").gameObject;
		outputCamera = transform.Find("Output Camera").gameObject;

		mesh = GetComponent<MeshFilter>().mesh;
		vertices = mesh.vertices;
		UpdateVertices();

		corners = new Vector3[4];
		UpdateCorners();

		cam = outputCamera.GetComponent<Camera>();
		cam.orthographicSize = scaleFactor * nativeResolution.x * 0.5f;

		selectionLineRenderer = selectionLines.GetComponent<LineRenderer>();
		selectionLineRenderer.widthMultiplier = thickness;
		selectionLineRenderer.positionCount = 12;
		selectionLineRenderer.useWorldSpace = false;
		selectionLineRenderer.loop = true;

		frameLineRenderer = frameLines.GetComponent<LineRenderer>();
		frameLineRenderer.widthMultiplier = thickness;
		frameLineRenderer.positionCount = 4;
		frameLineRenderer.useWorldSpace = false;
		frameLineRenderer.loop = true;
	}

	void Update() {
		cam.targetDisplay = projectorNumber;
		mat.mainTexture = virtualProjectorRenderTextures[projectorNumber - 1];

		UpdateCorners();

		if (Input.GetKeyDown(KeyCode.Mouse0) && nearPoint) {
			dragging = true;
		}

		if (Input.GetKeyUp(KeyCode.Mouse0)) {
			dragging = false;
		}
		
		if (dragging) {
			vertices[selectedPointIndex] = mouseWorldPosition - transform.position;
		}

		DrawCircle(vertices[selectedPointIndex]);
		DrawFrame();
		
		// vertices = mesh.vertices;
		// normals = mesh.normals;

		// for (var i = 0; i < vertices.Length; i++) {
		// 	Vector3 direction = new Vector3(0, Mathf.Sin(Time.time) * 0.01f, 0);
		// 	vertices[i] += direction;
		// }

		mesh.vertices = vertices;
	}

	void OnGUI() {
		UpdateWorldSpaceCursorPoint();

		selectedPointIndex = NearestVertexTo(mouseWorldPosition);
		Vector3 selectedPoint = vertices[selectedPointIndex];

		nearPoint = Vector3.Distance(selectedPoint, mouseWorldPosition - transform.position) < 0.05f;
	}

	public void UpdateVertices() {
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

	public void UpdateCorners() {
		float x = nativeResolution.x * scaleFactor * 0.5f;
		float y = nativeResolution.y * scaleFactor * 0.5f;

		corners[0].x = x;
		corners[0].y = y;

		corners[1].x = x;
		corners[1].y = -y;

		corners[2].x = -x;
		corners[2].y = -y;

		corners[3].x = -x;
		corners[3].y = y;
	}

	public void UpdateWorldSpaceCursorPoint() {
		Event currentEvent = Event.current;

		float mouseX = currentEvent.mousePosition.x;
		float mouseY = uiCamera.pixelHeight - currentEvent.mousePosition.y;
		float depth = Mathf.Abs(uiCamera.transform.position.z - transform.position.z);

		mouseWorldPosition = uiCamera.ScreenToWorldPoint(new Vector3(mouseX, mouseY, depth));
	}

	public int NearestVertexTo(Vector3 point) {
		// Convert point to local space
		Vector3 localPoint = transform.InverseTransformPoint(point);

		float minDistanceSqr = Mathf.Infinity;
		Vector3 nearestVertex = Vector3.zero;
		int chosenIndex = 0;

		// Scan all vertices to find nearest
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 diff = localPoint - vertices[i];
			float distSqr = diff.sqrMagnitude;
			if (distSqr < minDistanceSqr) {
				minDistanceSqr = distSqr;
				nearestVertex = vertices[i];
				chosenIndex = i;
			}
		}

		// Convert nearest vertex back to world space
		return chosenIndex;
	}

	void DrawCircle(Vector3 center) {
		if (nearPoint) {
			selectionLineRenderer.enabled = true;
			
			float angle = 0;
			for (int i = 0; i < selectionLineRenderer.positionCount; i++) {
				float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius + center.x;
				float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius + center.y;

				selectionLineRenderer.SetPosition(i, new Vector3(x, y, 0));

				angle += (360f / selectionLineRenderer.positionCount);
			}
		}
		else {
			selectionLineRenderer.enabled = false;
		}
	}

	void DrawFrame() {
		for (int i = 0; i < corners.Length; i++) {
			frameLineRenderer.SetPosition(i, corners[i]);
		}
	}
}
