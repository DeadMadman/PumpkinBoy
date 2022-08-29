using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

enum TriggerType { Body, Head, HeadAndBody, TriggerObject, AnyTrigger };
enum TimerType { noTimer, ExitEventTimer };

public class EventTriggerArea : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] TriggerType triggerType;
    [SerializeField] bool tiggerOnEnterEvents;
    [SerializeField] bool tiggerOnExitEvents;
    [SerializeField] bool useTriggerDelayOnEnter;
    [SerializeField] private float timeToStayInsideTrigger = 2f;
    [SerializeField] TimerType timerType;
    [SerializeField] float exitEventTimer = 2;
    public List<GameObject> connectedTriggers;
    [HideInInspector] public bool ConnectionisTriggerd;
    [HideInInspector] public bool triggerd = false;
    private float stayInTriggerTimer = 0;
    [SerializeField] TriggerIndicator triggerIndicator;

    [SerializeField]
    List<GameObject> gameObjects;

    bool indicatorActivated;
    bool prev = false;
    bool current = false;

    // Added preprocessor macros to not cause harm to your build 
#if UNITY_EDITOR
    private float currentTime = 0.0f;
    private void Update()
    {
        currentTime = Time.time;
    }
#endif


    private void OnTriggerEnter(Collider other)
    {
        if (triggerd)
          return;

        AddTriggerObjectToList(other, true);

        if (!useTriggerDelayOnEnter)
            AddIncicator(other);

        if (tiggerOnEnterEvents && !useTriggerDelayOnEnter && connectedTriggers.Count == 0)
            OnEnter(other);
        else //(if UseTriggerDelayOnEnter) Sets a timer for how long a gameobject needs to be in a trigger to trigger it.
            stayInTriggerTimer = Time.time + timeToStayInsideTrigger;
    }

    void UpdateTriggerStay(Collider other)
    {
        if (stayInTriggerTimer <= Time.time && useTriggerDelayOnEnter)
        {
            ConnectionisTriggerd = true;
            if (!indicatorActivated)
                AddIncicator(other);
            indicatorActivated = true;
        }
        else if (!useTriggerDelayOnEnter)
            ConnectionisTriggerd = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (tiggerOnEnterEvents)
        {
            if (triggerType == TriggerType.Body && other.CompareTag("Body"))
                UpdateTriggerStay(other);
            else if (triggerType == TriggerType.Head && other.CompareTag("Head"))
                UpdateTriggerStay(other);
            else if (triggerType == TriggerType.HeadAndBody && (other.CompareTag("Head") || other.CompareTag("Body")))
                UpdateTriggerStay(other);
            else if (triggerType == TriggerType.TriggerObject && CheckIfITriggerObject(other))
                UpdateTriggerStay(other);
            else if (triggerType == TriggerType.AnyTrigger && (other.CompareTag("Head") || other.CompareTag("Body") || CheckIfITriggerObject(other)))
                UpdateTriggerStay(other);
            else
                return;
        }

        if (useTriggerDelayOnEnter) //objects
        {
            //A object is the trigger activate afther X seconds
            if (stayInTriggerTimer <= Time.time)
            {
                OnEnter(other);
            }
        }
        else if (!useTriggerDelayOnEnter && connectedTriggers.Count !=0)
        {
            OnEnter(other);
        }
    }

    void ConnectedTriggerOnExit(Collider other)
    {
        if (triggerType == TriggerType.Body && other.CompareTag("Body"))
            ConnectionisTriggerd = false;
        else if (triggerType == TriggerType.Head && other.CompareTag("Head"))
            ConnectionisTriggerd = false;
        else if (triggerType == TriggerType.HeadAndBody && (other.CompareTag("Head") || other.CompareTag("Body")))
            ConnectionisTriggerd = false;
        else if (triggerType == TriggerType.TriggerObject && CheckIfITriggerObject(other))
            ConnectionisTriggerd = false;
        else if (triggerType == TriggerType.AnyTrigger && (other.CompareTag("Head") || other.CompareTag("Body") || CheckIfITriggerObject(other)))
            ConnectionisTriggerd = false;
    }

    void UpdateOnTriggerExit(Collider collider)
    {
        if (stayInTriggerTimer <= Time.time && useTriggerDelayOnEnter)
        {
            OnExit(collider);
            if (connectedTriggers.Count != 0)
                ConnectedTriggerOnExit(collider);
        }
        else if (!useTriggerDelayOnEnter)
        {
            OnExit(collider);
            if (connectedTriggers.Count != 0)
                ConnectedTriggerOnExit(collider);
        }
    }

    private IEnumerator OnTriggerExit(Collider other) // Enumerator so that on trigger exit can be delayd
    {
        gameObjects.Remove(other.gameObject);

        if (gameObjects.Count < 1)
        {
            triggerd = false;
            indicatorActivated = false;
            if (!useTriggerDelayOnEnter)
                RemoveIndicator(other);
            else if (useTriggerDelayOnEnter && stayInTriggerTimer <= Time.time && useTriggerDelayOnEnter)
                RemoveIndicator(other);

            if (tiggerOnExitEvents)
            {
                if (timerType == TimerType.ExitEventTimer)
                {
                    if (triggerType == TriggerType.Body && other.CompareTag("Body"))
                    {
                        yield return new WaitForSeconds(exitEventTimer);
                        UpdateOnTriggerExit(other);
                    }
                    else if (triggerType == TriggerType.Head && other.CompareTag("Head"))
                    {
                        yield return new WaitForSeconds(exitEventTimer);
                        UpdateOnTriggerExit(other);
                    }
                    else if (triggerType == TriggerType.HeadAndBody && (other.CompareTag("Head") || other.CompareTag("Body")))
                    {
                        yield return new WaitForSeconds(exitEventTimer);
                        UpdateOnTriggerExit(other);
                    }
                    else if (triggerType == TriggerType.TriggerObject && CheckIfITriggerObject(other))
                    {
                        yield return new WaitForSeconds(exitEventTimer);
                        UpdateOnTriggerExit(other);
                    }
                    else if (triggerType == TriggerType.AnyTrigger && (other.CompareTag("Head") || other.CompareTag("Body") || CheckIfITriggerObject(other)))
                    {
                        yield return new WaitForSeconds(exitEventTimer);
                        UpdateOnTriggerExit(other);
                    }
                }
                else
                {
                    if (triggerType == TriggerType.Body && other.CompareTag("Body"))
                        UpdateOnTriggerExit(other);
                    else if (triggerType == TriggerType.Head && other.CompareTag("Head"))
                        UpdateOnTriggerExit(other);
                    else if (triggerType == TriggerType.HeadAndBody && (other.CompareTag("Head") || other.CompareTag("Body")))
                        UpdateOnTriggerExit(other);
                    else if (triggerType == TriggerType.TriggerObject && CheckIfITriggerObject(other))
                        UpdateOnTriggerExit(other);
                    else if (triggerType == TriggerType.AnyTrigger && (other.CompareTag("Head") || other.CompareTag("Body") || CheckIfITriggerObject(other)))
                        UpdateOnTriggerExit(other);
                }
            }
        }
    }

    void OnEnter(Collider other)
    {
        prev = current;
        current = CheckConnectedTriggers();
        if (!prev && current && connectedTriggers.Count != 0)
        {
            CheckIfITriggerObject(other);
            if (triggerType == TriggerType.Body && other.CompareTag("Body"))
                GameEventManager.current.OnEventTriggerEnter(id); // triggers the event conected to the trigger with the same id
            else if (triggerType == TriggerType.Head && other.CompareTag("Head"))
                GameEventManager.current.OnEventTriggerEnter(id); // triggers the event conected to the trigger with the same id
            else if (triggerType == TriggerType.HeadAndBody && (other.CompareTag("Head") || other.CompareTag("Body")))
                GameEventManager.current.OnEventTriggerEnter(id); // triggers the event conected to the trigger with the same id
            else if (triggerType == TriggerType.TriggerObject && CheckIfITriggerObject(other))
                GameEventManager.current.OnEventTriggerEnter(id); // triggers the event conected to the trigger with the same id
            else if (triggerType == TriggerType.AnyTrigger && (other.CompareTag("Head") || other.CompareTag("Body") || CheckIfITriggerObject(other)))
                GameEventManager.current.OnEventTriggerEnter(id); // triggers the event conected to the trigger with the same id
        }
        else if (connectedTriggers.Count == 0 && !prev)
        {
            CheckIfITriggerObject(other);
            if (triggerType == TriggerType.Body && other.CompareTag("Body"))
                GameEventManager.current.OnEventTriggerEnter(id); // triggers the event conected to the trigger with the same id
            else if (triggerType == TriggerType.Head && other.CompareTag("Head"))
                GameEventManager.current.OnEventTriggerEnter(id); // triggers the event conected to the trigger with the same id
            else if (triggerType == TriggerType.HeadAndBody && (other.CompareTag("Head") || other.CompareTag("Body")))
                GameEventManager.current.OnEventTriggerEnter(id); // triggers the event conected to the trigger with the same id
            else if (triggerType == TriggerType.TriggerObject && CheckIfITriggerObject(other))
                GameEventManager.current.OnEventTriggerEnter(id); // triggers the event conected to the trigger with the same id
            else if (triggerType == TriggerType.AnyTrigger && (other.CompareTag("Head") || other.CompareTag("Body") || CheckIfITriggerObject(other)))
                GameEventManager.current.OnEventTriggerEnter(id); // triggers the event conected to the trigger with the same id
        }
    }

    private void AddTriggerObjectToList(Collider other, bool triggerd)
    {
        if (triggerType == TriggerType.Body && other.CompareTag("Body"))
        {
            triggerd = this.triggerd;
            gameObjects.Add(other.gameObject);
        }
        else if (triggerType == TriggerType.Head && other.CompareTag("Head"))
        {
            triggerd = this.triggerd;
            gameObjects.Add(other.gameObject);
        }
        else if (triggerType == TriggerType.HeadAndBody && (other.CompareTag("Head") || other.CompareTag("Body")))
        {
            triggerd = this.triggerd;
            gameObjects.Add(other.gameObject);
        }
        else if (triggerType == TriggerType.TriggerObject && CheckIfITriggerObject(other))
        {
            triggerd = this.triggerd;
            gameObjects.Add(other.gameObject);
        }
        else if (triggerType == TriggerType.AnyTrigger && (other.CompareTag("Head") || other.CompareTag("Body") || CheckIfITriggerObject(other)))
        {
            triggerd = this.triggerd;
            gameObjects.Add(other.gameObject);
        }
    }
    void OnExit(Collider other)
    {
        if (gameObjects.Count < 1)
        {
            prev = current;
            current = !CheckConnectedTriggers();
            if (prev && !current && connectedTriggers.Count != 0)
            {
                if (triggerType == TriggerType.Body && other.CompareTag("Body"))
                {
                    GameEventManager.current.OnEventTriggerExit(id); // triggers the event conected to the trigger with the same id
                    ConnectionisTriggerd = false;
                }
                else if (triggerType == TriggerType.Head && other.CompareTag("Head"))
                {
                    GameEventManager.current.OnEventTriggerExit(id); // triggers the event conected to the trigger with the same id
                    ConnectionisTriggerd = false;
                }
                else if (triggerType == TriggerType.HeadAndBody && (other.CompareTag("Head") || other.CompareTag("Body")))
                {
                    GameEventManager.current.OnEventTriggerExit(id); // triggers the event conected to the trigger with the same id
                    ConnectionisTriggerd = false;
                }
                else if (triggerType == TriggerType.TriggerObject && CheckIfITriggerObject(other))
                {
                    GameEventManager.current.OnEventTriggerExit(id); // triggers the event conected to the trigger with the same id
                    ConnectionisTriggerd = false;
                }
                else if (triggerType == TriggerType.AnyTrigger && (other.CompareTag("Head") || other.CompareTag("Body") || CheckIfITriggerObject(other)))
                {
                    GameEventManager.current.OnEventTriggerExit(id); // triggers the event conected to the trigger with the same id
                    ConnectionisTriggerd = false;
                }
            }
            else if (connectedTriggers.Count == 0)
            {
                if (triggerType == TriggerType.Body && other.CompareTag("Body"))
                    GameEventManager.current.OnEventTriggerExit(id); // triggers the event conected to the trigger with the same id
                else if (triggerType == TriggerType.Head && other.CompareTag("Head"))
                    GameEventManager.current.OnEventTriggerExit(id); // triggers the event conected to the trigger with the same id
                else if (triggerType == TriggerType.HeadAndBody && (other.CompareTag("Head") || other.CompareTag("Body")))
                    GameEventManager.current.OnEventTriggerExit(id); // triggers the event conected to the trigger with the same id
                else if (triggerType == TriggerType.TriggerObject && CheckIfITriggerObject(other))
                    GameEventManager.current.OnEventTriggerExit(id); // triggers the event conected to the trigger with the same id
                else if (triggerType == TriggerType.AnyTrigger && (other.CompareTag("Head") || other.CompareTag("Body") || CheckIfITriggerObject(other)))
                    GameEventManager.current.OnEventTriggerExit(id); // triggers the event conected to the trigger with the same id
            }
        }
    }

    /// <MultipleTriggers Checks if all Triggers in a multiple trigger setup are active>
    /// <returns False when All connected triggers are not triggerd><returns True When all connected triggers are active>
    bool CheckConnectedTriggers()
    {
        foreach (GameObject trigger in connectedTriggers)
        {
            if (!trigger.GetComponent<EventTriggerArea>().ConnectionisTriggerd)
                return false;
        }
        return true;
    }

    bool CheckIfITriggerObject(Collider other)
    {
        ITriggerObject iTriggerObject = other.gameObject.GetComponent<ITriggerObject>();

        if (iTriggerObject != null)
            return true;
        else
            return false;
    }



    private void RemoveIndicator(Collider other)
    {
        if (triggerIndicator != null)
        {
            if (triggerType == TriggerType.Body && other.CompareTag("Body"))
            { 
                triggerIndicator.removeTrigger();
                triggerIndicator.UpdateIndicator(false, triggerIndicator.connectedTriggersTriggerd, this);
            }
            else if (triggerType == TriggerType.Head && other.CompareTag("Head"))
            {
                triggerIndicator.removeTrigger();
                triggerIndicator.UpdateIndicator(false, triggerIndicator.connectedTriggersTriggerd, this);
            }
            else if (triggerType == TriggerType.HeadAndBody && (other.CompareTag("Head") || other.CompareTag("Body")))
            {
                triggerIndicator.removeTrigger();
                triggerIndicator.UpdateIndicator(false, triggerIndicator.connectedTriggersTriggerd, this);
            }
            else if (triggerType == TriggerType.TriggerObject && CheckIfITriggerObject(other))
            {
                triggerIndicator.removeTrigger();
                triggerIndicator.UpdateIndicator(false, triggerIndicator.connectedTriggersTriggerd, this);
            }
            else if (triggerType == TriggerType.AnyTrigger && (other.CompareTag("Head") || other.CompareTag("Body") || CheckIfITriggerObject(other)))
            {
                triggerIndicator.removeTrigger();
                triggerIndicator.UpdateIndicator(false, triggerIndicator.connectedTriggersTriggerd, this);
            }
        }    
    }

    private void AddIncicator(Collider other)
    {
        if (triggerIndicator != null)
        {
            if (triggerType == TriggerType.Body && other.CompareTag("Body"))
            { 
                triggerIndicator.addTrigger();
                triggerIndicator.UpdateIndicator(true, triggerIndicator.connectedTriggersTriggerd, this);
            }
            else if (triggerType == TriggerType.Head && other.CompareTag("Head"))
            {
                triggerIndicator.addTrigger();
                triggerIndicator.UpdateIndicator(true, triggerIndicator.connectedTriggersTriggerd, this);
            }
            else if (triggerType == TriggerType.HeadAndBody && (other.CompareTag("Head") || other.CompareTag("Body")))
            {
                triggerIndicator.addTrigger();
                triggerIndicator.UpdateIndicator(true, triggerIndicator.connectedTriggersTriggerd, this);
            }
            else if (triggerType == TriggerType.TriggerObject && CheckIfITriggerObject(other))
            {
                triggerIndicator.addTrigger();
                triggerIndicator.UpdateIndicator(true, triggerIndicator.connectedTriggersTriggerd, this);
            }
            else if (triggerType == TriggerType.AnyTrigger && (other.CompareTag("Head") || other.CompareTag("Body") || CheckIfITriggerObject(other)))
            {
                triggerIndicator.addTrigger();
                triggerIndicator.UpdateIndicator(true, triggerIndicator.connectedTriggersTriggerd, this);
            }
        }
    }

    public int ReurnTriggerId()
    {
        return id;
    }
}
