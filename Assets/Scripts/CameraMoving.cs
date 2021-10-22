using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour {

    public bool useDirections;

    public float startingSpeed;
    public float speed;
    public float maxSpeed;

    public GameSettings gs;

    [HideInInspector] public bool slowdown;

    void Update () {
        if (gs.GameStarted && !gs.Paused) {
            if (useDirections) {
                if (CreateLevels.instance.currentDirection == CreateLevels.direction.Up)
                    this.transform.position += Vector3.up * Time.deltaTime * speed;
                if (CreateLevels.instance.currentDirection == CreateLevels.direction.Left)
                    this.transform.position += Vector3.left * Time.deltaTime * speed;
                if (CreateLevels.instance.currentDirection == CreateLevels.direction.Right)
                    this.transform.position += Vector3.right * Time.deltaTime * speed;
                if (CreateLevels.instance.currentDirection == CreateLevels.direction.Down)
                    this.transform.position += Vector3.down * Time.deltaTime * speed;
            }else{
                this.transform.position += Vector3.up * Time.deltaTime * speed;
            }
        }

    }
    public void EnableSlowdown () {
        slowdown = true;
    }

    public void DisableSlowdown () {
        slowdown = false;
    }
}