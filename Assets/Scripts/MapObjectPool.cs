using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectPool : MonoBehaviour, IObjectPool {

    RandomCoin coins;
    RandomDanger danger;
    RandomPowerup powerup;

    void Awake () {
        coins = GetComponent<RandomCoin> ();
        danger = GetComponent<RandomDanger> ();
        powerup = GetComponent<RandomPowerup> ();
    }

    public void OnObjectSpawned () {
        if (coins)
            coins.SetCoins ();
        if (danger)
            danger.SetDanger ();
        if (powerup)
            powerup.SetPowerUp ();
    }
}