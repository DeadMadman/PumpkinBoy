using System;
using System.Collections;
using UnityEngine;
using MoreMountains.Feedbacks;

[SelectionBase]
public class TransformEvent : EaseControllerBase
{
	[SerializeField] private int id;

	[Space(10)]
	[SerializeField] float easingTime;
	[SerializeField] EasingUtility.Style easingStyle;

	enum LoopStyle
	{
		Once,
		PingPongPos,
		PingPongInfinite,
		ReturnPosOnExit
	}
	[SerializeField] private LoopStyle loopStyle;

	[Header("Easing postition")]
	private Vector3 startPos;
	[SerializeField] private Vector3 endPos;
	private Vector3 aPos;
	private Vector3 bPos;

	[Header("Easing rotation")]
	private Vector3 startRot;
	[SerializeField] private Vector3 endRot;
	
	private Vector3 aRot;
	private Vector3 bRot;

	[Space(10)] 
	[SerializeField] private bool DisablePushingBox;

	[Space(10)]
	[SerializeField] private bool onlyPlayOnEnter = true;
	[SerializeField] private bool stopOnExit = false;
	[SerializeField] private MMFeedbacks activationFeedbacks;
	
	private void Awake()
	{
		startPos = transform.position;
		startRot = transform.rotation.eulerAngles;
		endPos += startPos;

		aPos = endPos;
		bPos = startPos;
		aRot = endRot + startRot;
		bRot = startRot;
	}

	void Start()
	{
		GameEventManager.current.onTriggerEnter += OnTransformTriggerEnter; // adds the event to the gameEvents
		GameEventManager.current.onTriggerExit += OnTransformTriggerExit; // adds the event to the gameEvents
	}

	private void OnDestroy()
	{
		GameEventManager.current.onTriggerEnter -= OnTransformTriggerEnter; // remove the event to the gameEvents
		GameEventManager.current.onTriggerExit -= OnTransformTriggerExit; // remove the event to the gameEvents
	}

	private void OnTransformTriggerEnter(int id) // moves the door with the same id as the trigger x distance
	{
		if (id == this.id)
		{
			activationFeedbacks?.PlayFeedbacks();
			if (loopStyle == LoopStyle.ReturnPosOnExit)
			{
				SetReverse(false);
				Play();
			}
			else
			{
				Play();
			}
		}
	}
	private void OnTransformTriggerExit(int id) // moves the door with the same id as the trigger x distance
	{
		if (id == this.id)
		{
			if (!onlyPlayOnEnter && !stopOnExit)
			{
				activationFeedbacks?.PlayFeedbacks();
			}
			if (stopOnExit)
			{
				activationFeedbacks?.StopFeedbacks();
			}
			
			if (loopStyle == LoopStyle.ReturnPosOnExit)
			{
				SetReverse(true);
				Play();
			}
			else
			{
				Play();
			}
		}
	}

	public override void OnStart()
	{
		SetStyle(easingStyle);
		SetDuration(easingTime);

		if (loopStyle == LoopStyle.PingPongInfinite)
		{
			(aPos, bPos) = (bPos, aPos);
			(aRot, bRot) = (bRot, aRot);
		}
		else if (loopStyle == LoopStyle.ReturnPosOnExit)
		{
			aPos = startPos;
			bPos = endPos;

			aRot = transform.rotation.eulerAngles;
			bRot = endRot;
			bRot += aRot;
		}
		else
		{
			aPos = transform.position;
			bPos = endPos;

			aRot = transform.rotation.eulerAngles;
			bRot = endRot;
			bRot += aRot;
		}
	}
	public override void Evaluate(float t)
	{
		transform.position = EasingUtility.Interpolate(aPos, bPos, t);
		transform.rotation = Quaternion.Euler(EasingUtility.Interpolate(aRot, bRot, t));
	}
	public override void OnEnd()
	{
		if (loopStyle == LoopStyle.PingPongInfinite)
		{
			Play();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (loopStyle == LoopStyle.ReturnPosOnExit && other.CompareTag("Holdable item"))
		{
			if (!DisablePushingBox)
			{
				Pause();
			}
		}
	}
}
