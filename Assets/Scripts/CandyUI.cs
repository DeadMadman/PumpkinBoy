using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandyUI : EaseControllerBase
{
    [SerializeField] private EasingUtility.Style easingStyle;
    [SerializeField] private float easeTimer;
    private RawImage image;
    private Vector2 startScale;
    private Vector2 endScale;
    
    private void Awake()
    {
        image = GetComponent<RawImage>();
    }
    public void EaseCandy()
    {
        Play();
    }
    public override void OnStart()
    {
        SetStyle(easingStyle);
        SetDuration(easeTimer);
        startScale = image.rectTransform.sizeDelta;
        endScale = startScale + image.rectTransform.sizeDelta;
    }
    public override void Evaluate(float t)
    {
        image.rectTransform.sizeDelta = EasingUtility.Interpolate(startScale, endScale, t);
    }
    public override void OnEnd()
    {
        
    }
}
