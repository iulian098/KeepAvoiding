using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCoin : MonoBehaviour{
    public Coin[] coins;

    void Awake () {
        if (coins.Length < 1)
            coins = GetComponentsInChildren<Coin> ();
        if (!CreateLevels.instance || !CreateLevels.instance.usePool)
            SetCoins ();
    }

    public void SetCoins () {
        foreach (Coin item in coins) {
            float rnr = Random.Range(0f, 1f);
           // Debug.Log(rnr);
            if (rnr > 0.65f) {
                item.gameObject.SetActive (true);
            } else {
                item.gameObject.SetActive (false);
            }
        }
    }
}