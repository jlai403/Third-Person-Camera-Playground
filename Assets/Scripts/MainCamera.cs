using UnityEngine;
using System.Collections;

struct CameraPosition {
	private Vector3 position;
	private Transform gameObject;

	public Vector3 Position {
		get { return this.position; }
		set { this.position = value; }
	}

	public Transform GameObject {
		get { return this.gameObject; }
		set { this.gameObject = value; }
	}

	public void Init(string camName, Vector3 position, Transform transform, Transform parent) {
		this.position = position;
		this.gameObject = transform;
		this.gameObject.name = camName;
		this.gameObject.SetParent (parent);
		this.gameObject.localPosition = Vector3.zero;
		this.gameObject.localPosition = position;
	}
}

public class MainCamera : MonoBehaviour {

	[SerializeField]
	private float thirdPersonDistanceAway;
	[SerializeField]
	private float thirdPersonDistanceUp;
	[SerializeField]
	private float smooth;
	[SerializeField]
	private CharacterMovement characterMovement;
	[SerializeField]
	private Transform characterFollower;
	[SerializeField]
	private float camSmoothDampTime = 0.1f;
	
	private Vector3 cameraPosition;
	private Vector3 velocityCamSmooth = Vector3.zero;
	
	// First Person Camera
	private CameraPosition firstPersonCameraPosition;
	private float xAxisRotation = 0.0f;
	private float yAxisRotation = 0.0f;
	[SerializeField]
	private float firstPersonLookSpeed = 1.5f;
	[SerializeField]
	private Vector2 firstPersonXAxisClamp = new Vector2 (-35.0f, 35.0f);

	// Use this for initialization
	void Start () {
		characterMovement = GameObject.FindWithTag ("Player").GetComponent<CharacterMovement> ();
		characterFollower = GameObject.FindWithTag ("Player").transform;

		firstPersonCameraPosition = new CameraPosition ();
		firstPersonCameraPosition.Init (
			"First Person Camera", 
			new Vector3 (0f, 1.7f, 0.5f), 
			new GameObject ().transform, 
			characterFollower
		);
	}

	void LateUpdate() {
		Vector3 cameraView = Vector3.zero;

		if (IsFirstPersonViewTriggered()) {
			cameraView = FirstPersonCameraView();
		} else {
			ResetCamera();			
			cameraView = ThirdPersonCameraView();
		}

		SmoothPosition (this.transform.position, cameraPosition);
		this.transform.LookAt (cameraView);
	}

	private bool IsFirstPersonViewTriggered() {
		return Input.GetKey (KeyCode.Mouse1);
	}

	private Vector3 ThirdPersonCameraView() {
		Vector3 characterOffset = characterFollower.position + new Vector3(0f, thirdPersonDistanceUp, 0f);
		cameraPosition = characterOffset + characterFollower.up * thirdPersonDistanceUp - characterFollower.forward * thirdPersonDistanceAway;
		CompensateForWalls (characterOffset, ref cameraPosition);
		return characterOffset;
	}

	private Vector3 FirstPersonCameraView() {
		// left/right
		float mouseX = Input.GetAxisRaw ("Mouse X");
		yAxisRotation += (mouseX * 0.5f * firstPersonLookSpeed);
		yAxisRotation = Mathf.Clamp (yAxisRotation, firstPersonXAxisClamp.x, firstPersonXAxisClamp.y);
		characterMovement.Turn (mouseX);

		// up/down
		float mouseY = Input.GetAxisRaw ("Mouse Y");
		xAxisRotation -= (mouseY * 0.5f * firstPersonLookSpeed);
		firstPersonCameraPosition.GameObject.localRotation = Quaternion.Euler (xAxisRotation, 0, 0);
		Quaternion rotationShift = Quaternion.FromToRotation (this.transform.forward, firstPersonCameraPosition.GameObject.forward);
		this.transform.rotation = rotationShift * this.transform.rotation;
		
		cameraPosition = firstPersonCameraPosition.GameObject.position;
		
		Vector3 cameraView = Vector3.Lerp (cameraPosition + characterFollower.forward, this.transform.position + this.transform.forward, camSmoothDampTime + Time.deltaTime);
		var firstPersonCameraDistance = Vector3.Distance (this.transform.position, firstPersonCameraPosition.GameObject.position);
		cameraView = Vector3.Lerp (this.transform.position + this.transform.forward, cameraView, firstPersonCameraDistance);
		return cameraView;
	}

	private void ResetCamera() {
		this.transform.localRotation = Quaternion.Lerp (transform.localRotation, Quaternion.identity, Time.deltaTime);
	}

	private void SmoothPosition(Vector3 from, Vector3 to) {
		this.transform.position = Vector3.SmoothDamp (from, to, ref velocityCamSmooth, camSmoothDampTime);
	}

	private void CompensateForWalls(Vector3 from, ref Vector3 to) {
		RaycastHit wallHit = new RaycastHit ();
		if (Physics.Linecast (from, to, out wallHit)) {
			to = new Vector3(wallHit.point.x, to.y, wallHit.point.z);
		}
	}
}




