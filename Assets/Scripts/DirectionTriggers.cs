using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionTriggers : MonoBehaviour, IObjectPool
{
    public GameObject up,down,left,right;
    public void OnObjectSpawned(){
        switch(CreateLevels.instance._levels.lastDirection){
            case CreateLevels.direction.Up:
                up.SetActive(true);
                break;
            case CreateLevels.direction.Down:
                down.SetActive(true);
                break;
            case CreateLevels.direction.Left:
                left.SetActive(true);
                break;
            case CreateLevels.direction.Right:
                right.SetActive(true);
                break;
        }
    }
}
