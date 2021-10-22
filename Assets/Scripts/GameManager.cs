using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    #region classes

    [System.Serializable]
    public class PlayerD {
        public string name;
        public bool unlocked;

        public GameObject prefab;
        public Sprite icon;
        public Image lockedImage;

        [Header ("Use ADs")]
        public bool useADs;
        public int totalADs;
        public int WatchedADs;
        public TMP_Text ADsStats;

        [Header ("Use Score")]
        public bool useScore;
        public int score;
        public int coins;

        [Header ("Daily Rewards")]
        public bool DailyReward;
        public int days;

        [Header ("Premium")]
        public bool premium;

        [Header ("Preview")]
        public bool UseCustomPreview;
        public GameObject previewCharacter;
    }

    [System.Serializable]
    public class Score {
        public int _score;
        public int _highscore;

        public TMP_Text _inGameScore;
        public TMP_Text _scoreText;
        public TMP_Text _highscoreText;
    }

    [System.Serializable]
    public class Panels {
        public GameObject MainMenu_Panel;
        public GameObject Death_Panel;
        public GameObject Others_Panel;
        public GameObject Controll;
        public GameObject InGame_Panel;
        public GameObject LoadingAdPanel;
        public GameObject AdFailedToLoadPanel;
        public Animator ContinuePanel;
        public Animator ContinueCountDown;
        public Animator NoInternet_Panel;
        public Animator newHighscore_Panel;

        public Animator ContinuePlayer_Obj;

        public TMP_Text CountDownText;
        public TMP_Text UnlockText;
    }

    [System.Serializable]
    public class Buttons {
        [Header ("Others")]
        public GameObject buyBtn;
        public GameObject watchAdBtn;
        public GameObject continueBtn;
        public GameObject doubleCoinsBtn;

        [Header ("Upgrades")]
        public GameObject magnetUpgradeBtn;
        public GameObject magnetMaxLevelBtn;
        public GameObject slowdownUpgradeBtn;
        public GameObject slowdownMaxLevelBtn;
        public GameObject ghostUpgradeBtn;
        public GameObject ghostMaxLevelBtn;

        public GameObject scoreMultiplierUpgradeBtn;
        public GameObject scoreMultiplierMaxLevelBtn;

        [Header ("Toggles")]
        public GameObject defaultSkin;
    }

    [System.Serializable]
    public class TextManager {
        public TMP_Text magnetPrice;
        public TMP_Text slowdownPrice;
        public TMP_Text ghostPrice;
        public TMP_Text scoreMultiplierPrice;

        public TMP_Text magnetValue;
        public TMP_Text slowdownValue;
        public TMP_Text ghostValue;
        public TMP_Text scoreMultiplierValue;

        public TMP_Text magnetAdd;
        public TMP_Text slowdownAdd;
        public TMP_Text ghostAdd;
        public TMP_Text scoreMultiplierAdd;
    }

    [System.Serializable]
    public class UpgradeBar {
        public List<GameObject> magnetBar;
        public List<GameObject> slowdownBar;
        public List<GameObject> ghostBar;
        public List<GameObject> scoreMultiplierBar;
    }

    [System.Serializable]
    public class ContinuePanel {
        public bool Active;
        public Image fillImage;
        public int showingTime;
        public GameObject SkipButton;

        public float ContinueShowTime = 15f;
        public float CurrentContinueShowTime;
    }

    public DeveloperMode developerMode;

    #endregion

    #region variables

    public static GameManager instance;

    public List<PlayerD> players;
    public List<PlayerD> _scoreCharacters;
    public List<PlayerD> _coinsCharacters;
    public List<PlayerD> _adCharacters;
    public List<PlayerD> _premiumCharacters;
    public Sprite LockedCharacterImage;

    public Dictionary<string, PlayerD> _totalPlayers;

    public UpgradeBar _upgradeBars;
    public ContinuePanel _continuePanel;

    public TextManager _texts;

    public CameraMoving camMoving;
    public Transform _camera;
    float currentMovingSpeed;

    public Transform player;

    public Score _score;
    public int Coins;
    public int CollectedCoins;

    public Panels _panels;
    public Buttons _btns;

    public Transform _levelsContainer;
    public Vector3 _startingPos;
    public Vector3 _playerStartingPos;

    public bool GameStarted;

    public Transform checkpoint;

    public bool useFinger;

    public Transform spawnPoint;

    public GameSettings gs;
    int SelectedCharacterIndex;
    string SelectedCharacterTag;

    [Header ("Colors")]
    public int scoreToChangeColor;
    public List<Color> _colors;
    public Color SelectedColor;
    public bool colorChanged = false;

    [Header ("Character Preview")]
    public GameObject previewCustomImage;
    public GameObject standardImage;

    public GameObject previewCustomImage_Continue;
    public GameObject standardImage_Continue;

    public bool HavePremium;

    float tempScore;
    int gamesPlayed;
    [HideInInspector] public int scoreMultiplier;
    [HideInInspector] public bool Paused;

    [HideInInspector] public bool dead;

    [HideInInspector] public bool loadAd, adFailed;
    public bool _continue;

    public float continueTime = 3f;
    float CurrentContinueTime;

    public bool Continued;

    bool deathCheck;
    bool slowDownEnabled;
    bool premiumCoinsAdded;

    private bool newHighscore;
    private float hs_panelTime = 3f;

    private Player_Powerup ppu;

    #endregion

    void Awake () {
        //Application.targetFrameRate = 60;

        instance = this;

        if (!gs)
            gs = Resources.Load ("GameSettings") as GameSettings;

        _totalPlayers = new Dictionary<string, PlayerD> ();

    }

    void Start () {

        ppu = GameManager.instance.GetComponent<Player_Powerup> ();

        ZPlayerPrefs.Initialize ("somethingSpecial", "someSaltIsNedded");

        if (!PlayerPrefs.HasKey ("CoinsAdded"))
            PlayerPrefs.SetInt ("CoinsAdded", 0);

        gs.GameStarted = false;

        scoreMultiplier = 1;

        developerMode.EnableDisable (false);

        AddAllCharacters ();

        MainMenu.instance.Load ();

        HavePremium = gs.havePremium;

        LoadCharacters ();

        if (!PlayerPrefs.HasKey (_totalPlayers["Default"].name + "Unlocked")) {
            _totalPlayers["Default"].unlocked = true;
            PlayerPrefs.SetInt (_totalPlayers["Default"].name + "Unlocked", IntBoolConverter.IntBoolConvert.ToInt (_totalPlayers["Default"].unlocked));
        }

        _score._highscore = PlayerPrefs.GetInt ("Highscore", 0);
        _startingPos = _levelsContainer.position;

        ChangeCharacter (gs.SelectedCharacterTag);

        if (HavePremium) {
            //Premium things
            PremiumStuff ();

        } else {
            _btns.defaultSkin.SetActive (false);
        }

        _playerStartingPos = player.position;
        currentMovingSpeed = camMoving.speed;

        Coins = gs.Coins;

        Setup ();

        if (gs.Coins != Coins) {
            if (gs.Coins > Coins)
                Coins = gs.Coins;
            else if (Coins > gs.Coins)
                gs.Coins = Coins;
        }
    }

    void FixedUpdate () {
        if (gs.GameStarted && !gs.Paused) {

            //Score controll
            tempScore += (Time.deltaTime * 4) * scoreMultiplier;
            _score._score = (int) tempScore;
            _score._inGameScore.text = _score._score.ToString ();

            //New Highscore Panel
            if (!newHighscore && _score._score > _score._highscore && _score._highscore > 0) {

                hs_panelTime -= Time.deltaTime * 1;

                if (hs_panelTime <= 0) {
                    if (!_panels.newHighscore_Panel.GetCurrentAnimatorStateInfo (0).IsName ("HS_Out"))
                        _panels.newHighscore_Panel.Play ("HS_Out");
                    newHighscore = true;
                } else {
                    if (!_panels.newHighscore_Panel.GetCurrentAnimatorStateInfo (0).IsName ("HS_In"))
                        _panels.newHighscore_Panel.Play ("HS_In");
                }
            }

            //Moving speed controll
            if (currentMovingSpeed > 0 && camMoving.speed < camMoving.maxSpeed) {

                if (camMoving.slowdown && camMoving.speed != 2.2f) {
                    camMoving.speed = 2.2f;
                    //Debug.Log ("SlowDown");
                } else if (!camMoving.slowdown) {
                    camMoving.speed = currentMovingSpeed + (0.0025f * _score._score);
                }

            } else {

                if (camMoving.slowdown && camMoving.speed != 2.2f) {
                    camMoving.speed = 2.2f;
                    //Debug.Log ("SlowDown");
                } else if (!camMoving.slowdown && camMoving.speed != camMoving.maxSpeed) {
                    camMoving.speed = camMoving.maxSpeed;
                }
            }

            //Change color
            if (!colorChanged && _score._score % scoreToChangeColor == 0) {
                SelectedColor = GetRandomColor ();
            }

            if (Camera.main.backgroundColor != SelectedColor)
                Camera.main.backgroundColor = Color.Lerp (Camera.main.backgroundColor, SelectedColor, 0.05f);
            else
                colorChanged = false;

            //Pause/Resume panel
            /*if (gs.Paused && _panels.Controll.activeSelf)
                _panels.Controll.SetActive (false);

            if (!gs.Paused && !_panels.Controll.activeSelf)
                _panels.Controll.SetActive (true);*/
        }

        if (gs.Coins != Coins && !gs.GameStarted) {
            gs.Coins = Coins;

            MainMenu.instance.SaveCoins ();
            MainMenu.instance.CoinsText ();
        }

        if (gs.GameStarted && !deathCheck) {
            EnableDeathCheck ();
        }
        if (!gs.GameStarted && deathCheck) {
            DisableDeathCheck ();
        }

        //Change upgrade buttons
        if (!gs.GameStarted)
            UpgradesButtons ();

        //Continue countdown
        if (_continue) {
            CurrentContinueTime -= Time.deltaTime * 1;
            _panels.CountDownText.text = ((int) CurrentContinueTime).ToString ();
            if (CurrentContinueTime <= 0f && !gs.GameStarted) {
                //Debug.Log ("Continue Game");
                _panels.ContinueCountDown.Play ("Pop_Out");
                //player.gameObject.SetActive(true);
                gs.GameStarted = true;
                _continue = false;
            }
        }

        if (_continuePanel.Active) {
            _continuePanel.CurrentContinueShowTime -= Time.deltaTime * 1;
            _continuePanel.fillImage.fillAmount = _continuePanel.CurrentContinueShowTime / _continuePanel.ContinueShowTime;

            if (_continuePanel.CurrentContinueShowTime <= _continuePanel.ContinueShowTime - 3) {
                _continuePanel.SkipButton.SetActive (true);
            }

            if (_continuePanel.CurrentContinueShowTime <= 0) {
                _panels.ContinuePanel.Play ("Pop_Out");
                _panels.Death_Panel.GetComponent<Animator> ().Play ("Fade_In");
                _continuePanel.Active = false;
            }
        }

        if (HavePremium != gs.havePremium) {
            HavePremium = gs.havePremium;
        }

        //CheckAchievements ();

    }

    void Setup () {
        CheckCharactersScoreUnlock ();

        //SetMagnetUpgradeBar ();
        //SetSlowdownUpgradeBar ();
        //SetGhostUpgradeBar ();
        //SetScoreMultiplierUpgradeBar ();

        //SetUpgradePriceText ();

        SetUpgradesValues ();

        //MainMenu.instance.SpawnLevelsBtns ();

        MainMenu.instance.CheckPremium ();
    }

    #region Game Start/End

    public void StartGame () {

        hs_panelTime = 3f;
        newHighscore = false;

        if (!player)
            player = GameObject.FindGameObjectWithTag ("Player").transform;

        //Request banner
        if (!gs.NoAds || !gs.havePremium) {

        }

        camMoving.speed = camMoving.startingSpeed;

        _score._score = 0;
        tempScore = 0f;

        gs.GameStarted = true;
        gs.Paused = false;

        //Set time scale to 1 if it's 0
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;

        if (CreateLevels.instance)
            CreateLevels.instance.ClearLevels ();

        _levelsContainer.position = _startingPos;

        //Panels

        _panels.Controll.SetActive (true);
        _panels.InGame_Panel.SetActive (true);

        _panels.MainMenu_Panel.GetComponent<Animator> ().Play ("Fade_Out");
        if (_panels.Death_Panel.GetComponent<CanvasGroup> ().alpha == 1) {
            _panels.Death_Panel.GetComponent<Animator> ().Play ("Fade_Out");
        }
        _panels.Others_Panel.GetComponent<Animator> ().Play ("Fade_Out");

        player.gameObject.SetActive (false);

        //Setting up player
        Player_Controll pc = player.GetComponent<Player_Controll> ();
        pc.useFinger = gs.useFinger;
        pc.speed = gs.sensitivity;
        pc.axisVector = Vector3.zero;

        player.localPosition = _playerStartingPos;

        player.gameObject.SetActive (true);

        //Reset collected coins
        CollectedCoins = 0;

        dead = false;

    }

    //Check if player is out of bounds
    void DeathCheck () {
        if (CreateLevels.instance.currentDirection == CreateLevels.direction.Up || CreateLevels.instance.currentDirection == CreateLevels.direction.Down) {
            if ((player.transform.TransformVector (player.transform.position).x < _camera.position.x - 3.15 ||
                    player.transform.TransformVector (player.transform.position).x > _camera.position.x + 3.15 ||
                    player.transform.localPosition.y < -10 ||
                    player.transform.localPosition.y > 10) &&
                !GetComponent<Player_Powerup> ().ghost.Active &&
                !dead
            ) {
                ShowDeadScreen ();
            }
        }
    }

    public void ShowDeadScreen () {

        if (newHighscore && _panels.newHighscore_Panel.GetCurrentAnimatorStateInfo (0).IsName ("HS_In"))
            _panels.newHighscore_Panel.Play ("HS_Out");

        //Panels
        _panels.Controll.SetActive (false);
        _panels.InGame_Panel.SetActive (false);

        gs.GameStarted = false;

        //score/highscore text
        _score._scoreText.text = _score._score.ToString ();
        _score._highscoreText.text = _score._highscore.ToString ();

        //Save highscore
        PlayerPrefs.SetInt ("Highscore", _score._highscore);

        MainMenu.instance._objects.CollectedCoins_text.text = "+" + CollectedCoins.ToString ();

        CheckCharactersScoreUnlock ();

        MainMenu.instance.Save ();

        //Show death screen
        _panels.Death_Panel.GetComponent<Animator> ().Play ("Fade_In");
        _panels.Others_Panel.GetComponent<Animator> ().Play ("Fade_In");

        dead = true;
        gs.Paused = false;

        Player_Powerup.instace.DisableAll ();

    }

    public void Continue () {
        _levelsContainer.transform.position = new Vector3 (_levelsContainer.position.x, checkpoint.position.y, _levelsContainer.position.z);
        player.transform.localPosition = Vector2.zero;

        //Reset mousePos and axisVector
        player.GetComponent<Player_Controll> ().mousePos = Vector2.zero;
        player.GetComponent<Player_Controll> ().axisVector = Vector2.zero;

        //Panels
        _panels.Controll.SetActive (true);
        _panels.InGame_Panel.SetActive (true);

        //Disable panels
        if (_panels.Death_Panel.GetComponent<CanvasGroup> ().alpha == 1) {
            _panels.Death_Panel.GetComponent<Animator> ().Play ("Fade_Out");
        }

        if (_panels.Death_Panel.GetComponent<CanvasGroup> ().alpha == 1)
            _panels.Others_Panel.GetComponent<Animator> ().Play ("Fade_Out");

        _panels.ContinueCountDown.Play ("Pop_In");

        CurrentContinueTime = continueTime;
        _continuePanel.Active = false;
        _continue = true;
        Continued = true;
        dead = false;
        gs.Paused = false;

    }

    public void SkipContinue () {
        _panels.ContinuePanel.Play ("Pop_Out");
        _continuePanel.Active = false;
    }

    public void DoubleCoins () {
        Coins += CollectedCoins;
    }

    void EnableDeathCheck () {
        InvokeRepeating ("DeathCheck", 2f, 2f);
        deathCheck = true;
    }

    void DisableDeathCheck () {
        CancelInvoke ("DeathCheck");
        deathCheck = false;
    }

    #endregion

    #region Windows

    //Show/Hide Windows
    public void ShowWindow (GameObject _window) {
        _window.SetActive (true);
    }

    public void HideWindow (GameObject _window) {
        _window.SetActive (false);
    }

    #endregion

    #region Characters

    public void ChangeCharacter (string Tag) {
        SelectedCharacterTag = Tag;
        PlayerD pd = _totalPlayers[Tag];
        if (pd.unlocked) {
            if (pd.UseCustomPreview) {

                if (standardImage.activeSelf)
                    standardImage.SetActive (false);

                if (!previewCustomImage.activeSelf)
                    previewCustomImage.SetActive (true);

                SetCustomPreview (Tag);

            } else {

                if (!standardImage.activeSelf)
                    standardImage.SetActive (true);

                if (previewCustomImage.activeSelf)
                    previewCustomImage.SetActive (false);

                MainMenu.instance.SetPreviewSprite (pd.icon);
            }

            MainMenu.instance.SetContinueSprite (pd.icon);

            gs.selectedSprite = pd.icon;

            _btns.buyBtn.SetActive (false);
            _btns.watchAdBtn.SetActive (false);
            _panels.UnlockText.gameObject.SetActive (false);

            gs.SelectedCharacterTag = Tag;
            if (player)
                Destroy (player.gameObject);

            //Spawn player
            GameObject _playerObj = Instantiate (pd.prefab, spawnPoint.position, Quaternion.identity);
            _playerObj.transform.SetParent (_camera.transform);
            player = _playerObj.transform;

            //Powerups
            ppu.player = _playerObj;
            ppu.FindPowerup (); //magnet._object = _playerObj.GetComponent<Player_Controll>()._powerUps.magnet;
            SpriteRenderer[] rends = _playerObj.GetComponentsInChildren<SpriteRenderer> ();
            ppu.renderers = new List<SpriteRenderer> (rends);

            //SaveData
            MainMenu.instance.Save ();
            SaveCharacters ();

        } else if (pd.useScore) {

            if (!standardImage.activeSelf)
                standardImage.SetActive (true);

            if (previewCustomImage.activeSelf)
                previewCustomImage.SetActive (false);

            _btns.buyBtn.SetActive (false);
            _btns.watchAdBtn.SetActive (false);

            string txt = "Make %s score\nto unlock.";
            _panels.UnlockText.gameObject.SetActive (true);
            _panels.UnlockText.text = txt.Replace ("%s", pd.score.ToString ());

        } else if (!pd.useScore && !pd.useADs && !pd.DailyReward && !pd.premium) {

            if (!standardImage.activeSelf)
                standardImage.SetActive (true);

            if (previewCustomImage.activeSelf)
                previewCustomImage.SetActive (false);

            _panels.UnlockText.gameObject.SetActive (false);
            _btns.watchAdBtn.SetActive (false);
            _btns.buyBtn.SetActive (true);
            _btns.buyBtn.GetComponentInChildren<TMP_Text> ().text = pd.coins.ToString ();

        } else if (pd.useADs) {

            if (!standardImage.activeSelf)
                standardImage.SetActive (true);

            if (previewCustomImage.activeSelf)
                previewCustomImage.SetActive (false);

            _btns.buyBtn.SetActive (false);
            _panels.UnlockText.gameObject.SetActive (false);
            _btns.watchAdBtn.SetActive (true);
            _btns.watchAdBtn.GetComponentInChildren<TMP_Text> ().text = "Watch AD\n" + pd.WatchedADs + "/" + _totalPlayers[Tag].totalADs;

        } else if (pd.DailyReward) {

            if (!standardImage.activeSelf)
                standardImage.SetActive (true);

            if (previewCustomImage.activeSelf)
                previewCustomImage.SetActive (false);

            _btns.buyBtn.SetActive (false);
            _btns.watchAdBtn.SetActive (false);
            _panels.UnlockText.gameObject.SetActive (true);
            _panels.UnlockText.text = "Play " + pd.days + " days in a row.";

        } else if (pd.premium) {

            if (!standardImage.activeSelf)
                standardImage.SetActive (true);

            if (previewCustomImage.activeSelf)
                previewCustomImage.SetActive (false);

            _btns.buyBtn.SetActive (false);
            _btns.watchAdBtn.SetActive (false);
            _panels.UnlockText.gameObject.SetActive (true);
            _panels.UnlockText.text = "Buy premium to unlock";

        }

        if (!pd.unlocked) {
            MainMenu.instance.SetPreviewSprite (LockedCharacterImage);
        }
    }

    public void BuyCharacterByTag () {
        if (Coins >= _totalPlayers[SelectedCharacterTag].coins) {

            Coins -= _totalPlayers[SelectedCharacterTag].coins;
            _btns.buyBtn.SetActive (false);
            _totalPlayers[SelectedCharacterTag].unlocked = true;
            _totalPlayers[SelectedCharacterTag].lockedImage.gameObject.SetActive (false);

            SaveCharacters ();
            ChangeCharacter (SelectedCharacterTag);
        }
    }

    void CheckCharactersScoreUnlock () {

        foreach (KeyValuePair<string, PlayerD> pd in _totalPlayers) {
            if (_score._highscore >= pd.Value.score && pd.Value.useScore) {
                pd.Value.unlocked = true;
                if (pd.Value.lockedImage)
                    pd.Value.lockedImage.gameObject.SetActive (false);
            } else if (pd.Value.useADs && pd.Value.WatchedADs == pd.Value.totalADs) {
                pd.Value.unlocked = true;
                if (pd.Value.lockedImage)
                    pd.Value.lockedImage.gameObject.SetActive (false);
            }
        }
    }

    //Save/Load Characters
    public void LoadCharacters () {
        foreach (KeyValuePair<string, PlayerD> pd in _totalPlayers) {
            pd.Value.unlocked = System.Convert.ToBoolean (PlayerPrefs.GetInt (pd.Value.name + "Unlocked"));
            pd.Value.WatchedADs = PlayerPrefs.GetInt (pd.Value.name + "WatchedAds", pd.Value.WatchedADs);
            if (pd.Value.unlocked && pd.Value.lockedImage)
                pd.Value.lockedImage.gameObject.SetActive (false);
            if (pd.Value.WatchedADs == pd.Value.totalADs && pd.Value.useADs) {
                pd.Value.unlocked = true;
                if (pd.Value.lockedImage)
                    pd.Value.lockedImage.gameObject.SetActive (false);
            }
            if (HavePremium && pd.Value.premium) {
                pd.Value.unlocked = true;
                if (pd.Value.lockedImage)
                    pd.Value.lockedImage.gameObject.SetActive (false);
            }
        }
    }

    public void SaveCharacters () {
        foreach (KeyValuePair<string, PlayerD> pd in _totalPlayers) {
            PlayerPrefs.SetInt (pd.Value.name + "Unlocked", System.Convert.ToInt32 (pd.Value.unlocked));
            PlayerPrefs.SetInt (pd.Value.name + "WatchedAds", pd.Value.WatchedADs);
        }
    }

    public void SetCustomPreview (string Tag) {
        foreach (KeyValuePair<string, PlayerD> pd in _totalPlayers) {
            if (pd.Key == Tag) {
                pd.Value.previewCharacter.SetActive (true);
            } else {
                if (pd.Value.previewCharacter)
                    pd.Value.previewCharacter.SetActive (false);
            }
        }
    }

    void AddAllCharacters () {

        gs._totalCharacters = new Dictionary<string, GameObject> ();

        //Add score characters
        foreach (PlayerD sp in _scoreCharacters) {
            _totalPlayers.Add (sp.name, sp);
            gs._totalCharacters.Add (sp.name, sp.prefab);
        }

        //Add coins characters
        foreach (PlayerD cp in _coinsCharacters) {
            _totalPlayers.Add (cp.name, cp);
            gs._totalCharacters.Add (cp.name, cp.prefab);
        }

        //Add ADs characters
        foreach (PlayerD ap in _adCharacters) {
            _totalPlayers.Add (ap.name, ap);
            gs._totalCharacters.Add (ap.name, ap.prefab);
        }

        //Add Premium characters
        foreach (PlayerD pc in _premiumCharacters) {
            _totalPlayers.Add (pc.name, pc);
            gs._totalCharacters.Add (pc.name, pc.prefab);
        }

    }

    public int GetUnlockedCharacter () {
        int CharactersUnlocked = 0;
        foreach (KeyValuePair<string, PlayerD> p in _totalPlayers) {
            if (p.Value.unlocked) {
                CharactersUnlocked++;
            }
        }
        return CharactersUnlocked;
    }

    public void AddWatchedAd () {
        _totalPlayers[SelectedCharacterTag].WatchedADs++;
        if (_totalPlayers[SelectedCharacterTag].WatchedADs == _totalPlayers[SelectedCharacterTag].totalADs)
            _totalPlayers[SelectedCharacterTag].unlocked = true;
        SaveCharacters ();
        CheckCharactersScoreUnlock ();
        ChangeCharacter (SelectedCharacterTag);
    }

    #endregion

    #region Upgrades

    public void SetMagnetUpgradeBar () {

        //Disable all
        foreach (GameObject item in _upgradeBars.magnetBar) {
            item.SetActive (false);
        }

        //Enable from 0 to gs.MagnetLeve
        for (int i = 0; i < gs.MagnetLevel; i++) {
            _upgradeBars.magnetBar[i].SetActive (true);
        }
    }

    public void UpgradeMagnet () {

        if (Coins >= gs.MagnetUpgradesCost[gs.MagnetLevel + 1]) {

            Coins -= gs.MagnetUpgradesCost[gs.MagnetLevel + 1];

            gs.MagnetLevel++;

            SetMagnetUpgradeBar ();
            SetUpgradesValues ();

            if (developerMode.Enabled)
                Debug.Log (gs.MagnetDurationUpgrades.Count);

            SetUpgradePriceText ();

            MainMenu.instance.Save ();

        }
    }

    public void SetSlowdownUpgradeBar () {
        //Disable all
        foreach (GameObject item in _upgradeBars.slowdownBar) {
            item.SetActive (false);
        }

        //Enable from 0 to gs.MagnetLeve
        for (int i = 0; i < gs.SlowdownLevel; i++) {
            _upgradeBars.slowdownBar[i].SetActive (true);
        }
    }

    public void UpgradeSlowdown () {

        if (Coins >= gs.SlowdownUpgradesCost[gs.SlowdownLevel + 1]) {

            Coins -= gs.SlowdownUpgradesCost[gs.SlowdownLevel + 1];

            gs.SlowdownLevel++;

            SetSlowdownUpgradeBar ();
            SetUpgradesValues ();

            SetUpgradePriceText ();

            MainMenu.instance.Save ();

        }
    }

    public void SetGhostUpgradeBar () {
        //Disable all
        foreach (GameObject item in _upgradeBars.ghostBar) {
            item.SetActive (false);
        }

        //Enable from 0 to gs.MagnetLeve
        for (int i = 0; i < gs.GhostLevel; i++) {
            _upgradeBars.ghostBar[i].SetActive (true);
        }
    }

    public void UpgradeGhost () {

        if (Coins >= gs.GhostUpgradesCost[gs.GhostLevel + 1]) {

            Coins -= gs.GhostUpgradesCost[gs.GhostLevel + 1];

            gs.GhostLevel++;

            SetGhostUpgradeBar ();
            SetUpgradesValues ();

            SetUpgradePriceText ();

            MainMenu.instance.Save ();

        }
    }

    public void SetScoreMultiplierUpgradeBar () {
        //Disable all
        foreach (GameObject item in _upgradeBars.scoreMultiplierBar) {
            item.SetActive (false);
        }

        //Enable from 0 to gs.MagnetLeve
        for (int i = 0; i < gs.ScoreMultiplierLevel; i++) {
            _upgradeBars.scoreMultiplierBar[i].SetActive (true);
        }
    }

    public void UpgradeScoreMultiplier () {

        if (Coins >= gs.ScoreMultiplierUpgradesCost[gs.ScoreMultiplierLevel + 1]) {

            Coins -= gs.ScoreMultiplierUpgradesCost[gs.ScoreMultiplierLevel + 1];

            gs.ScoreMultiplierLevel++;

            SetScoreMultiplierUpgradeBar ();
            SetUpgradesValues ();

            SetUpgradePriceText ();

            MainMenu.instance.Save ();

        }
    }

    public void SetUpgradePriceText () {
        if (gs.MagnetLevel < gs.MagnetDurationUpgrades.Count - 1)
            _texts.magnetPrice.text = gs.MagnetUpgradesCost[gs.MagnetLevel + 1].ToString ();

        if (gs.SlowdownLevel < gs.SlowdownDurationUpgrades.Count - 1)
            _texts.slowdownPrice.text = gs.SlowdownUpgradesCost[gs.SlowdownLevel + 1].ToString ();

        if (gs.GhostLevel < gs.GhostDurationUpgrades.Count - 1)
            _texts.ghostPrice.text = gs.GhostUpgradesCost[gs.GhostLevel + 1].ToString ();

        if (gs.ScoreMultiplierLevel < gs.ScoreMultiplierDurationUpgrades.Count - 1)
            _texts.scoreMultiplierPrice.text = gs.ScoreMultiplierUpgradesCost[gs.ScoreMultiplierLevel + 1].ToString ();

    }

    public void UpgradesButtons () {
        if (gs.MagnetLevel != gs.MagnetDurationUpgrades.Count - 1) {
            if (_btns.magnetMaxLevelBtn.activeSelf && !_btns.magnetUpgradeBtn.activeSelf) {
                _btns.magnetMaxLevelBtn.SetActive (false);
                _btns.magnetUpgradeBtn.SetActive (true);
            }
        } else {
            if (!_btns.magnetMaxLevelBtn.activeSelf && _btns.magnetUpgradeBtn.activeSelf) {
                _btns.magnetMaxLevelBtn.SetActive (true);
                _btns.magnetUpgradeBtn.SetActive (false);
            }
        }

        if (gs.SlowdownLevel != gs.SlowdownDurationUpgrades.Count - 1) {
            if (_btns.slowdownMaxLevelBtn.activeSelf && !_btns.slowdownUpgradeBtn.activeSelf) {
                _btns.slowdownMaxLevelBtn.SetActive (false);
                _btns.slowdownUpgradeBtn.SetActive (true);
            }
        } else {
            if (!_btns.slowdownMaxLevelBtn.activeSelf && _btns.slowdownUpgradeBtn.activeSelf) {
                _btns.slowdownMaxLevelBtn.SetActive (true);
                _btns.slowdownUpgradeBtn.SetActive (false);
            }
        }

        if (gs.GhostLevel != gs.GhostDurationUpgrades.Count - 1) {
            if (_btns.ghostMaxLevelBtn.activeSelf && !_btns.ghostUpgradeBtn.activeSelf) {
                _btns.ghostMaxLevelBtn.SetActive (false);
                _btns.ghostUpgradeBtn.SetActive (true);
            }
        } else {
            if (!_btns.ghostMaxLevelBtn.activeSelf && _btns.ghostUpgradeBtn.activeSelf) {
                _btns.ghostMaxLevelBtn.SetActive (true);
                _btns.ghostUpgradeBtn.SetActive (false);
            }
        }

        if (gs.ScoreMultiplierLevel != gs.ScoreMultiplierDurationUpgrades.Count - 1) {
            if (_btns.scoreMultiplierMaxLevelBtn.activeSelf && !_btns.scoreMultiplierUpgradeBtn.activeSelf) {
                _btns.scoreMultiplierMaxLevelBtn.SetActive (false);
                _btns.scoreMultiplierUpgradeBtn.SetActive (true);
            }
        } else {
            if (!_btns.scoreMultiplierMaxLevelBtn.activeSelf && _btns.scoreMultiplierUpgradeBtn.activeSelf) {
                _btns.scoreMultiplierMaxLevelBtn.SetActive (true);
                _btns.scoreMultiplierUpgradeBtn.SetActive (false);
            }
        }
    }

    public void SetUpgradesValues () {
        //Get time for each powerup
        gs.MagnetTime = gs.MagnetDurationUpgrades[gs.MagnetLevel];
        gs.SlowdownTime = gs.SlowdownDurationUpgrades[gs.SlowdownLevel];
        gs.GhostTime = gs.GhostDurationUpgrades[gs.GhostLevel];
        gs.ScoreMultiplierTime = gs.ScoreMultiplierDurationUpgrades[gs.ScoreMultiplierLevel];

        //Get duretion from powerups
        _texts.magnetValue.text = "Duration: " + gs.MagnetTime.ToString ();
        _texts.slowdownValue.text = "Duration: " + gs.SlowdownTime.ToString ();
        _texts.ghostValue.text = "Duration: " + gs.GhostTime.ToString ();
        _texts.scoreMultiplierValue.text = "Duration: " + gs.ScoreMultiplierTime.ToString ();

        //Get upgrate text
        if (gs.MagnetLevel < gs.MagnetDurationUpgrades.Count - 1)
            _texts.magnetAdd.text = "+" + (gs.MagnetDurationUpgrades[gs.MagnetLevel + 1] - gs.MagnetTime);
        else
            _texts.magnetAdd.text = "";

        if (gs.SlowdownLevel < gs.SlowdownDurationUpgrades.Count - 1)
            _texts.slowdownAdd.text = "+" + (gs.SlowdownDurationUpgrades[gs.SlowdownLevel + 1] - gs.SlowdownTime);
        else
            _texts.slowdownAdd.text = "";

        if (gs.GhostLevel < gs.GhostDurationUpgrades.Count - 1)
            _texts.ghostAdd.text = "+" + (gs.GhostDurationUpgrades[gs.GhostLevel + 1] - gs.GhostTime);
        else
            _texts.ghostAdd.text = "";

        if (gs.ScoreMultiplierLevel < gs.ScoreMultiplierDurationUpgrades.Count - 1)
            _texts.scoreMultiplierAdd.text = "+" + (gs.ScoreMultiplierDurationUpgrades[gs.ScoreMultiplierLevel + 1] - gs.ScoreMultiplierTime);
        else
            _texts.scoreMultiplierAdd.text = "";

    }

    #endregion

    Color GetRandomColor () {
        colorChanged = true;
        return _colors[Random.Range (0, _colors.Count)];
    }

    public void ClearData () {
        PlayerPrefs.DeleteAll ();
        SceneManager.LoadScene (0);
    }

    public void SetPaused (bool p) {
        gs.Paused = p;
        _panels.Controll.SetActive (!p);
    }

    public void ResetPlayer () {
        gs.GameStarted = false;
        player.localPosition = _playerStartingPos;
        _levelsContainer.position = _startingPos;
        _score._score = 0;
    }

    #region PanelsControl

    public void NoInternetConnection () {
        _panels.NoInternet_Panel.Play ("Pop_In");
    }

    #endregion

    public void PremiumChange (bool b) {
        gs.havePremium = b;
    }

    public void SetDebugMode (Toggle t) {
        developerMode.EnableDisable (t.isOn);
    }

    public void PremiumStuff () {
        _btns.defaultSkin.SetActive (true);
        MainMenu.instance._premium.adsBtn.SetActive (false);
        gs.NoAds = true;
        gs.havePremium = true;

        premiumCoinsAdded = System.Convert.ToBoolean (PlayerPrefs.GetInt ("CoinsAdded", 0));

        if (!premiumCoinsAdded) {
            //Coins += 10000;
            premiumCoinsAdded = true;
            PlayerPrefs.SetInt ("CoinsAdded", System.Convert.ToInt32 (premiumCoinsAdded));
        }

        //Unlock premium characters
        foreach (KeyValuePair<string, PlayerD> pc in _totalPlayers) {
            if (pc.Value.premium) {
                pc.Value.unlocked = true;
                pc.Value.lockedImage.gameObject.SetActive (false);

                if (developerMode.Enabled)
                    Debug.Log ("Have premium, unlock " + pc.Value.name);

            }
        }
        MainMenu.instance.Save ();
        MainMenu.instance.CheckPremium ();
    }

    public void GetData () {
        if (!gs.restored)
            PhpDatabase.instance.GetData ();
    }

    public void CheckLevels () {
        //Check level
        if (PlayerPrefs.GetString ("Levels") == "" || !PlayerPrefs.HasKey ("Levels")) {
            gs.levelsData = "";
            for (int i = 0; i < gs.LevelsNr; i++) {
                gs.levelsData = gs.levelsData + "0";
            }
            PlayerPrefs.SetString ("Levels", gs.levelsData);
        } else {
            gs.levelsData = PlayerPrefs.GetString ("Levels");
            if (gs.levelsData.Length < gs.LevelsNr) {
                for (int i = gs.levelsData.Length; i < gs.LevelsNr; i++) {
                    gs.levelsData = gs.levelsData + "0";
                }
                PlayerPrefs.SetString ("Levels", gs.levelsData);
            }
        }

        //Check for unlocked levels
        if (PlayerPrefs.GetString ("LevelsU") == "" || !PlayerPrefs.HasKey ("LevelsU")) {
            gs.levelsUnlocked = "";
            for (int i = 0; i < gs.LevelsNr; i++) {
                if (i == 0)
                    gs.levelsUnlocked = gs.levelsUnlocked + "1";
                else
                    gs.levelsUnlocked = gs.levelsUnlocked + "0";
            }

            PlayerPrefs.SetString ("LevelsU", gs.levelsUnlocked);

        } else {
            gs.levelsUnlocked = PlayerPrefs.GetString ("LevelsU");

            if (gs.levelsUnlocked.Length < gs.LevelsNr) {
                for (int i = gs.levelsUnlocked.Length; i < gs.LevelsNr; i++) {
                    gs.levelsUnlocked = gs.levelsUnlocked + "0";
                }
                PlayerPrefs.SetString ("LevelsU", gs.levelsUnlocked);
            }
        }
    }

}