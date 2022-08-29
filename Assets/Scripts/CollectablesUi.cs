using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class CollectablesUi : MonoBehaviour
{
    TMP_Text text;
    private CandyUI candy;
    [SerializeField] private ScriptableScore candysColected;
    
    private void Awake()
    {
        candy = gameObject.GetComponentInChildren<CandyUI>();
        text = gameObject.GetComponentInChildren<TMP_Text>();
        text.text = candysColected.score.ToString();
    }

    public void UpdateCandyCount()
    {
        candysColected.score++;
        //play candy here
        candy.EaseCandy();
        text.text = candysColected.score.ToString();
    }
}
