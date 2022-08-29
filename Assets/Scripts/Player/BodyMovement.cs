using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;

public class BodyMovement : MonoBehaviour
{
	private PlayerController playerController;
	private Animator animator;
	[SerializeField] private Transform headTransform;
	[SerializeField, Tooltip("Regular move speed")] private float moveSpeed = 5f;
	[SerializeField, Tooltip("Speed while moving a block")] private float grabMoveSpeed = 2.5f;
	[SerializeField, Tooltip("How quickly the character is aple to turn")] private float rotationSpeed = 20f;

	[SerializeField] private MMFeedbacks walkFeedbacks;

	[SerializeField] private Transform holdTransform;

	private new Transform camera;

	public UnityEvent startedPushBoxEvent;

	private float gravityChangeValue = 9.81f;
	private float gravityCurrent = 0f;
	private float gravityMax = 27f;

	private CharacterController controller;

	private Vector2 leftInputVector;
	private Vector2 rightInputVector;
	private Vector3 velocity;
	private Vector3 moveDir;
	private Vector3 rotationDir;


	private bool isGrounded = false;
	private bool isHeadColliding = false;

	private bool grabHeld = false;
	private PushableObjects pushableObject;
	private Vector3 pushableObjectSize;
	private Vector3 objectOffset;

	private float dropDelay = 0.25f;
	private float dropTimer = 0;

	private AudioSource audioSource;

