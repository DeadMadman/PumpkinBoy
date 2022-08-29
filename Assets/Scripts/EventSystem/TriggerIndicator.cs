using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TriggerIndicator : MonoBehaviour
{
    [SerializeField] Material triggerdMaterial;
    [SerializeField] WhenToTrigger whenToTrigger;
    [SerializeField] bool stayActiveOnExit;
    [SerializeField] GameObject[] indicators;
    Material originalMaterial;
    MeshRenderer[] mesh;
    [HideInInspector] public int connectedTriggersTriggerd;
    int j;
    bool allIndicatorsActive = false;

    private void Awake()
    {
        mesh = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < indicators.Length; i++)
            originalMaterial = mesh[i].material;
    }

    int howManyActive;

    void CheckIfActive(int connectedTriggersCount)
    {
        if (indicators.Length > 1)
        {
            for (int i = 0; i < indicators.Length; i++)
            {
                if (indicators[i].GetComponent<MeshRenderer>().material != originalMaterial)
                {
                    howManyActive++;
                    Debug.Log(howManyActive);
                }
            }
            if (howManyActive == connectedTriggersCount)
                allIndicatorsActive = true;
            else
            {
                howManyActive = 0;
                allIndicatorsActive = false;
            }
        }
        else
            allIndicatorsActive = true;
    }

    public void UpdateIndicator(bool add, int triggerIndex, EventTriggerArea eventTrigger)
    {
        if (allIndicatorsActive)
            return;


        if (add && !allIndicatorsActive)
        {
            if (eventTrigger.connectedTriggers.Count !=0)
            {
                for (int i = 0; i < connectedTriggersTriggerd; i++)
                    mesh[i].material = triggerdMaterial;
            }
            else
                mesh[j].material = triggerdMaterial;

            if (j > 0)
                j--;
        }
        else if (!add && !allIndicatorsActive)
        {
            foreach (GameObject trigger in eventTrigger.connectedTriggers)
            {
                if (!trigger.GetComponent<EventTriggerArea>().triggerd)
                {

                    mesh[j].material = originalMaterial;
                    if (j < connectedTriggersTriggerd)
                        j++;
                }
            }
            if (eventTrigger.connectedTriggers.Count == 0)
            {
                mesh[j].material = originalMaterial;
            }
        }

        if (stayActiveOnExit)
        CheckIfActive(eventTrigger.connectedTriggers.Count);
    }
   
    public void addTrigger()
    {
        connectedTriggersTriggerd++;
    }

    public void removeTrigger()
    {
        connectedTriggersTriggerd--;
    }
}


    
