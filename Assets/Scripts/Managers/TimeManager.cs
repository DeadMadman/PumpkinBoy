using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    /* This scrip is used to manage time in the game.
     * Call the methods in this script when changing time scale in other scripts.
     */ 

    public static void ContinueTime() // sets the time to normal speed
	{
        Time.timeScale = 1;
	}

    public static void PausTime() // sets the time to 0 to paus
    {
        Time.timeScale = 0;
    }

    public static void SlowDownTime(float slowAmount) // slows down time
    {
        Time.timeScale = slowAmount;
    }

    public static IEnumerator WaitForSeconds(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
    }
}
