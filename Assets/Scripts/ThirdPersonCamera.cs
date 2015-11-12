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
		Vector3 characterOffset = characterFollower.position + new Vector3(0f, distanceUp, 0f);

		lookDirection = characterOffset - this.transform.position;
		lookDirection.y = 0;
		lookDirection.Normalize();

		targetPosition = characterFollower.position + characterFollower.up * distanceUp - lookDirection * distanceAway;
		CompensateForWalls (characterOffset, ref targetPosition);
		SmoothPosition (this.transform.position, targetPosition);
		this.transform.LookAt (characterFollower);
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




