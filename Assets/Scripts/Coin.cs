using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class Coin : MonoBehaviour {
    public AudioClip pickUpAudio;
    public AudioSource source;
    void OnTriggerEnter2D (Collider2D other) {
        if (other.tag == "Player") {
            gameObject.SetActive (false);
            if (GameManager.instance) {
                GameManager.instance.Coins++;
                GameManager.instance.CollectedCoins++;
            } else if (LevelsManager.instance) {
                LevelsManager.instance.CollectedCoins++;
            }
            SoundManager.instance.PlayCoinSound ();
        }
    }
}