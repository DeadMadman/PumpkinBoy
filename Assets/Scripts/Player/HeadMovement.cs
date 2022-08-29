using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;

public class HeadMovement : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 1f;
	[SerializeField] private MMFeedbacks impactFeedback;
	public static bool headAttached = false;

	private Rigidbody rb;
	private Vector3 movement;
	private TrailRenderer trailRenderer;

	private bool isGrounded;

	public void DisableTrailRenderer()
    {
		trailRenderer.emitting = false;
    }

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		trailRenderer = GetComponent<TrailRenderer>();
	}
	public void RegisterInputs(SimpleControls controls)
	{
		controls.Gameplay.MoveHead.performed += OnMoveHead;
		controls.Gameplay.MoveHead.canceled += OnMoveHead;
	}
	public void DeregisterInputs(SimpleControls controls)
	{
		controls.Gameplay.MoveHead.performed -= OnMoveHead;
		controls.Gameplay.MoveHead.canceled -= OnMoveHead;
	}

	public void StopMovement()
	{
		movement = Vector3.zero;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}

	private void FixedUpdate()
	{
		if (headAttached)
			return;

		isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.7f, 1);
		if (isGrounded)
		{
			if (movement.sqrMagnitude == 0)
			{
				Vector2 xAndZVelocity = new Vector2(rb.velocity.x, rb.velocity.z);
				if (rb.velocity.sqrMagnitude > 2f)
				{
					rb.velocity -= new Vector3(xAndZVelocity.x, 0, xAndZVelocity.y) * Time.deltaTime;
				}
				else
				{
					if (rb.velocity.y < 0.5f)
					{
						rb.velocity = Vector3.zero;
						rb.angularVelocity = Vector3.zero;
					}
				}
			}
			else
			{
				if (rb.velocity.sqrMagnitude < 23f)
					rb.AddForce(movement);
			}
			trailRenderer.emitting = false;
		}
		else
		{
			trailRenderer.emitting = true;
		}
	}

	private void OnMoveHead(InputAction.CallbackContext context)
	{
		Vector3 moveDir = context.ReadValue<Vector2>();
		movement = new Vector3(moveDir.x, 0, moveDir.y).normalized * moveSpeed;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!headAttached)
		{
			if (collision.relativeVelocity.magnitude > 4f && !collision.gameObject.CompareTag("Body"))
			{
				impactFeedback?.PlayFeedbacks();
			}
		}
	}
}
