using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings")]
public class GameSettings : ScriptableObject
{

    [Header("Player Data")]
    public string playerName;

    public int coins1;
    public int coins2;
    public int coins3;

    public Sprite selectedSprite;

    public bool restored;

    [Header("Main data")]
    public int SelectedCharacter;
    public string SelectedCharacterTag;
    public bool useFinger;
    public float sensitivity;
    public int Coins;

    public bool NoAds;
    public bool havePremium;
    public bool useDefaultSkin;

    public List<Color> backgroundColors;

    [Header("Levels data")]
    public GameObject LevelBtnPrefab;
    public int LevelsNr;
    public int SelectedLevel;
    public int SelectedStage;
    public int LevelsPerStage = 50;
    public int LevelsInRow;
    public string levelsData;
    public string levelsUnlocked;
    public string levelName;
    public bool GameStarted;
    public bool Paused;

    [Header("Camera Effects")]
    public bool cameraEffects;
    public bool Bloom;
    public bool Vignette;

    [Header("Characters")]
    public Dictionary<string, GameObject> _totalCharacters = new Dictionary<string, GameObject>();

    [Header("Sounds")]
    public AudioClip dieSound;
    public AudioClip coinSound;
    public AudioClip powerUpSound;
    public AudioClip buttonSound;

    [Header("Audio")]
    public AudioMixer mixer;

    public AudioMixerGroup SFX_Group;
    public AudioMixerGroup Music_Group;
    public float SFX_Volume;
    public float Music_Volume;

    [Header("Magnet")]
    public int MagnetLevel;
    public float MagnetTime;
    public float MagnetRange;

    public List<float> MagnetDurationUpgrades;
    public List<int> MagnetUpgradesCost;
    
    [Header("Slowdown")]
    public int SlowdownLevel;
    public float SlowdownTime;
    public List<float> SlowdownDurationUpgrades;
    public List<int> SlowdownUpgradesCost;

    [Header("Ghost")]
    public int GhostLevel;
    public float GhostTime;

    public List<float> GhostDurationUpgrades;
    public List<int> GhostUpgradesCost;

    [Header("Score Multiplier")]
    public int ScoreMultiplierLevel;
    public float ScoreMultiplierTime;

    public List<float> ScoreMultiplierDurationUpgrades;
    public List<int> ScoreMultiplierUpgradesCost;



}
