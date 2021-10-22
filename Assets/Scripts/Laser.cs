using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
    public Transform shootPoint;
    public int enableTime;
    public int disableTime;
    public float currentTime;
    public LineRenderer laserLine;

    public bool _enable;
    void FixedUpdate () {
        currentTime -= Time.deltaTime * 1;
        if (currentTime <= 0) {
            _enable = !_enable;
            if(_enable){
                currentTime = enableTime;
            }else{
                currentTime = disableTime;
            }
        }
        EnableDisableLaser ();

    }

    void EnableDisableLaser () {
        if (_enable) {
            laserLine.gameObject.SetActive (true);
            RaycastHit2D hit = Physics2D.Raycast (shootPoint.position, shootPoint.right);

            if (hit.collider.tag == "Player") {
                if (GameManager.instance && !GameManager.instance.dead)
                    GameManager.instance.ShowDeadScreen ();
                else if (LevelsManager.instance && !LevelsManager.instance.dead)
                    LevelsManager.instance.Death ();
            }

            Debug.DrawLine (shootPoint.position, hit.point, Color.red);
            laserLine.SetPosition (0, shootPoint.position);
            laserLine.SetPosition (1, hit.point);
        } else {
            laserLine.gameObject.SetActive (false);
        }
    }
}