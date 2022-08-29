using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class MenuClick : MonoBehaviour
{
    [SerializeField] private MMFeedbacks activationFeedbacks;
    private Animator anim;

    private bool once = true;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetBool("Selected") && once)
        {
            activationFeedbacks?.PlayFeedbacks();
            once = false;
        }
        else if (!anim.GetBool("Selected") && !once)
        {
            once = true;
        }
    }
}
