using UnityEngine;

public class CandyTransform : EaseControllerBase
{
	[SerializeField] float easingTime;
	[SerializeField] EasingUtility.Style easingStyle;

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
	
	private bool playAgain = true;
	private float t;

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

	public override void OnStart()
	{
		SetStyle(easingStyle);
		SetDuration(easingTime);

		(aPos, bPos) = (bPos, aPos);
		
		aRot = transform.rotation.eulerAngles;
		bRot = endRot;
		bRot += aRot;
	}

	public override void Evaluate(float t)
	{
		transform.position = EasingUtility.Interpolate(aPos, bPos, t);
		transform.rotation = Quaternion.Euler(EasingUtility.Interpolate(aRot, bRot, t));
	}

	public override void OnEnd()
	{
		playAgain = true;
	}
	
	private void Update()
	{
		if (playAgain)
		{
			playAgain = false;
			Play();
		}
	}
}
