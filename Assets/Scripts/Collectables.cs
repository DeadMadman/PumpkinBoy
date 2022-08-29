using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class Collectables : MonoBehaviour
{
	[SerializeField] int id;
	[SerializeField] private MMFeedbacks pickupFeedbacks;
	CollectablesUi collectablesUi;
	MeshRenderer meshRenderer;
	bool collected;

	[SerializeField]Material colletedMaterial;

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		collectablesUi = FindObjectOfType<CollectablesUi>();
	}

    private void Start()
    {
        if (collected)
			meshRenderer.material = colletedMaterial;
    }

    private void OnTriggerEnter(Collider other)
	{
		if (!collected)
		{
			if (other.CompareTag("Body") || other.CompareTag("Head"))
			{
				collectablesUi.UpdateCandyCount();
				pickupFeedbacks?.PlayFeedbacks(transform.position);
				collected = true;
				Destroy(gameObject);
			}
		}
	}
}
