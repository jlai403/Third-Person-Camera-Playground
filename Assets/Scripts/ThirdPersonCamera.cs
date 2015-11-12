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

public class ThirdPersonCamera : MonoBehaviour {

	[SerializeField]
	private float distanceAway;
	[SerializeField]
	private float distanceUp;
	[SerializeField]
	private float smooth;
	[SerializeField]
	private CharacterMovement characterMovement;
	[SerializeField]
	private Transform characterFollower;
	[SerializeField]
	private float camSmoothDampTime = 0.1f;
	
	private Vector3 targetPosition;
	private Vector3 lookDirection;
	private CamStates camState = CamStates.Behind;
	private Vector3 velocityCamSmooth = Vector3.zero;
	
	// First Person Camera
	private CameraPosition firstPersonCameraPosition;
	private float xAxisRotation = 0.0f;
	private float yAxisRotation = 0.0f;
	[SerializeField]
	private float firstPersonLookSpeed = 1.5f;
	[SerializeField]
	private Vector2 firstPersonXAxisClamp = new Vector2 (-35.0f, 35.0f);


	public enum CamStates {
		Behind,
		FirstPerson
	}

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
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate() {
		UpdateCamState ();

		Vector3 characterOffset = characterFollower.position + new Vector3(0f, distanceUp, 0f);
		Vector3 lookAt = characterOffset;

		switch (camState) {
		case CamStates.Behind:
			ResetCamera();

			lookDirection = characterOffset - this.transform.position;
			lookDirection.y = 0;
			lookDirection.Normalize();
			targetPosition = characterOffset + characterFollower.up * distanceUp - characterFollower.forward * distanceAway;

			break;
		case CamStates.FirstPerson:
			float mouseX = Input.GetAxisRaw ("Mouse X");
			yAxisRotation += (mouseX * 0.5f * firstPersonLookSpeed);
			yAxisRotation = Mathf.Clamp(yAxisRotation, firstPersonXAxisClamp.x, firstPersonXAxisClamp.y);

			characterMovement.Rotate(mouseX);

			float mouseY = Input.GetAxisRaw ("Mouse Y");
			xAxisRotation -= (mouseY * 0.5f * firstPersonLookSpeed);
			firstPersonCameraPosition.GameObject.localRotation = Quaternion.Euler(xAxisRotation, 0, 0);
			Quaternion rotationShift = Quaternion.FromToRotation(this.transform.forward, firstPersonCameraPosition.GameObject.forward);
			this.transform.rotation = rotationShift * this.transform.rotation;

			targetPosition = firstPersonCameraPosition.GameObject.position;

			lookAt = Vector3.Lerp(targetPosition + characterFollower.forward, this.transform.position + this.transform.forward, camSmoothDampTime + Time.deltaTime);

			var firstPersonCameraDistance = Vector3.Distance(this.transform.position, firstPersonCameraPosition.GameObject.position);
			lookAt = Vector3.Lerp(this.transform.position + this.transform.forward, lookAt, firstPersonCameraDistance);

			break;
		}

		CompensateForWalls (characterOffset, ref targetPosition);
		SmoothPosition (this.transform.position, targetPosition);
		this.transform.LookAt (lookAt);
	}

	private void UpdateCamState() {
		if (Input.GetKey (KeyCode.Mouse1)) {
			camState = CamStates.FirstPerson;
		} else {
			camState = CamStates.Behind;
		}
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




