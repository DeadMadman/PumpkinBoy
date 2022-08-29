using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Deathzone : MonoBehaviour
{
    [SerializeField] private float timeToDie;
    private Fade fade;

    private void Awake()
    {
        fade = GetComponent<Fade>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Holdable item"))
        {
            other.GetComponent<PushableObjects>().ResetBoxToOriginalPosition();
            return;
        }
        StartCoroutine(RestartGame());
    }
    
    private IEnumerator RestartGame()
    {
        //play sound and have image
        
        fade.FadeOut();
        yield return new WaitForSeconds(timeToDie);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
