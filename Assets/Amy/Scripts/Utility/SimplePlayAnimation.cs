using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayAnimation : MonoBehaviour
{

    public string animationToPlay;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator.Play(animationToPlay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
