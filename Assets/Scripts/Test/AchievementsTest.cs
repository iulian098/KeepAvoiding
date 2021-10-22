using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsTest : MonoBehaviour
{
    public void SetScore(int score){
        GameManager.instance._score._score = score;
        GameManager.instance.ShowDeadScreen();
    }
}
