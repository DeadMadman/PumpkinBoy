using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EaseControllerBase : MonoBehaviour
{
	public enum TimeMode
    {
        Update,
        UnscaledUpdate,
        FixedUpdate,
        UnscaledFixedUpdate,
        WaitForSeconds,
        UnscaledWaitForSeconds
	}

    private bool isReverse = false;
    public bool IsReverse => isReverse;
    
    private float duration = 1f;
    private TimeMode timeMode = TimeMode.Update;
    private float deltaTime = 0.01f;
    private EasingUtility.Style easeStyle = EasingUtility.Style.Linear;

    public EaseControllerBase SetDuration(float f)
    {
	    duration = f;
	    return this;
    }
    
    public EaseControllerBase SetTimeMode(TimeMode tm)
    {
	    timeMode = tm;
	    return this;
    }
    
    //only required if using time mode (unscaled) wait for seconds
    public EaseControllerBase SetDeltaTime(float dt)
    {
	    deltaTime = dt;
	    return this;
    }
    
    public EaseControllerBase SetStyle(EasingUtility.Style s)
    {
	    easeStyle = s;
	    return this;
    }
    
    private bool isPlaying = false;
    public bool IsPlaying => isPlaying;
    private bool isRunning = false;
    private bool isHardstop = false;
    
    public abstract void OnStart(); // Assume 0.0f
    public abstract void Evaluate(float t); // Assume [0.0f; 1.0f[
    public abstract void OnEnd(); // Assume 1.0f
    
    public void Play()
    {
        isPlaying = true;
        StartCoroutine(Run());
	}
    
    public void Pause()
    {
        isPlaying = false;
    }

	public void Stop(bool hardStop = false)
	{
        Pause();
        StopCoroutine(Run());
		isRunning = false;
		isHardstop = hardStop;
	}
	
	public void SetReverse(bool to)
	{
		isReverse = to;
	}

	private IEnumerator Run()
    {
        if(isRunning) {
            yield break;
		}
        isRunning = true;

        OnStart();

        bool isStillRunning(float t)
        {
	        if (isReverse)
	        {
		        return t >0.0f;
	        }
	        return t < 1.0f;
        }

        float t = isReverse ? 1.0f : 0.0f;
        while(isStillRunning(t) && isRunning)
        {
	        Evaluate(EasingUtility.GetFunction(easeStyle)(t));
	        
	        float change = (GetDeltaTime(timeMode) / duration);
	        t += isReverse ? -change : change;
	        
	        yield return GetWaitingTime(timeMode);
		}
        Evaluate(EasingUtility.GetFunction(easeStyle)(isReverse ? 0.0f : 1.0f));

        isRunning = false;
        OnEnd();
    }

    private IEnumerator GetWaitingTime(TimeMode timeMode)
    {
        if(!isPlaying) {
            yield return new WaitUntil(() => isPlaying);
		}

		switch (timeMode) {
			case TimeMode.Update:
				yield return null;
				break;
			case TimeMode.UnscaledUpdate:
				yield return null;
				break;
			case TimeMode.FixedUpdate:
				yield return new WaitForFixedUpdate();
				break;
			case TimeMode.UnscaledFixedUpdate:
				yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
				break;
			case TimeMode.WaitForSeconds:
				yield return new WaitForSeconds(deltaTime);
                break;
			case TimeMode.UnscaledWaitForSeconds:
                yield return new WaitForSecondsRealtime(deltaTime);
                break;
            default:
				throw new UnityException("Unknown TimeMode");
		}
	}

	private float GetDeltaTime(TimeMode timeMode)
    {
		switch (timeMode) {
			case TimeMode.Update:
				return Time.deltaTime;
			case TimeMode.UnscaledUpdate:
				return Time.unscaledDeltaTime;
			case TimeMode.FixedUpdate:
				return Time.fixedDeltaTime;
			case TimeMode.UnscaledFixedUpdate:
				return Time.fixedUnscaledDeltaTime;
			case TimeMode.WaitForSeconds:
                return deltaTime;
			case TimeMode.UnscaledWaitForSeconds:
                return deltaTime;
            default:
				throw new UnityException("Unknown TimeMode");
		}
	}
}
