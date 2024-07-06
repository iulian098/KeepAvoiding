using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    public int counter;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)){
            ScreenCapture.CaptureScreenshot("SS" + counter + ".png");
            counter++;
        }
    }
}
