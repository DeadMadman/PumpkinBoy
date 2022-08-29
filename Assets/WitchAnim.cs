using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchAnim : MonoBehaviour
{
    [SerializeField] private float time = 30;
    private Animation anim;
    private AudioSource audio;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        audio = GetComponent<AudioSource>();
        StartCoroutine(LoopAnim());
    }

    private IEnumerator LoopAnim()
    {
        yield return new WaitForSeconds(time + Random.Range(0, time / 2));
        anim.Play();
        audio.Play();
        StartCoroutine(LoopAnim());
    }

}
