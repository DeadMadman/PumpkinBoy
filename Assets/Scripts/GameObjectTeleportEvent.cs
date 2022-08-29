using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectTeleportEvent : MonoBehaviour
{
    [SerializeField] WhenToTrigger whenToTrigger;
    [SerializeField] TriggerType triggerType;
    [SerializeField] int timetoWait;
    [SerializeField] Transform teleportPosition;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(OnTeleportTriggerEnter(other));
    }

    private void OnTriggerExit(Collider other)
    {
        OnTeleportTriggerExit (other);
    }

    private IEnumerator OnTeleportTriggerEnter(Collider other)
    {
        if (whenToTrigger == WhenToTrigger.triggerOnEnter)
        {
            if (triggerType == TriggerType.Body && other.CompareTag("Body"))
            {
                if(timetoWait > 0)
                {
                    other.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    other.GetComponent<Rigidbody>().isKinematic = true;
                }
                yield return new WaitForSeconds(timetoWait);
                if (timetoWait > 0)
                {
                    other.GetComponent<Rigidbody>().isKinematic = false;
                }
                    other.transform.position = teleportPosition.transform.position;
            }
            else if (triggerType == TriggerType.Head && other.CompareTag("Head"))
            {
                if (timetoWait > 0)
                {
                    other.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    other.GetComponent<Rigidbody>().isKinematic = true;
                }
                yield return new WaitForSeconds(timetoWait);
                if (timetoWait > 0)
                {
                    other.GetComponent<Rigidbody>().isKinematic = false;
                }
                    other.transform.position = teleportPosition.transform.position;
            }
            else if (triggerType == TriggerType.HeadAndBody && (other.CompareTag("Head") || other.CompareTag("Body")))
            {
                if (timetoWait > 0)
                {
                    other.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    other.GetComponent<Rigidbody>().isKinematic = true;
                }
                yield return new WaitForSeconds(timetoWait);
                if (timetoWait > 0)
                {
                    other.GetComponent<Rigidbody>().isKinematic = false;
                }
                    other.transform.position = teleportPosition.transform.position;
            }
            else if (triggerType == TriggerType.TriggerObject && CheckIfITriggerObject(other))
            {
                if (timetoWait > 0)
                {
                    other.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    other.GetComponent<Rigidbody>().isKinematic = true;
                }
                yield return new WaitForSeconds(timetoWait);
                if (timetoWait > 0)
                {
                    other.GetComponent<Rigidbody>().isKinematic = false;
                }
                    other.transform.position = teleportPosition.transform.position;
            }
            else if (triggerType == TriggerType.AnyTrigger && (other.CompareTag("Head") || other.CompareTag("Body") || CheckIfITriggerObject(other)))
            {
                if (timetoWait > 0)
                {
                    other.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    other.GetComponent<Rigidbody>().isKinematic = true;
                }
                yield return new WaitForSeconds(timetoWait);
                if (timetoWait > 0)
                {
                    other.GetComponent<Rigidbody>().isKinematic = false;
                }
                    other.transform.position = teleportPosition.transform.position;
            }
        }
        yield break;
    }

    private void OnTeleportTriggerExit(Collider other)
    {
        if (whenToTrigger == WhenToTrigger.TriggerOnExit)
        {
            if (triggerType == TriggerType.Body && other.CompareTag("Body"))
            {
                other.transform.position = teleportPosition.transform.position;
            }
            else if (triggerType == TriggerType.Head && other.CompareTag("Head"))
            {
                other.transform.position = teleportPosition.transform.position;
            }
            else if (triggerType == TriggerType.HeadAndBody && (other.CompareTag("Head") || other.CompareTag("Body")))
            {
                other.transform.position = teleportPosition.transform.position;
            }
            else if (triggerType == TriggerType.TriggerObject && CheckIfITriggerObject(other))
            {
                other.transform.position = teleportPosition.transform.position;
            }
            else if (triggerType == TriggerType.AnyTrigger && (other.CompareTag("Head") || other.CompareTag("Body") || CheckIfITriggerObject(other)))
            {
                other.transform.position = teleportPosition.transform.position;
            }
        }
    }

    private void OnDrawGizmos()
    {
        
    }

    bool CheckIfITriggerObject(Collider other)
    {
        ITriggerObject iTriggerObject = other.gameObject.GetComponent<ITriggerObject>();

        if (iTriggerObject != null)
            return true;
        else
            return false;
    }
}
