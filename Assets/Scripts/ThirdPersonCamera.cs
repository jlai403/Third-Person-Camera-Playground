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

	// Use this for initialization
	void Start () {
		characterFollower = GameObject.FindWithTag ("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate() {
		targetPosition = characterFollower.position + characterFollower.up * distanceUp - characterFollower.forward * distanceAway;
		Debug.DrawRay (characterFollower.position, Vector3.up * distanceUp, Color.red);
		Debug.DrawRay (characterFollower.position, -characterFollower.forward * distanceAway, Color.blue);
		Debug.DrawLine(characterFollower.position, targetPosition, Color.magenta);

		this.transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * smooth);
		this.transform.LookAt (characterFollower);
	}
}
