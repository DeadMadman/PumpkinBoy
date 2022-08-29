using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager current;

    public event Action<int> onTriggerEnter;
    public event Action<int> onTriggerExit;

    private void Awake()
    {
        current = this;
    }
    //Triggers
    public void OnEventTriggerEnter(int id) // preformce the game event a trigger is set to do when the corect component enters
    {
        if (onTriggerEnter != null)
        {
            onTriggerEnter(id);
        }
    }

    public void OnEventTriggerExit(int id) // preformce the game event a trigger is set to do when the corect component exits
    {
        if (onTriggerExit != null)
        {
            onTriggerExit(id);
        }
    }
}
