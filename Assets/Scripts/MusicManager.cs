using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public List<AudioClip> _clips;
    public AudioSource _source;

    public static MusicManager instance{
        get
        {
            if(Instance == null)
            {
                Instance = GameObject.FindObjectOfType<MusicManager>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(Instance.gameObject);
            }

            return Instance;
        }
    }

    void Awake(){
        if(Instance == null)
        {
            //If I am the first instance, make me the Singleton
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if(this != Instance)
                Destroy(this.gameObject);
        }

    }

    void Start(){
        if(!_source)
            _source = this.GetComponent<AudioSource>();
        RandomPlay();
       //_source.PlayOneShot(_clips[Random.Range(0, _clips.Count)]);
    }

    void FixedUpdate(){
        if(!_source.isPlaying){
            //_source.PlayOneShot(_clips[Random.Range(0, _clips.Count)]);
            RandomPlay();
        }
    }

    void RandomPlay(){
        _source.clip = _clips[Random.Range(0, _clips.Count)];
        _source.Play();
    }
    
}