	private int pushingBlockHash;
	private int walkingHash;
	private int groundedHash;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		startedPushBoxEvent = new UnityEvent();
		playerController = GetComponent<PlayerController>();
		animator = GetComponentInChildren<Animator>();
		controller = GetComponent<CharacterController>();
		camera = Camera.main.transform;
		pushingBlockHash = Animator.StringToHash("isPushingBlock");
		walkingHash = Animator.StringToHash("isWalking");
		groundedHash = Animator.StringToHash("isGrounded");
	}
	public void RegisterHead(SimpleControls controls)
	{
		controls.Gameplay.MoveHead.performed += OnMoveHead;
		controls.Gameplay.MoveHead.canceled += OnMoveHead;
	}

	public void RegisterMovement(SimpleControls controls)
	{
		controls.Gameplay.MoveBody.performed += OnMoveBody;
		controls.Gameplay.MoveBody.canceled += OnMoveBody;
	}

	public void DeregisterHead(SimpleControls controls)
	{
		controls.Gameplay.MoveHead.performed -= OnMoveHead;
		controls.Gameplay.MoveHead.canceled -= OnMoveHead;
		rightInputVector = Vector2.zero;
	}

	public void DeregisterMovement(SimpleControls controls)
	{
		if (walkFeedbacks != null)
			walkFeedbacks.StopFeedbacks();

		controls.Gameplay.MoveBody.performed -= OnMoveBody;
		controls.Gameplay.MoveBody.canceled -= OnMoveBody;
		leftInputVector = Vector2.zero;
	}

	public void RegisterGrab(SimpleControls controls)
	{
		controls.Gameplay.PickUp.started += OnPickup;
		controls.Gameplay.PickUp.canceled += OnPickup;
	}
	public void DeregisterGrab(SimpleControls controls)
	{
		controls.Gameplay.PickUp.started -= OnPickup;
		controls.Gameplay.PickUp.canceled -= OnPickup;
		grabHeld = false;
	}

	public void StopMovement()
	{
		if (walkFeedbacks != null)
			walkFeedbacks.StopFeedbacks();

		velocity = Vector3.zero;
		leftInputVector = Vector2.zero;
		rightInputVector = Vector2.zero;
	}
	
	int stepsSinceLastGrounded;
	
	private void FixedUpdate()
	{
		stepsSinceLastGrounded += 1;
		
		//cc isGrounded is unreliable, check with raycast to make sure
		if (controller.isGrounded)
			isGrounded = true;
		else
			isGrounded = Physics.Raycast(transform.position, -transform.up, out RaycastHit _, 1.1f, 
				1, QueryTriggerInteraction.Ignore);
		
		if (isGrounded)
		{
			stepsSinceLastGrounded = 0;
		}

		if ( stepsSinceLastGrounded > 2)
		{
			animator.SetBool(groundedHash, false);
		}
		else
		{
			animator.SetBool(groundedHash, true);
		}

		if (!isGrounded)
			return;

		if (!grabHeld)
		{
			if (pushableObject)
				DropObject();

			return;
		}

		if (dropTimer > 0)
			return;

		//disable head rb check parent rigidbody head

		//you are grounded, holding grab, and head is not attached
		if (pushableObject == null)
		{
			RaycastHit[] hits = Physics.SphereCastAll(transform.position, 0.25f, transform.forward, 0.9f, 1, QueryTriggerInteraction.Ignore);
			foreach (RaycastHit hit in hits)
			{

				if (hit.collider.gameObject.TryGetComponent(out PushableObjects pO))
				{

					pushableObject = pO;
					pushableObjectSize = pushableObject.BoxSize;
					pushableObject.StartHolding();
					objectOffset = pushableObject.transform.position - transform.position + new Vector3(0, 0.1f, 0) + transform.forward * 0.1f;
					int layerMask = LayerMask.GetMask(new string[] {"Default"});

					Collider[] colliders = Physics.OverlapBox(transform.position + objectOffset, pushableObjectSize / 2, Quaternion.identity, layerMask, QueryTriggerInteraction.Ignore);

					foreach (var collider in colliders)
					{
                        
						if (collider.gameObject == pushableObject.gameObject)
							continue;
						else
						{

							transform.position -= transform.forward * 0.1f;
							objectOffset = pushableObject.transform.position - transform.position + new Vector3(0, 0.1f, 0);
							break;
						}
					}

					pushableObject.transform.position = transform.position + objectOffset;
					playerController.isHoldingBox = true;
					startedPushBoxEvent.Invoke();
					
					animator.SetBool(pushingBlockHash, true);
					return;
				}
			}
		}
	}


	private void Update()
	{
		RotateCharecter();
		RegularMovement();
	}

	private void RotateCharecter()
    {
		if (rightInputVector != Vector2.zero && leftInputVector == Vector2.zero && HeadMovement.headAttached)
        {
			rotationDir = (Vector3.right * rightInputVector.x + Vector3.forward * rightInputVector.y).normalized;
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rotationDir), Time.deltaTime * rotationSpeed);
        }
	}

	private void RegularMovement()
	{
		if (leftInputVector != Vector2.zero)
			moveDir = (Vector3.right * leftInputVector.x + Vector3.forward * leftInputVector.y).normalized;
		else
			moveDir = Vector2.zero;
		
		velocity = moveDir * moveSpeed;
		velocity = AdjustVelocityToSlope(velocity);
		
		animator.SetBool(walkingHash, velocity != Vector3.zero);
		
		//handle rotation if not grabbing an object
		if (velocity != Vector3.zero && pushableObject == null)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * rotationSpeed);
		}
		if (isGrounded)
		{
			gravityCurrent = 0;
			if (pushableObject != null)
			{
				velocity = moveDir * grabMoveSpeed;
				Vector3 testVelocity = velocity * Time.deltaTime;

				int layerMask = LayerMask.GetMask(new string[] {"Default"});
				Collider[] colliders = Physics.OverlapBox(transform.position + objectOffset + testVelocity, pushableObjectSize / 2, Quaternion.identity, layerMask, QueryTriggerInteraction.Ignore);
                
                foreach (var collider in colliders)
                {
					if (collider.gameObject == pushableObject.gameObject)
                    {
						continue;
					}
                    else
                    {
						velocity = Vector3.zero;
                        break;
                    }
                }
                pushableObject.transform.position = transform.position + objectOffset;

			}
			//check if head is colliding
			if (headTransform.parent == transform)
			{
				float headRadius = 0.7f;
				isHeadColliding = Physics.Raycast(headTransform.position, moveDir, out RaycastHit hit, headRadius, 1, QueryTriggerInteraction.Ignore);
				if (isHeadColliding)
					velocity = Vector3.zero;
			}
		}
		else
		{
			if (walkFeedbacks != null)
				walkFeedbacks.StopFeedbacks();

			if (pushableObject)
			{
				DropObject();
			}
			gravityCurrent += gravityChangeValue * Time.deltaTime;
			gravityCurrent = Mathf.Clamp(gravityCurrent, 0, gravityMax);
			velocity += -gravityCurrent * transform.up;
		}

		//time before you can move after letting go off a box
		if (dropTimer > 0)
			dropTimer -= Time.deltaTime;
		else
		{
			if (velocity != Vector3.zero)
            {
	            controller.Move(velocity * Time.deltaTime);
				
				if (!isGrounded)
				{
					if (walkFeedbacks != null)
						walkFeedbacks.StopFeedbacks();
				}
				else
                {
					if (walkFeedbacks != null)
					{
						if (!walkFeedbacks.IsPlaying)
						{
							walkFeedbacks.PlayFeedbacks();
						}
					}
				}
			}
			else
            {
                if (walkFeedbacks != null)
                    walkFeedbacks.StopFeedbacks();

				if (!isGrounded)
                {
					if (walkFeedbacks != null)
						walkFeedbacks.StopFeedbacks();
				}
                animator.SetBool(walkingHash, false);
			}
		}
	}
	
	private Vector3 AdjustVelocityToSlope(Vector3 velocity)
	{
		var ray = new Ray(transform.position + velocity * Time.deltaTime, Vector3.down);

		if (Physics.Raycast(ray, out RaycastHit hitInfo, controller.height + 5))
		{
			var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
			var adjustedVelocity = slopeRotation * velocity;

			if (adjustedVelocity.y < 0)
			{
				
				return adjustedVelocity;
			}
		}

		return velocity;
	}

	void DropObject()
	{
		//pushableObject.transform.position += transform.forward * 0.1f;
		pushableObject.transform.position = transform.position + objectOffset;
		if (!isGrounded)
			pushableObject.transform.position += transform.forward * 0.3f;

		pushableObject.StopHolding();
		pushableObject.transform.parent = null;
		pushableObject = null;
		playerController.isHoldingBox = false;
		dropTimer = dropDelay;
		animator.SetBool(pushingBlockHash, false);
	}

	private Vector2 MakeInput8Dir(Vector2 inputVector)
	{
		float x = inputVector.x;
		float y = inputVector.y;
		if (x > 0)
		{
			if (x > 0.5f)
				x = 1f;
			else if (x > 0.3f)
				x = 0.5f;
			else
				x = 0;
		}
		else if (x < 0)
		{
			if (x < -0.5f)
				x = -1f;
			else if (x < -0.3f)
				x = -0.5f;
			else
				x = 0;
		}

		if (y > 0)
		{
			if (y > 0.5f)
				y = 1f;
			else if (y > 0.3f)
				y = 0.5f;
			else
				y = 0;
		}
		else if (y < 0)
		{
			if (y < -0.5f)
				y = -1f;
			else if (y < -0.3f)
				y = -0.5f;
			else
				y = 0;
		}

		inputVector.x = x;
		inputVector.y = y;
		return inputVector;
	}

	public void OnPickup(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			grabHeld = true;
		}
		else if (context.canceled)
		{
			if (pushableObject != null)
			{
				DropObject();
			}
			grabHeld = false;
		}
	}

	public void OnMoveHead(InputAction.CallbackContext context)
	{
		if (!context.canceled)
		{
			rightInputVector = context.ReadValue<Vector2>();
		}
		else
		{
			rightInputVector = Vector2.zero;
		}
	}

	public void OnMoveBody(InputAction.CallbackContext context)
	{
		if (!context.canceled)
		{
			leftInputVector = context.ReadValue<Vector2>();
		}
		else
		{
			leftInputVector = Vector2.zero;
		}
	}
}
