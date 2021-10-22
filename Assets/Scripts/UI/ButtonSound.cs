using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonSound : MonoBehaviour
{
    public Button btn;
    public AudioClip clip;
    public AudioSource source;
    void Start()
    {

        if(this.GetComponent<AudioSource>())
            source = this.GetComponent<AudioSource>();
        else
            source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        
        if(GameManager.instance){
            source.outputAudioMixerGroup = GameManager.instance.gs.SFX_Group;
            clip = GameManager.instance.gs.buttonSound;
        }else if(LevelsManager.instance){
            source.outputAudioMixerGroup = LevelsManager.instance.gs.SFX_Group;
            clip = LevelsManager.instance.gs.buttonSound;
        }

        
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(StartSound);
    }

    public void StartSound(){
        source.PlayOneShot(clip);
    }
}
