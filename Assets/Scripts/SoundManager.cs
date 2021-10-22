using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public GameObject audioSourcePrefab;

    public AudioSource source;
    
    public AudioMixerGroup mixer_group;


    public AudioClip deadSound;
    public AudioClip coinPickup;
    public AudioClip powerUpPickup;
    public AudioClip buttonSound;

    GameSettings gameSettings;

    void Awake(){
        instance = this;

        gameSettings = Resources.Load("GameSettings") as GameSettings;
    }

    void Start()
    {

        buttonSound = gameSettings.buttonSound;
        coinPickup = gameSettings.coinSound;
        powerUpPickup = gameSettings.powerUpSound;
        deadSound = gameSettings.dieSound;

        GameObject audioS = Instantiate(audioSourcePrefab, Vector3.zero, Quaternion.identity);
        audioS.transform.SetParent(Camera.main.transform);

        source = audioS.GetComponent<AudioSource>();
        source.outputAudioMixerGroup = mixer_group;
    }

    public void PlayCoinSound(){
        source.PlayOneShot(coinPickup);
    }

    public void PlayPowerUpSound(){
        source.PlayOneShot(powerUpPickup);
    }

    public void PlayDeadSound(){
        source.PlayOneShot(deadSound);
    }
}
