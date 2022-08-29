using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerController : MonoBehaviour
{
    
    [HideInInspector] public bool isHoldingBox = false;
    //[SerializeField] Tutorializer tutorializerPrefab;
    [SerializeField] private Tutorializer tutorializer;
    [SerializeField] private bool startWithTutorial;

    [SerializeField] private bool disableBodyUntilHeadAttached;
    private bool headHasBeenAttachedOnce = false;
    [SerializeField] private HeadMovement headMovement;
    private HeadInteraction headInteraction;
    private BodyMovement bodyMovement;

    private SimpleControls controls;

    private void Awake()
    {
        controls = new SimpleControls();
        headInteraction = GetComponent<HeadInteraction>();
        bodyMovement = GetComponent<BodyMovement>();
    }

    private void Start()
    {
        bodyMovement.startedPushBoxEvent.AddListener(delegate { headInteraction.DropHead(); });
    }

    public void EnableActions()
    {
        controls.Gameplay.MoveBody.Enable();
        controls.Gameplay.MoveHead.Enable();
        controls.Gameplay.Throw.Enable();
        controls.Gameplay.PickUp.Enable();
    }

    public void DisableActions()
    {
        controls.Gameplay.MoveBody.Disable();
        controls.Gameplay.MoveHead.Disable();
        controls.Gameplay.Throw.Disable();
        controls.Gameplay.PickUp.Disable();
    }

    public void RegisterAllInputs()
    {
        headMovement.RegisterInputs(controls);

        //Debug.Log($"enabling inputs");

        if (!headHasBeenAttachedOnce)
        {
            if (disableBodyUntilHeadAttached)
                return;
        }

        headInteraction.RegisterInputs(controls);
        bodyMovement.RegisterMovement(controls);
        bodyMovement.RegisterHead(controls);
        bodyMovement.RegisterGrab(controls);

    }

    private void OnMoveBody(InputAction.CallbackContext context)
    {
        controls.Gameplay.MoveBody.performed -= OnMoveBody;
        controls.Gameplay.Throw.canceled += OnThrowHead;
        tutorializer.ShowThrowSprite();
        //tutorializer.gameObject.SetActive(false);
    }

    private void OnThrowHead(InputAction.CallbackContext context)
    {
        controls.Gameplay.Throw.canceled -= OnThrowHead;
        tutorializer.gameObject.SetActive(false);
    }

    public void DeregisterAllInputs()
    {
        //Debug.Log($"disabling inputs");
        headMovement.DeregisterInputs(controls);
        headInteraction.DeregisterInputs(controls);
        bodyMovement.DeregisterGrab(controls);
        bodyMovement.DeregisterMovement(controls);
        bodyMovement.DeregisterHead(controls);

    }

    public void StopAllMovement()
    {
        headMovement.StopMovement();
        bodyMovement.StopMovement();
    }

    public void RegisterHeadToBody()
    {
        if (disableBodyUntilHeadAttached && headHasBeenAttachedOnce == false)
        {
            headInteraction.RegisterInputs(controls);
            if (startWithTutorial)
            {
                tutorializer.ShowBodySprite();
                controls.Gameplay.MoveBody.performed += OnMoveBody;
            }

            bodyMovement.RegisterMovement(controls);
            bodyMovement.RegisterHead(controls);
            bodyMovement.RegisterGrab(controls);
        }
        headHasBeenAttachedOnce = true;
        //bodyMovement.RegisterHead(controls);
    }
    public void DeregisterHeadFromBody()
    {
        //bodyMovement.DeregisterHead(controls);
    }

    private void OnEnable()
    {
        EnableActions();
        RegisterAllInputs();
        InputUser.onChange += (user, enu, device) =>
        {
            //Debug.LogFormat("{0} - {1} - {2}", user, enu, device);
        };
    }

    private void OnDisable()
    {
        DisableActions();
        DeregisterAllInputs();
    }
}
