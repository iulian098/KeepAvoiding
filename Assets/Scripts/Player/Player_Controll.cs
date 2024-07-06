using System.Collections;
using System.Collections.Generic;
using SimpleInputNamespace;
using UnityEngine;

public class Player_Controll : MonoBehaviour {

    [System.Serializable]
    public class Power_Up {
        public GameObject magnet;
    }

    public float speed;

    public Vector2 mousePos;

    private Rigidbody2D rb;

    public Transform checkpoint;

    GameManager gm;

    [Range (-1, 1)] public float v, h;

    public Vector3 axisVector;

    public bool useFinger;

    public Power_Up _powerUps;

    public bool die;

    LevelsManager lvManager;
    GameSettings gameSettings;
    Animator anim;
    public ParticleSystem MagnetEffect;

    void Start () {
        rb = this.GetComponent<Rigidbody2D> ();
        gm = GameManager.instance;
        gameSettings = Resources.Load ("GameSettings") as GameSettings;
        lvManager = LevelsManager.instance;
        anim = GetComponent<Animator>();
        if (!anim)
            anim = GetComponentInChildren<Animator>();
    }

    void Update () {

        h = Mathf.Clamp(SimpleInput.GetAxis ("Horizontal"), -1f, 1f);
        v = Mathf.Clamp(SimpleInput.GetAxis ("Vertical"), -1f, 1f);

        if(anim){
            anim.SetFloat("Direction", Mathf.Lerp(anim.GetFloat("Direction"), h, 0.25f));
        }

        if (gameSettings.GameStarted && !gameSettings.Paused) {

            if (gameSettings.useFinger) {

                mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition) - new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y, 0);

                Vector2 localP = Camera.main.transform.InverseTransformVector (mousePos);

                Vector2 targetPos = Vector2.Lerp (this.transform.localPosition, localP, 0.5f);
                //rb.MovePosition(mousePos);
                transform.localPosition = new Vector3 (targetPos.x, targetPos.y, 10);

            } else {

                axisVector += new Vector3 (h * Time.deltaTime * speed, v * Time.deltaTime * speed);

                //Vector2 targetPos = Vector2.Lerp (transform.position, transform.position - axisVector, 0.5f);
                //rb.MovePosition(targetPos);
                transform.localPosition = new Vector3 (axisVector.x, axisVector.y, 10);

            }
        }
    }

    void OnCollisionEnter2D (Collision2D other) {
        if (gm) {
            if (other.gameObject.tag == "Spikes" && !gm.dead && !Player_Powerup.instace.ghost.Active) {
                gm.ShowDeadScreen ();
                SoundManager.instance.PlayDeadSound();
                StartCoroutine(CameraShake.instance.Shake(0.5f, 0.1f));
            }
        } else if (lvManager) {
            if (other.gameObject.tag == "Spikes" && !lvManager.dead) {
                lvManager.Death ();
                SoundManager.instance.PlayDeadSound();
                StartCoroutine(CameraShake.instance.Shake(0.5f, 0.1f));
            }
        }
    }

    void OnTriggerEnter2D (Collider2D other) {

        if (gm) {
            if (other.tag == "Checkpoint") {
                gm.checkpoint = other.transform;
            }
        }else if(lvManager){
            if(other.tag == "Checkpoint"){
                lvManager.checkpoint = other.transform;
            }else if(other.tag == "Finish"){
                lvManager.FinishLevel();
            }
        }

        if (other.tag == "Spikes") {
            die = true;
            if(!Player_Powerup.instace.ghost.Active && SoundManager.instance)
                SoundManager.instance.PlayDeadSound();
        }
    }

    void OnTriggerExit2D (Collider2D other) {
        if (other.tag == "Spikes")
            die = false;
    }

    void OnTriggerStay2D (Collider2D other) {
        if (Player_Powerup.instace.ghost.Active && other.tag == "Spikes")
            die = true;
    }
    
    public void StartMagnetEffect(){
        if(MagnetEffect.isPlaying)
            MagnetEffect.Stop();
        else
            MagnetEffect.Play();
    }
}