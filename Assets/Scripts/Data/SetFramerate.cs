using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFramerate : MonoBehaviour
{
    void Awake(){
        Application.targetFrameRate = 60;
    }
}
