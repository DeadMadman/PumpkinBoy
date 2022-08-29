using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class PressurePlateController : MonoBehaviour
{
    //Material
    private int id;
    [SerializeField] bool ReturnOnExit;
    [SerializeField] bool OriginalCollorOnExit;
    [SerializeField] bool BoxOnly;
    [SerializeField] Material disabledMaterial;
    [SerializeField] private MMFeedbacks exitFeedbacks;
    Material originalMaterial;
    EventTriggerArea eventTrigger;

    //Click
    TriggerType triggerType; // TODO ADD THIS
    [SerializeField] private MMFeedbacks activationFeedbacks;
    [SerializeField] Vector3 endPos = new Vector3(0, -0.079f, 0);
    Vector3 originalPos;
    bool pressed;
    Collider currentObject;

    private void Awake()
    {
        eventTrigger = GetComponent<EventTriggerArea>();
        originalMaterial = GetComponent<MeshRenderer>().material;
        id = eventTrigger.ReurnTriggerId();
        originalPos = transform.localPosition;
    }

    void Start()
    {
        GameEventManager.current.onTriggerEnter += OnMaterialTriggerEnter; // adds the event to the gameEvents
        GameEventManager.current.onTriggerExit += OnMaterialTriggerExit; // remove the event to the gameEvents

    }

    private void OnDestroy()
    {
        GameEventManager.current.onTriggerEnter -= OnMaterialTriggerEnter; // remove the event to the gameEvents
        GameEventManager.current.onTriggerExit -= OnMaterialTriggerExit; // remove the event to the gameEvents
    }

    void OnMaterialTriggerEnter(int id)
    {
        if (id == this.id)
            gameObject.GetComponent<MeshRenderer>().material = disabledMaterial;
    }

    void OnMaterialTriggerExit(int id)
    {
        if (OriginalCollorOnExit)
            if (id == this.id)
                gameObject.GetComponent<MeshRenderer>().material = originalMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (BoxOnly && !other.CompareTag("Holdable item"))
            return;

        if (currentObject == null)
        {
            currentObject = other;
            transform.localPosition = endPos;
            if (!pressed)
                activationFeedbacks?.PlayFeedbacks();
            pressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (BoxOnly && !other.CompareTag("Holdable item"))
            return;

        if (other == currentObject && ReturnOnExit)
        {
            transform.localPosition = originalPos;
            pressed = false;
            currentObject = null;
            // if statment to not trigger if some thing is standing on the plate
            exitFeedbacks?.PlayFeedbacks(transform.position);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (BoxOnly && !other.CompareTag("Holdable item"))
            return;

        if (currentObject == null)
        {
            currentObject = other;
            transform.localPosition = endPos;
            pressed = true;
        }
    }

}

