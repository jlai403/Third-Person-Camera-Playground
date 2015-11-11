using UnityEngine;
using System.Collections;

public class CharacterLogicController : MonoBehaviour {

	[SerializeField]
	private Animator animator;
	[SerializeField]
	private float directionDampTime = .25f;

	private float speed = 0.0f;
	private float horizontal = 0.0f;
	private float vertical = 0.0f;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();

		if (animator.layerCount >= 2) {
			animator.SetLayerWeight (1, 1);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (animator) {
			horizontal = Input.GetAxis("Horizontal");
			vertical = Input.GetAxis("Vertical");
			speed = new Vector2(horizontal, vertical).sqrMagnitude;

			Debug.Log ("horizontal: " + horizontal);
			Debug.Log ("vertical: " + vertical);

			animator.SetFloat("Speed", speed);
			animator.SetFloat("Direction", horizontal, directionDampTime, Time.deltaTime);
		}
	}
}
