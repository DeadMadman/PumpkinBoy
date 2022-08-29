using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    [SerializeField] private int NextSceneIndex;
    [SerializeField] private GameObject credits; 
    private Fade fade;
    
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        StartCoroutine(WaitAndLoad());
    }

    private IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds((float) videoPlayer.clip.length);
        if (credits != null)
        {
            credits.SetActive(true);
            fade = credits.GetComponentInChildren<Fade>();
            fade.FadeIn();
            yield return new WaitForSeconds((float) videoPlayer.clip.length);
        }
        SceneManager.LoadScene(NextSceneIndex);
    }

    private IEnumerator Load()
    {
        if (fade != null)
        {
            fade.FadeOut();
            yield return new WaitForSeconds(fade.transitionSpeed);
            SceneManager.LoadScene(NextSceneIndex);
        }
    }
    
    private void Update()
    {
        if (Gamepad.current != null)
        {
            if (Gamepad.current.selectButton.isPressed)
            {
                StartCoroutine(Load());
            }
        }
        else if(Keyboard.current != null)
        {
            if (Keyboard.current.escapeKey.isPressed)
            {
                StartCoroutine(Load());
            }
        }
    }
}
