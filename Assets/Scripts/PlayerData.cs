using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData")]

public class PlayerData : ScriptableObject
{
    public string _name;
    public bool unlocked;
    public bool useScore;
    public int price;
    public int score;
    public GameObject prefab;
    public Sprite icon;
}
