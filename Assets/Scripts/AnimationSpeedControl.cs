using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSpeedControl : MonoBehaviour
{

    public float speed;

    Animator anim;
    
    void Start()
    {
        if(!anim)
            anim = this.GetComponent<Animator>();

        anim.speed = speed;
    }
}
