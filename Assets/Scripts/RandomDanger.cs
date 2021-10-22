using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDanger : MonoBehaviour {

    public List<GameObject> _enemyes;

    void Start () {
        if (!CreateLevels.instance || !CreateLevels.instance.usePool)
            SetDanger ();
    }

    public void SetDanger () {
        foreach (GameObject go in _enemyes) {
            float rand = Random.Range (0f, 1f);
            //Debug.Log(rand);
            if (rand >= 0.5) {
                if (!go.activeSelf)
                    go.SetActive (true);
            } else {
                if (go.activeSelf)
                    go.SetActive (false);
            }
        }
    }

}