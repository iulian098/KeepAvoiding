using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBtn : MonoBehaviour {
    public int Level;
    public int Stage;
    public bool completed;
    public bool unlocked;
    public GameObject checkImg, lockedImg;

    void SelectLevel () {
        MainMenu.instance.SelectLevel (Level, Stage);
    }

    public void CheckCompleted () {
        if (completed)
            checkImg.SetActive (true);
        else
            checkImg.SetActive (false);

        if (unlocked) {
            lockedImg.SetActive (false);
            this.GetComponent<Button> ().onClick.AddListener (() => SelectLevel ());
        } else {
            lockedImg.SetActive (true);
            this.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

}