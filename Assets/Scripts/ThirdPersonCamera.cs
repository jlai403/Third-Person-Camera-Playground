using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {

	[SerializeField]
	private float distanceAway;
	[SerializeField]
	private float distanceUp;
	[SerializeField]
	private float smooth;
	[SerializeField]
	private Transform characterFollower;
	[SerializeField]
	private Vector3 offset = new Vector3 (0f, 1.5f, 0f);

	private Vector3 targetPosition;
	private Vector3 lookDirection;

	private Vector3 velocityCamSmooth = Vector3.zero;
	[SerializeField]
	private float camSmoothDampTime = 0.1f;


	// Use this for initialization
	void Start () {
		characterFollower = GameObject.FindWithTag ("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate() {
		Vector3 characterOffset = characterFollower.position + offset;

		lookDirection = characterOffset - this.transform.position;
		lookDirection.y = 0;
		lookDirection.Normalize();

		targetPosition = characterFollower.position + characterFollower.up * distanceUp - lookDirection * distanceAway;
	
		smoothPosition (this.transform.position, targetPosition);
		this.transform.LookAt (characterFollower);
	}

	private void smoothPosition(Vector3 from, Vector3 to) {
		this.transform.position = Vector3.SmoothDamp (from, to, ref velocityCamSmooth, camSmoothDampTime);
	}
}
