using System;
using UnityEngine;
using UnityEngine.UI;

public class Fade : EaseControllerBase
{
    [SerializeField] private Image image;

    [SerializeField] private EasingUtility.Style easingStyle;
    [SerializeField] public float transitionSpeed;
    private static readonly int Cut = Shader.PropertyToID("_Cut");
    [NonSerialized] public bool faded = false;
    
    private void Awake()
    {
        image.gameObject.SetActive(true);
    }

    public void FadeOut()
    {
        SetReverse(true);
        Play();
    }
    
    public void FadeIn()
    {
        SetReverse(false);
        Play();
    }

    private float origin;
    private float target;
    
    public override void OnStart()
    {
        faded = false;
        SetDuration(transitionSpeed);
        SetStyle(easingStyle);
        origin = -2f;
        image.material.SetFloat(Cut, origin);
        target = 1f;
    }

    public override void Evaluate(float t)
    {
        image.material.SetFloat(Cut, EasingUtility.Interpolate(origin, target, t)); 
    }

    public override void OnEnd()
    {
        faded = true;
    }
}
