using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

enum WhenToTrigger {triggerOnEnter, TriggerOnExit, Timer }

public class CinemachineCameraPrio : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] WhenToTrigger whenToTrigger;
    [SerializeField] int PriorityValue = 20;
    [SerializeField] float Timer = 3;
    [SerializeField] bool lockPlayerMovement;
    CinemachineVirtualCamera VirtualCam;
    PlayerController playerController;

    private void Awake()
    {
        VirtualCam = GetComponent<CinemachineVirtualCamera>();
        playerController = FindObjectOfType<PlayerController>();
    }

    void Start()
    {
        GameEventManager.current.onTriggerEnter += OnEnterCameraPan; // adds the event to the gameEvents
        GameEventManager.current.onTriggerExit += OnExitCameraPan; // adds the event to the gameEvents
        if (whenToTrigger == WhenToTrigger.Timer)
        {
            GameEventManager.current.onTriggerExit += TimerCameraPan; // adds the event to the gameEvents
            StartCoroutine(PriorityTimer());
        }
    }

    private void OnDestroy()
    {
        GameEventManager.current.onTriggerEnter -= OnEnterCameraPan; // remove the event to the gameEvents
        GameEventManager.current.onTriggerExit -= OnExitCameraPan; // remove the event to the gameEvents
        if (whenToTrigger == WhenToTrigger.Timer)
            GameEventManager.current.onTriggerExit -= TimerCameraPan; 
}

    private void OnEnterCameraPan(int id)
    {
        if (id == this.id && whenToTrigger == WhenToTrigger.triggerOnEnter)
        {
            StartCoroutine(PriorityTimer());
        }
    }

    private void OnExitCameraPan(int id)
    {
        if (id == this.id && whenToTrigger == WhenToTrigger.TriggerOnExit)
        {
            StartCoroutine(PriorityTimer());
        }
    }

    private void TimerCameraPan (int id)
    {
        if (id == this.id)
        {
            StartCoroutine(PriorityTimer());
        }
    }

    IEnumerator PriorityTimer()
    {
        if (lockPlayerMovement)
        {
            playerController.DeregisterAllInputs();
            playerController.StopAllMovement();
        }
            
        VirtualCam.Priority = PriorityValue;
        yield return new WaitForSeconds(Timer);
        VirtualCam.Priority = 0;
        if (lockPlayerMovement)
        {
            playerController.RegisterAllInputs();
        }
            
    }

}
