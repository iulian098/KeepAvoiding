using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPowerup : MonoBehaviour {
    public float chance = 0.1f;

    public PowerUp[] powerups;

    bool inLevelMode;

    void Start(){
        if (powerups.Length == 0)
            powerups = GetComponentsInChildren<PowerUp> ();
        
        if(LevelsManager.instance)
            inLevelMode = true;
        else
            inLevelMode = false;
        
        if(!CreateLevels.instance || !CreateLevels.instance.usePool)
            SetPowerUp();
    }

    public void SetPowerUp () {
        

        //Debug.Log("OnObjectSpawned called");
        foreach (PowerUp go in powerups) {
            float rand = Random.Range (0f, 1f);
            //Debug.Log (rand);
            if (rand < chance) {
                if(!go.gameObject.activeSelf)
                    go.gameObject.SetActive (true);
            } else {
                if(go.gameObject.activeSelf)
                    go.gameObject.SetActive (false);
            }

            if(go._type == PowerUp.type.ScoreMultiplier && inLevelMode)
                go.gameObject.SetActive(false);
        }
    }

}