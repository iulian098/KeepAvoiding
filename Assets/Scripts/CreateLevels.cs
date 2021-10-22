using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLevels : MonoBehaviour {

    public static CreateLevels instance;

    public enum direction {
        Left,
        Right,
        Up,
        Down
    }

    [System.Serializable]
    public class Levels {
        public direction lastDirection;
        public List<GameObject> easy;
        public List<GameObject> medium;
        public List<GameObject> hard;

        public List<GameObject> side;

        public GameObject DownToLeft, DownToRight, UpToLeft, UpToRight;
    }

    public direction currentDirection;

    public bool useDirections;

    public Levels _levels;

    public float offsetY;
    public float offsetX;
    public bool usePool;

    public Transform LevelsContainer;

    public Transform _Cam;
    public Transform currentPos;
    public Transform startingPos;

    public List<GameObject> _Created_levels;

    Vector3 pos;

    public List<GameObject> totalLevels;
    public List<GameObject> directionLevels;

    bool mediumAdded;
    bool hardAdded;

    public GameSettings gameSettings;

    private ObjectPool objP;

    private float DistanceFromObj;

    void Awake () {
        instance = this;
    }

    void Start () {
        currentDirection = direction.Up;
        AddEasyLevels ();
        currentPos = startingPos;
        _levels.lastDirection = direction.Up;

        objP = ObjectPool.instance;
    }

    void FixedUpdate () {

        if (useDirections) {

            if (_levels.lastDirection == direction.Up)
                DistanceFromObj = currentPos.position.y - _Cam.position.y;
            if (_levels.lastDirection == direction.Down)
                DistanceFromObj = _Cam.position.y - currentPos.position.y;
            if (_levels.lastDirection == direction.Right)
                DistanceFromObj = currentPos.position.x - _Cam.position.x;
            if (_levels.lastDirection == direction.Left)
                DistanceFromObj = _Cam.position.x - currentPos.position.x;
        }

        if (useDirections) {
            if (gameSettings.GameStarted && DistanceFromObj < 5) {
                GameObject go;
                if (!usePool) {
                    go = Instantiate (totalLevels[Random.Range (0, totalLevels.Count)], currentPos.position + new Vector3 (0, offsetY, 0), Quaternion.identity);
                    go.transform.SetParent (LevelsContainer);
                    SetDirection (go);
                } else {
                    go = null;
                    float rand = Random.Range (0f, 1f);

                    if (rand <= 0.8f) {
                        if (_levels.lastDirection == direction.Up)
                            go = objP.Spawn (totalLevels[Random.Range (0, totalLevels.Count)].name, currentPos.position + new Vector3 (0, offsetY, 0), Quaternion.identity);
                        else if (_levels.lastDirection == direction.Down)
                            go = objP.Spawn (totalLevels[Random.Range (0, totalLevels.Count)].name, currentPos.position + new Vector3 (0, -offsetY, 0), Quaternion.identity);
                        else if (_levels.lastDirection == direction.Left)
                            go = objP.Spawn (_levels.side[Random.Range (0, _levels.side.Count)].name, currentPos.position + new Vector3 (-offsetX, 0, 0), Quaternion.identity);
                        else if (_levels.lastDirection == direction.Right)
                            go = objP.Spawn (_levels.side[Random.Range (0, _levels.side.Count)].name, currentPos.position + new Vector3 (offsetX, 0, 0), Quaternion.identity);

                    } else {

                        float randDir = Random.Range (0f, 1f);

                        switch (_levels.lastDirection) {

                            case direction.Up:
                                if (randDir <= 0.5f)
                                    go = objP.Spawn (_levels.UpToLeft.name, currentPos.position + new Vector3 (0, offsetY, 0), Quaternion.identity);
                                else
                                    go = objP.Spawn (_levels.UpToRight.name, currentPos.position + new Vector3 (0, offsetY, 0), Quaternion.identity);
                                break;

                            case direction.Left:
                                if (randDir <= 0.5f)
                                    go = objP.Spawn (_levels.UpToRight.name, currentPos.position + new Vector3 (-offsetX, 0, 0), Quaternion.identity);
                                else
                                    go = objP.Spawn (_levels.DownToRight.name, currentPos.position + new Vector3 (-offsetX, 0, 0), Quaternion.identity);
                                break;

                            case direction.Right:
                                if (randDir <= 0.5f)
                                    go = objP.Spawn (_levels.DownToLeft.name, currentPos.position + new Vector3 (offsetX, 0, 0), Quaternion.identity);
                                else
                                    go = objP.Spawn (_levels.UpToLeft.name, currentPos.position + new Vector3 (offsetX, 0, 0), Quaternion.identity);
                                break;

                            case direction.Down:
                                if (randDir <= 0.5f)
                                    go = objP.Spawn (_levels.DownToLeft.name, currentPos.position + new Vector3 (0, -offsetY, 0), Quaternion.identity);
                                else
                                    go = objP.Spawn (_levels.DownToRight.name, currentPos.position + new Vector3 (0, -offsetY, 0), Quaternion.identity);
                                break;
                        }

                        SetDirection (go);
                    }

                }
                _Created_levels.Add (go);
                currentPos = go.transform;
            }
        } else {

            if (gameSettings.GameStarted && currentPos.position.y - _Cam.position.y < 5) {
                GameObject go;
                if (!usePool) {
                    go = Instantiate (totalLevels[Random.Range (0, totalLevels.Count)], currentPos.position + new Vector3 (0, offsetY, 0), Quaternion.identity);
                    go.transform.SetParent (LevelsContainer);
                    SetDirection (go);
                } else {

                    go = null;
                    go = objP.Spawn (totalLevels[Random.Range (0, totalLevels.Count)].name, currentPos.position + new Vector3 (0, offsetY, 0), Quaternion.identity);

                }
                _Created_levels.Add (go);
                currentPos = go.transform;
            }
        }

        if (_Created_levels.Count > 4) {
            GameObject go = _Created_levels[0];
            _Created_levels.Remove (_Created_levels[0]);
            if (!usePool)
                Destroy (go);
            else
                go.SetActive (false);
        }

        if (!mediumAdded && GameManager.instance._score._score > 200) {
            mediumAdded = true;
            foreach (GameObject mlv in _levels.medium) {
                if (!totalLevels.Contains (mlv))
                    totalLevels.Add (mlv);
            }
        }

        if (!hardAdded && GameManager.instance._score._score > 350) {
            hardAdded = true;
            foreach (GameObject hlv in _levels.hard) {
                if (!totalLevels.Contains (hlv))
                    totalLevels.Add (hlv);
            }
        }
    }

    public void GetTotalLevels () {
        AddEasyLevels ();
    }

    public void ClearLevels () {
        for (int i = 0; i < _Created_levels.Count; i++) {
            GameObject lvl = _Created_levels[i];
            if (!usePool)
                Destroy (lvl);
            else
                lvl.SetActive (false);
        }
        _Created_levels.Clear ();
        currentPos = startingPos;

        totalLevels.Clear ();

        AddEasyLevels ();
        mediumAdded = false;
        hardAdded = false;
    }

    public void AddEasyLevels () {
        foreach (GameObject elv in _levels.easy) {
            totalLevels.Add (elv);
        }
    }

    private void SetDirection (GameObject go) {
        if (_levels.lastDirection == direction.Up) {

            if (go == _levels.UpToLeft)
                _levels.lastDirection = direction.Left;
            else if (go == _levels.UpToRight)
                _levels.lastDirection = direction.Right;
            Debug.Log ("Direction changed from up");

        } else if (_levels.lastDirection == direction.Down) {

            if (go == _levels.DownToLeft)
                _levels.lastDirection = direction.Left;
            else if (go == _levels.DownToRight)
                _levels.lastDirection = direction.Right;

        } else if (_levels.lastDirection == direction.Left) {

            if (go == _levels.DownToRight)
                _levels.lastDirection = direction.Up;
            else if (go == _levels.UpToRight)
                _levels.lastDirection = direction.Down;

        } else if (_levels.lastDirection == direction.Right) {
            if (go == _levels.DownToLeft)
                _levels.lastDirection = direction.Up;
            else if (go == _levels.UpToLeft)
                _levels.lastDirection = direction.Down;
        }
    }
}