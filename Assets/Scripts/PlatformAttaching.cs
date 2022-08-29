using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAttaching : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Body") || other.CompareTag("Holdable item"))
        {
            other.transform.parent = transform.parent;
        }
        else if (other.CompareTag("Head"))
        {
            if (!HeadMovement.headAttached)
            {
                other.transform.parent = transform.parent;
                other.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;

            }
               
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Body") || other.CompareTag("Holdable item"))
        {
            other.transform.parent = null;
        }
        else if (other.CompareTag("Head"))
        {
            if (!HeadMovement.headAttached)
            {
                other.transform.parent = null;
                other.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                Debug.Log($"head exit");
            }
        }
    }
}
