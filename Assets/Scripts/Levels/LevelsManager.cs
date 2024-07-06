using System.Collections;
using System.Collections.Generic;
using SimpleInputNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelsManager : MonoBehaviour {
    public static LevelsManager instance;

    [System.Serializable]
    public class Panels {
        public GameObject Controller;
        public GameObject DeathPanel;
        public GameObject FinishPanel;
        public GameObject vignette;
        public GameObject LoadingAdPanel;
        public GameObject AdFailedToLoadPanel;
        public TMP_Text CountDownText;
        public Animator ContinueCountDown;
        public Animator ContinuePanel;
    }

    [System.Serializable]
    public class Texts {
        public Animator holdText;
        public TMP_Text collectedCoins;
        public TMP_Text totalCoins;
    }

    [System.Serializable]
    public class Premium {
        public Sprite currentButton;
        public Sprite premiumButton;
        public List<Image> buttons;

        public Sprite currentPanel;
        public Sprite premiumPanel;
        public List<Image> panels;
    }

    public Premium _premium;

    public GameObject doubleCoinsBtn;

    public Panels _panels;
    public Texts _texts;

    public List<GameObject> Levels;

    public GameSettings gs;
    public Transform player;

    public Transform _camera;
    public bool dead;
    public Transform checkpoint;
    public Vector3 _playerStartingPos;
    public Transform _camStartingPos;
    public int CollectedCoins;
    public int CurrentCoins;
    public int totalCoins;

    public List<Color> _colors;

    float mainSpeed;

    string levelData;

    CameraMoving camMove;
    public Transform _cameraContainer;

    bool loadAd;
    public bool _continue;
    float CurrentContinueTime;

    public float continueTime = 3f;

    bool continued;

    public GameManager.ContinuePanel _continuePanel;

    public Image playerPreview;

    MouseButtonInputUI controller_Input;

    char[] lvlUnlock;
    bool levelStarted;

    RandomPowerup[] randomPowerups;

    public Sprite btnSprite;
    public Sprite panelSprite;

    public Transform Checkpoint;

    void Awake () {
        instance = this;

        lvlUnlock = gs.levelsUnlocked.ToCharArray ();

        gs.useDefaultSkin = System.Convert.ToBoolean (ZPlayerPrefs.GetInt ("UseDefaultSkin", 0));
    }

    void Start () {

        controller_Input = _panels.Controller.GetComponent<MouseButtonInputUI> ();

        CurrentCoins = gs.Coins;
        _colors = gs.backgroundColors;
        //_camera = Camera.main.transform;

        //Find camera script
        camMove = FindObjectOfType (typeof (CameraMoving)) as CameraMoving;
        mainSpeed = camMove.speed;

        _texts.holdText.Play ("Pop_In");

        //Assing random background clor
        if (_colors.Count == 0) {
            Camera.main.backgroundColor = Random.ColorHSV ();
        } else {
            Camera.main.backgroundColor = _colors[Random.Range (0, _colors.Count)];
        }

        _panels.vignette.SetActive (gs.Vignette);

        Levels[gs.SelectedLevel].gameObject.SetActive (true);

        SpawnPlayer ();

        levelData = gs.levelsData;

        randomPowerups = FindObjectsOfType (typeof (RandomPowerup)) as RandomPowerup[];

        //Premium
        if (gs.havePremium) {

            //Set buttons
            _premium.currentButton = _premium.buttons[0].sprite;

            if (gs.useDefaultSkin)
                btnSprite = _premium.currentButton;
            else
                btnSprite = _premium.premiumButton;

            foreach (Image btn in _premium.buttons) {
                btn.sprite = btnSprite;
            }

            //Set panels
            _premium.currentPanel = _premium.panels[0].sprite;

            if (gs.useDefaultSkin)
                panelSprite = _premium.currentPanel;
            else
                panelSprite = _premium.premiumPanel;

            foreach (Image pan in _premium.panels) {
                pan.sprite = panelSprite;
            }

        }

        playerPreview.sprite = gs.selectedSprite;

        if (Player_Powerup.instace)
            Player_Powerup.instace.FindPowerup ();

    }

    void FixedUpdate () {
        //Cam Speed Control
        if (camMove.slowdown) {
            if (camMove.speed != 2.2f) {
                camMove.speed = 2.2f;
            }
        } else {
            if (camMove.speed != mainSpeed)
                camMove.speed = mainSpeed;
        }

        //Continue countdown
        if (_continue) {
            CurrentContinueTime -= Time.deltaTime * 1;
            _panels.CountDownText.text = ((int) CurrentContinueTime).ToString ();
            if (CurrentContinueTime <= 0f && !gs.GameStarted) {
                Debug.Log ("Continue Game");
                _panels.ContinueCountDown.Play ("Pop_Out");
                gs.GameStarted = true;
                _continue = false;
            }
        }

        //Continue Panel
        if (_continuePanel.Active) {
            _continuePanel.CurrentContinueShowTime -= Time.deltaTime * 1;
            _continuePanel.fillImage.fillAmount = _continuePanel.CurrentContinueShowTime / _continuePanel.ContinueShowTime;

            if (_continuePanel.CurrentContinueShowTime <= _continuePanel.ContinueShowTime - 3) {
                _continuePanel.SkipButton.SetActive (true);
            }

            if (_continuePanel.CurrentContinueShowTime <= 0) {
                _panels.ContinuePanel.Play ("Pop_Out");
                _panels.DeathPanel.GetComponent<Animator> ().Play ("Pop_In");
                _continuePanel.Active = false;
            }
        }

        //Start game
        if (!levelStarted && controller_Input.mouseButton.value) {
            StartGame ();
            _texts.holdText.Play ("Pop_Out");
            levelStarted = true;
        }

    }

    void SpawnPlayer () {
        GameObject _player = Instantiate (gs._totalCharacters[gs.SelectedCharacterTag], Vector3.zero, Quaternion.identity);
        _player.transform.SetParent (_cameraContainer);
        player = _player.transform;
    }

    public void StartGame () {
        gs.GameStarted = true;
        gs.Paused = false;
        _panels.Controller.SetActive (true);

        Player_Powerup.instace.DisableAll ();

        //Disable panels
        if (_panels.DeathPanel.GetComponent<CanvasGroup> ().alpha == 1)
            _panels.DeathPanel.GetComponent<Animator> ().Play ("Pop_Out");

        if (_panels.FinishPanel.GetComponent<CanvasGroup> ().alpha == 1)
            _panels.FinishPanel.GetComponent<Animator> ().Play ("Pop_Out");


        player.gameObject.SetActive (false);

        //Setting up player
        Player_Controll pc = player.GetComponent<Player_Controll> ();
        pc.useFinger = gs.useFinger;
        pc.speed = gs.sensitivity;
        pc.axisVector = Vector3.zero;

        player.localPosition = _playerStartingPos;
        _cameraContainer.position = _camStartingPos.position;

        player.gameObject.SetActive (true);

        CollectedCoins = 0;

        dead = false;
    }

    public void Retry () {
        gs.GameStarted = true;
        gs.Paused = false;
        _panels.Controller.SetActive (true);

        Player_Powerup.instace.DisableAll ();

        //Disable panels
        if (_panels.DeathPanel.GetComponent<CanvasGroup> ().alpha == 1)
            _panels.DeathPanel.GetComponent<Animator> ().Play ("Pop_Out");

        if (_panels.FinishPanel.GetComponent<CanvasGroup> ().alpha == 1)
            _panels.FinishPanel.GetComponent<Animator> ().Play ("Pop_Out");

        player.gameObject.SetActive (false);

        //Setting up player
        Player_Controll pc = player.GetComponent<Player_Controll> ();
        pc.useFinger = gs.useFinger;
        pc.speed = gs.sensitivity;
        pc.axisVector = Vector3.zero;

        player.localPosition = _playerStartingPos;
        _cameraContainer.position = _camStartingPos.position;

        player.gameObject.SetActive (true);

        foreach (RandomPowerup rp in randomPowerups) {
            rp.SetPowerUp ();
        }

        dead = false;
    }

    public void Continue () {
        if (!player) {
            player = GameObject.FindGameObjectWithTag ("Player").transform;
        }

        //Reset mousePos and axisVector
        player.GetComponent<Player_Controll> ().mousePos = Vector2.zero;
        player.GetComponent<Player_Controll> ().axisVector = Vector2.zero;

        _camera.transform.position = new Vector3 (_camera.position.x, checkpoint.position.y, _camera.position.z);
        player.position = checkpoint.position;

        _panels.DeathPanel.GetComponent<Animator> ().Play ("Pop_Out");
        _panels.Controller.SetActive (true);
        _panels.ContinueCountDown.Play ("Pop_In");

        CurrentContinueTime = continueTime;

        _continuePanel.Active = false;
        continued = true;
        gs.Paused = false;
        _continue = true;
        dead = false;
    }

    public void SkipContinue () {
        _panels.ContinuePanel.Play ("Pop_Out");
        _continuePanel.Active = false;
    }

    public void FinishLevel () {
        gs.GameStarted = false;

        //Show finish panel
        _panels.Controller.SetActive (false);
        _panels.FinishPanel.GetComponent<Animator> ().Play ("Pop_In");

        //Conins data
        _texts.collectedCoins.text = CollectedCoins + "<sprite=0>";
        _texts.totalCoins.text = (CurrentCoins + CollectedCoins) + "<sprite=0>";

        //Add and save coins
        gs.Coins += CollectedCoins;
        ZPlayerPrefs.SetInt ("Coins", gs.Coins);
        Debug.Log ("Saved coins " + gs.Coins);


        char[] lvlData = gs.levelsData.ToCharArray ();
        lvlData[gs.SelectedLevel + (gs.LevelsPerStage * gs.SelectedStage)] = '1';
        gs.levelsData = lvlData.ArrayToString ();

        if (gs.SelectedLevel != lvlUnlock.Length)
            lvlUnlock[gs.SelectedLevel + (gs.LevelsPerStage * gs.SelectedStage) + 1] = '1';

        gs.levelsUnlocked = lvlUnlock.ArrayToString ();

        //Save data
        PlayerPrefs.SetString ("LevelsU", gs.levelsUnlocked);
        PlayerPrefs.SetString ("Levels", gs.levelsData);

    }

    public void Death () {
        gs.GameStarted = false;
        _panels.DeathPanel.GetComponent<Animator> ().Play ("Pop_In");

        _panels.Controller.SetActive (false);
        dead = true;
    }

    public void LoadLevel (string name) {
        gs.levelName = name;
        SceneManager.LoadScene ("LoadingScreen");
    }

    public void Pause (bool p) {
        gs.Paused = p;
    }

    public void GameStarted (bool g) {
        gs.GameStarted = g;
    }

    public void NextLevel () {
        gs.SelectedLevel++;

        //Check if it's last level from selected stage
        if (gs.SelectedLevel > 1) {
            if (gs.SelectedLevel % gs.LevelsPerStage == 0) {
                gs.SelectedStage++;
                gs.SelectedLevel = 0;
            }
        }

        //If next level value is bigger than total levels switch to endless scene
        if (gs.SelectedLevel >= gs.LevelsNr) {
            gs.levelName = "Endless";
            SceneManager.LoadScene ("LoadingScreen");
        } else {
            gs.levelName = "Stage" + gs.SelectedStage;
            SceneManager.LoadScene ("LoadingScreen");
        }
        gs.GameStarted = false;
        gs.Paused = false;
    }
}