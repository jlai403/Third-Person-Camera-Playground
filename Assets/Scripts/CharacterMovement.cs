using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {

	[SerializeField]
	private Animator animator;
	[SerializeField]
	private MainCamera gameCamera;
	[SerializeField]
	private float directionDampTime = .25f;
	[SerializeField]
	private float directionSpeed = 1.5f;
	[SerializeField]
	public float rotationDegreesPerSec = 120f;

	private float direction = 0.0f;
	private float speed = 0.0f;
	private float horizontal = 0.0f;
	private float vertical = 0.0f;

	private int m_LocomotionId = 0;
	private AnimatorStateInfo animatorStateInfo;

	void Start () {
		animator = GetComponent<Animator> ();

		animator.GetCurrentAnimatorStateInfo (0);

		if (animator.layerCount >= 2) {
			animator.SetLayerWeight (1, 1);
		}

		m_LocomotionId = Animator.StringToHash ("Base Layer.Locomotion");
	}

	void Update () {
		if (animator) {
			horizontal = Input.GetAxis("Horizontal");
			vertical = Input.GetAxis("Vertical");

			AdjustToCamera(this.transform, gameCamera.transform, ref direction, ref speed); 

			animator.SetFloat("Speed", speed);
			animator.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);


		}
	}

	void FixedUpdate() {
		var movingLeftOrRight = (direction >= 0 && horizontal >= 0) || (direction <=0 && horizontal <=0);
		if (IsInLocomotion() && movingLeftOrRight) {
			Turn(horizontal);
		}
	}

	public void Turn(float horizontal) {
		var horizontalRotation = rotationDegreesPerSec * (horizontal < 0f ? -1f : 1f);
		Vector3 rotationAmount = Vector3.Lerp (Vector3.zero, new Vector3 (0f, horizontalRotation, 0f), Mathf.Abs (horizontal)); 
		Quaternion deltaRotation = Quaternion.Euler (rotationAmount * Time.deltaTime);
		this.transform.rotation = this.transform.rotation * deltaRotation;
	}

	public bool IsInLocomotion() {
		return animatorStateInfo.fullPathHash == m_LocomotionId;
	}

	public void AdjustToCamera(Transform root, Transform camera, ref float directionOut, ref float speedOut) {
		Vector3 rootDirection = root.forward;
		Vector3 mouseDirection = new Vector3 (horizontal, 0, vertical);

		speedOut = mouseDirection.sqrMagnitude;

		// get camera rotation
		Vector3 cameraDirection = camera.forward;	
		cameraDirection.y = 0.0f;
		Quaternion referentialShift = Quaternion.FromToRotation (Vector3.forward, cameraDirection);

		Vector3 moveDirection = referentialShift * mouseDirection;
		Vector3 directionAxis = Vector3.Cross (moveDirection, rootDirection);

		Debug.DrawRay (new Vector3 (root.position.x, root.position.y + 2f, root.position.z), moveDirection, Color.green);
		Debug.DrawRay (new Vector3 (root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
		Debug.DrawRay (new Vector3 (root.position.x, root.position.y + 2f, root.position.z), mouseDirection, Color.blue);

		float angleRootToMove = Vector3.Angle (rootDirection, moveDirection) * (directionAxis.y >= 0 ? -1f : 1f);
		angleRootToMove /= 180f;
		directionOut = angleRootToMove * directionSpeed;
	}
}
