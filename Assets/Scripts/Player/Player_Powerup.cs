using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Powerup : MonoBehaviour {

    public static Player_Powerup instace;

    [System.Serializable]
    public class PU {
        public bool Active;
        public GameObject _object;
        public GameObject sprite;
        public Image timeCounter;
        public float time;
        public float currentTime;

    }

    public GameObject player;

    public GameSettings gs;

    public PU magnet;
    public PU slowdown;
    public PU ghost;
    public PU x2_Multiplier;

    Collider2D player_coll;
    Player_Controll player_Controll;

    CameraMoving camMoving;
    private static readonly int Ghost = Shader.PropertyToID ("_Ghost");
    public List<SpriteRenderer> renderers;
    void Awake () {
        instace = this;
    }

    void Start () {

        //Load settings
        if (!gs)
            gs = Resources.Load ("GameSettings") as GameSettings;

        //Get player gameObject
        player = GameObject.FindGameObjectWithTag ("Player");

        camMoving = FindObjectOfType (typeof (CameraMoving)) as CameraMoving;
        FindPowerup ();

        SpriteRenderer[] renderer = player.GetComponentsInChildren<SpriteRenderer> ();
        renderers = new List<SpriteRenderer> (renderer);
    }

    void FixedUpdate () {
        if (!player) {
            player = GameObject.FindGameObjectWithTag ("Player");

            if (player) {
                Debug.Log("Player found");
                SpriteRenderer[] renderer = player.GetComponentsInChildren<SpriteRenderer> ();
                renderers = new List<SpriteRenderer> (renderer);
                Debug.Log("Renderers found : " + renderer.Length);
                FindPowerup ();
            }
        }


        //Magent
        if (magnet.Active) {
            magnet.time -= Time.deltaTime * 1;
            ActiveMagnet ();
            if (magnet.time <= 0)
                StopMagnet ();
        }

        //Slowdown
        if (slowdown.Active) {
            slowdown.time -= Time.deltaTime * 1;
            ActiveSlowdown ();
            if (slowdown.time <= 0)
                StopSlowdown ();
        }

        //Ghost
        if (ghost.Active) {
            ghost.time -= Time.deltaTime * 1;
            ActiveGhost ();
            if (ghost.time <= 0)
                StopGhost ();
        }

        //Multiplied
        if (x2_Multiplier.Active) {
            x2_Multiplier.time -= Time.deltaTime * 1;
            ActiveScoreMultiplier ();
            if (x2_Multiplier.time <= 0)
                StopScoreMultiplier ();
        }
    }

    #region PowerUps
    public void StartMagnet () {
        magnet.time = gs.MagnetTime;
        magnet.Active = true;
        magnet._object.SetActive (true);
        magnet.sprite.SetActive (true);
    }

    private void ActiveMagnet () {
        magnet.timeCounter.fillAmount = magnet.time / gs.MagnetTime;
    }

    private void StopMagnet () {
        magnet.Active = false;
        magnet._object.SetActive (false);
        magnet.sprite.SetActive (false);
    }

    public void StartSlowdown () {
        slowdown.time = gs.SlowdownTime;
        slowdown.Active = true;
        slowdown.sprite.SetActive (true);
        camMoving.EnableSlowdown ();

    }

    private void ActiveSlowdown () {
        slowdown.timeCounter.fillAmount = slowdown.time / gs.SlowdownTime;
    }

    private void StopSlowdown () {
        slowdown.Active = false;
        slowdown.sprite.SetActive (false);
        camMoving.DisableSlowdown ();

    }

    public void StartGhost () {
        ghost.time = gs.GhostTime;
        if (player_coll) {
            player_coll.isTrigger = true;
        }

        foreach (SpriteRenderer _r in renderers) {
            if (_r.material.HasProperty (Ghost))
                _r.material.SetFloat (Ghost, 1f);
        }

        ghost.Active = true;
        ghost.sprite.SetActive (true);
    }

    private void ActiveGhost () {
        ghost.timeCounter.fillAmount = ghost.time / gs.GhostTime;
    }

    private void StopGhost () {
        if (player_coll) {
            player_coll.isTrigger = false;
        }
        if (player_Controll.die && !GameManager.instance.dead)
            GameManager.instance.ShowDeadScreen ();

        foreach (SpriteRenderer _r in renderers) {
            if (_r.material.HasProperty (Ghost))
                _r.material.SetFloat (Ghost, 0f);
        }
        ghost.Active = false;
        ghost.sprite.SetActive (false);
    }

    public void StartScoreMultiplier () {
        x2_Multiplier.time = gs.ScoreMultiplierTime;

        if (GameManager.instance)
            GameManager.instance.scoreMultiplier = 2;

        x2_Multiplier.Active = true;
        x2_Multiplier.sprite.SetActive (true);
    }

    private void ActiveScoreMultiplier () {
        x2_Multiplier.timeCounter.fillAmount = x2_Multiplier.time / gs.ScoreMultiplierTime;
    }

    private void StopScoreMultiplier () {

        if (GameManager.instance)
            GameManager.instance.scoreMultiplier = 1;

        x2_Multiplier.Active = false;
        x2_Multiplier.sprite.SetActive (false);
    }

    #endregion

    public void FindPowerup () {

        player_Controll = player.GetComponent<Player_Controll> ();
        player_coll = player.GetComponent<Collider2D> ();

        foreach (Transform go in player.GetComponentsInChildren<Transform> ()) {
            if (go.name == "Magnet")
                magnet._object = go.gameObject;
        }

        Debug.Log ("Finding popwer up");

        magnet._object.SetActive (false);
    }

    //Disable all powerups
    public void DisableAll () {
        if (player) {
            StopMagnet ();
            StopGhost ();
            StopSlowdown ();
        }
    }
}