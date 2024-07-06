using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    void Awake(){
        instance = this;
    }

    public IEnumerator Shake(float duration, float magnitude){
        Vector3 originalPos = Vector3.zero;
        //Debug.Log("Shake! originalPos: " + originalPos);

        float time_el = 0.0f;

        while (time_el < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            time_el += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = Vector3.zero;
    }
}
