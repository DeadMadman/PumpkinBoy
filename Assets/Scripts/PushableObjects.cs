using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class PushableObjects : MonoBehaviour, ITriggerObject
{

	private Vector3 startPosition;
	
	[SerializeField] private MMFeedbacks impactFeedbacks;
	private Rigidbody rb;
	[SerializeField] private GameObject buttonSprite;

	private bool isHeld = false;
	private BoxCollider boxCollider;

	private bool playerIsClose = false;

	private RaycastHit[] hits;
	private bool foundBody = false;
	[SerializeField]private float boxCastSize = 2.1f;

	[SerializeField] private Sprite keyboardSprite;
	[SerializeField] private Sprite gamepadSprite;
	
	public void ResetBoxToOriginalPosition()
    {
		transform.position = startPosition;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		StopHolding();
    }

	public void StartHolding()
    {
		rb.useGravity = false;
		rb.constraints = RigidbodyConstraints.FreezeAll;
		isHeld = true;
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		rb.isKinematic = true;
		transform.rotation = Quaternion.identity;
		//boxCollider.enabled = false;
    }

	public void StopHolding()
    {
		rb.useGravity = true;
		rb.constraints = RigidbodyConstraints.None;
		isHeld = false;
		rb.isKinematic = false;
		//boxCollider.enabled = true;
		rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		boxCollider = GetComponent<BoxCollider>();
		startPosition = transform.position;
	}


	public Vector3 BoxSize => boxCollider.size;

	private void OnCollisionEnter(Collision other)
	{
		if (other.relativeVelocity.magnitude > 3f && !other.gameObject.CompareTag("Body") && !other.gameObject.CompareTag("Head"))
			impactFeedbacks?.PlayFeedbacks();
	}
	private void FixedUpdate()
	{
		ButtonSpriteCollider();
	}

	private void ButtonSpriteCollider()
	{
		hits = Physics.BoxCastAll(transform.position, transform.localScale * boxCastSize, transform.forward);

        for (int i = 0; i < hits.Length; i++)
        {
			//Debug.Log(hits[i].transform.name);
			if (hits[i].transform.tag == "Body")
			{
				foundBody = true;
				buttonSprite.transform.position = transform.position + Vector3.up * 2.5f;
				buttonSprite.transform.LookAt(Camera.main.transform);
				break;
			}
            else
				foundBody = false;
		}
        if (foundBody)
        {
	        if (buttonSprite != null)
	        {
		        bool isController = PlayerPrefs.GetInt("Controller") == 1;
		        buttonSprite.GetComponentInChildren<SpriteRenderer>().sprite = isController ? gamepadSprite : keyboardSprite; 
				buttonSprite.SetActive(!isHeld);
	        }
        }
        else if (!foundBody)
        {
			buttonSprite.SetActive(false);
		}
	}
}
