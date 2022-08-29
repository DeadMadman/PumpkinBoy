using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum WhenToDisable { EnableOnEnter, EnableOnExit, DisableOnEnter, DisableOnExit, EnableOnEnterDisableOnExit, DisableOnEnterEnableOnExit };

public class DisableEnableGameObjectEvent : MonoBehaviour
{
    [SerializeField] private int id;
    [Tooltip ("Use an empty game object as a parent to the game object that should be disabled")]
    [SerializeField] GameObject[] whatToDisable;
    [SerializeField] WhenToDisable whenToDisable;

    void Start()
    {
        GameEventManager.current.onTriggerEnter += OnDisableTriggerEnter; // adds the event to the gameEvents
        GameEventManager.current.onTriggerExit += OnDisableTriggerExit; // adds the event to the gameEvents
    }

    private void OnDestroy()
    {
        GameEventManager.current.onTriggerEnter -= OnDisableTriggerEnter; // remove the event to the gameEvents
        GameEventManager.current.onTriggerExit -= OnDisableTriggerExit; // remove the event to the gameEvents
    }

    private void OnDisableTriggerEnter(int id) // moves the door with the same id as the trigger x distance
    {
        if (id == this.id && whenToDisable == WhenToDisable.DisableOnEnter)
        {
            foreach (GameObject gameObject in whatToDisable)
            {
                gameObject.SetActive(false);    
            }
        }
        else if (id == this.id && whenToDisable == WhenToDisable.EnableOnEnter)
        {
            foreach (GameObject gameObject in whatToDisable)
            {
                gameObject.SetActive(true);
            }
        }
        else if (id == this.id && whenToDisable == WhenToDisable.EnableOnEnterDisableOnExit)
        {
            foreach (GameObject gameObject in whatToDisable)
            {
                gameObject.SetActive(true);
            }
        }
        else if (id == this.id && whenToDisable == WhenToDisable.DisableOnEnterEnableOnExit)
        {
            foreach (GameObject gameObject in whatToDisable)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnDisableTriggerExit(int id) // moves the door with the same id as the trigger x distance
    {
        if (id == this.id && whenToDisable == WhenToDisable.DisableOnExit)
        {
            foreach (GameObject gameObject in whatToDisable)
            {
                gameObject.SetActive(false);
            }
        }
        else if (id == this.id && whenToDisable == WhenToDisable.EnableOnExit)
        {
            foreach (GameObject gameObject in whatToDisable)
            {
                gameObject.SetActive(true);
            }
        }
        else if (id == this.id && whenToDisable == WhenToDisable.EnableOnEnterDisableOnExit)
        {
            foreach (GameObject gameObject in whatToDisable)
            {
                gameObject.SetActive(false);
            }
        }
        else if (id == this.id && whenToDisable == WhenToDisable.DisableOnEnterEnableOnExit)
        {
            foreach (GameObject gameObject in whatToDisable)
            {
                gameObject.SetActive(true);
            }
        }
    }
}
