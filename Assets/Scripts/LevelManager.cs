using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] WhenToTrigger whenToTrigger;
    int nextSceneIndex;

    bool bodyInGoal;
    bool headInGoal;

    private Fade fade;

    private void Awake()
    {
        fade = GetComponent<Fade>();
        StartCoroutine(WaitAndPlay());
        nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
    }

    private void Start()
    {
        fade.FadeIn();
    }

    void OnTriggerEnter(Collider other)
    {
        if (whenToTrigger == WhenToTrigger.triggerOnEnter && other.CompareTag("Body"))
            bodyInGoal = true;
        else if (whenToTrigger == WhenToTrigger.triggerOnEnter && other.CompareTag("Head"))
            headInGoal = true;

        if (bodyInGoal && headInGoal || (bodyInGoal && HeadMovement.headAttached))
        {
            if (SceneManager.sceneCountInBuildSettings > nextSceneIndex) // loads next level if there is one
            {
                fade.FadeOut();
                StartCoroutine(WaitAndLoad());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (whenToTrigger == WhenToTrigger.triggerOnEnter && other.CompareTag("Body"))
            bodyInGoal = false;
        else if (whenToTrigger == WhenToTrigger.triggerOnEnter && other.CompareTag("Head"))
            headInGoal = false;

        //if (bodyInGoal && headInGoal)
        //{
        //    if (SceneManager.sceneCountInBuildSettings > nextSceneIndex) // loads next level if there is one
        //    {
        //        SceneManager.LoadScene(nextSceneIndex);
        //    }
        //}
    }

    private IEnumerator WaitAndLoad()
    {
        yield return new WaitUntil(() => fade.faded);
        SceneManager.LoadScene(nextSceneIndex);
    }
    
    private IEnumerator WaitAndPlay()
    {
        yield return new WaitUntil(() => fade.faded);
    }
}
