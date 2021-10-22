using System;
using System.Collections.Generic;
using IntBoolConverter;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.LWRP;

public class MainMenu : MonoBehaviour
{

    public static MainMenu instance;

    [System.Serializable]
    public class UIObjects
    {
        public Toggle UseFinger_Toggle;
        public Toggle cameraEffects_Toggle;
        public Toggle DevMode_Toggle;
        public Toggle Vignette_Toggle;
        public Toggle Bloom_Toggle;
        public Toggle Post_Process;
        public GameObject PP_Object;
        public Toggle UseDefaultSkin_Toggle;
        public GameObject Vignette;

        public Slider Sensitivity_Slider;

        public Animator _exitPanel;

        [Header("Audio")]
        public Slider SFX_Slider;
        public Slider Music_Slider;

        public TMP_Text Coins_text;
        public TMP_Text StoreCoins_text;
        public TMP_Text CollectedCoins_text;
        public Image preview_Image;
        public Image previewContinue_Image;
    }

    [System.Serializable]
    public class Premium
    {
        public bool UseDefaultSkin;

        public Sprite _premiumButton;
        public Sprite _currentButton;
        public List<Image> _buttons;

        public Sprite _premiumPanel;
        public Sprite _currentPanel;
        public List<Image> _panels;

        public Sprite _premiumInnerPanel;
        public Sprite _currentInnerPanel;
        public List<Image> _innerPanels;

        public Sprite _premiumSlider;
        public Sprite _currentSlider;
        public List<Image> _slidersFill;

        public Sprite _premiumSlider_Backgroud;
        public Sprite _currentSlider_Background;
        public List<Image> _slidersBackground;

        public Sprite _premiumCheck;
        public Sprite _currentCheck;
        public List<Image> _checks;

        public Sprite _premiumCheckBackground;
        public Sprite _currentCheckBackground;
        public List<Image> checks_back;

        public GameObject adsBtn;

    }

    [System.Serializable]
    public class ActivePanels
    {
        public bool CharacterChange;
        public bool Settings;
        public bool Upgrades;
    }

    [System.Serializable]
    public class Stages
    {
        public Transform[] stage;
    }

    public AudioMixer mixer;

    public GameSettings gs;
    public GameManager gm;

    public UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset _bloomPipeline;
    public UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset _defaultPipeline;
    public Transform levelsContainer;

    public GameObject _camera;

    public Premium _premium;

    public Stages _stages;
    int stageIndex;

    bool btnsSpawned = false;

    public RectTransform rebuildObj;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {

        mixer = gs.mixer;

        _premium._currentButton = _premium._buttons[0].sprite;
        _premium._currentPanel = _premium._panels[0].sprite;
        _premium._currentInnerPanel = _premium._innerPanels[0].sprite;
        _premium._currentSlider = _premium._slidersFill[0].sprite;
        _premium._currentSlider_Background = _premium._slidersBackground[0].sprite;
        _premium._currentCheck = _premium._checks[0].sprite;
        _premium._currentCheckBackground = _premium.checks_back[0].sprite;

        Debug.Log("OpenGL Version : " + SystemInfo.graphicsDeviceVersion + "\nGraphic Name : " + SystemInfo.graphicsDeviceType);

        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2)
        {
            _objects.Bloom_Toggle.gameObject.SetActive(false);
            SetBloom(false);
        }

    }

    public UIObjects _objects;

    #region Save/Load

    public void Save()
    {

        //Settings
        PlayerPrefs.SetInt("UseFinger", IntBoolConvert.ToInt(gs.useFinger));
        PlayerPrefs.SetFloat("Sensitivity", gs.sensitivity);
        ZPlayerPrefs.SetInt("SelectedCharacter", gs.SelectedCharacter);
        ZPlayerPrefs.SetString("SelectedCharacterTag", gs.SelectedCharacterTag);
        ZPlayerPrefs.SetInt("ads", System.Convert.ToInt32(gs.NoAds));

        //Upgrades
        LoadUpgrades();

        SaveCameraEffects();

        //Premium
        ZPlayerPrefs.SetInt("UseDefaultSkin", Convert.ToInt32(_premium.UseDefaultSkin));

        ZPlayerPrefs.SetInt("HavePremium", System.Convert.ToInt32(gs.havePremium));

        //Coins
        SaveCoins();

        //Debug.Log ("<color=green>Save successfully</color>");
    }

    public void Load()
    {

        gs.useFinger = IntBoolConvert.ToBool(PlayerPrefs.GetInt("UseFinger", 0));
        _objects.UseFinger_Toggle.isOn = gs.useFinger;

        gs.sensitivity = PlayerPrefs.GetFloat("Sensitivity", 9.5f);
        _objects.Sensitivity_Slider.value = gs.sensitivity;

        gs.Coins = ZPlayerPrefs.GetInt("Coins");
        gs.SelectedCharacter = ZPlayerPrefs.GetInt("SelectedCharacter", 0);
        gs.SelectedCharacterTag = ZPlayerPrefs.GetString("SelectedCharacterTag", "Default");

        gs.SFX_Volume = PlayerPrefs.GetFloat("SFXVolume", 0f);
        _objects.SFX_Slider.value = gs.SFX_Volume;

        gs.Music_Volume = PlayerPrefs.GetFloat("MusicVolume", 0f);
        _objects.Music_Slider.value = gs.Music_Volume;

        //Vignette
        gs.Vignette = IntBoolConvert.ToBool(PlayerPrefs.GetInt("Vignette", 1));
        _objects.Vignette_Toggle.isOn = gs.Vignette;
        _objects.Vignette.SetActive(gs.Vignette);

        if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2)
        {
            gs.Bloom = IntBoolConvert.ToBool(PlayerPrefs.GetInt("Bloom", 0));
            _objects.Bloom_Toggle.isOn = gs.Bloom;
            SetBloom(gs.Bloom);
        }

        //Upgrades
        gs.MagnetLevel = PlayerPrefs.GetInt("MagnetLevel", 0);
        gs.SlowdownLevel = PlayerPrefs.GetInt("SlowdownLevel", 0);
        gs.GhostLevel = PlayerPrefs.GetInt("GhostLevel", 0);
        gs.ScoreMultiplierLevel = PlayerPrefs.GetInt("ScoreMultiplier", 0);


        //Ads/Premium
        gs.NoAds = System.Convert.ToBoolean(ZPlayerPrefs.GetInt("ads", 0));
        gs.havePremium = System.Convert.ToBoolean(ZPlayerPrefs.GetInt("HavePremium", 0));

        _premium.UseDefaultSkin = Convert.ToBoolean(ZPlayerPrefs.GetInt("UseDefaultSkin", 0));
        _objects.UseDefaultSkin_Toggle.isOn = _premium.UseDefaultSkin;

        gs.restored = Convert.ToBoolean(ZPlayerPrefs.GetInt("Restored", 0));


        //Others
        SetPreviewSprite(gm.players[gs.SelectedCharacter].icon);

        CoinsText();

        //Debug.Log ("<color=green>Load successfully</color>");
    }

    public void SaveCameraEffects()
    {
        //Sounds
        PlayerPrefs.SetFloat("SFXVolume", gs.SFX_Volume);
        PlayerPrefs.SetFloat("MusicVolume", gs.Music_Volume);

        //Camera Effects
        PlayerPrefs.SetInt("Vignette", IntBoolConvert.ToInt(gs.Vignette));
        PlayerPrefs.SetInt("Bloom", IntBoolConvert.ToInt(gs.Bloom));
    }

    public void SaveCoins()
    {
        ZPlayerPrefs.SetInt("Coins", gs.Coins);
    }

    private void LoadUpgrades()
    {
        PlayerPrefs.SetInt("MagnetLevel", gs.MagnetLevel);
        PlayerPrefs.SetInt("SlowdownLevel", gs.SlowdownLevel);
        PlayerPrefs.SetInt("GhostLevel", gs.GhostLevel);
        PlayerPrefs.SetInt("ScoreMultiplier", gs.ScoreMultiplierLevel);
    }

    #endregion

    #region Premium

    Sprite btnS, panelS, iPannelS, sliderFill, sliderBack, check, checkBack;

    public void CheckPremium()
    {
        if (!GameManager.instance.HavePremium)
        {
            return;
        }
        else
        {

            _premium.adsBtn.SetActive(false);

            if (_premium.UseDefaultSkin)
            {
                btnS = _premium._currentButton;
                panelS = _premium._currentPanel;
                iPannelS = _premium._currentInnerPanel;
                sliderFill = _premium._currentSlider;
                sliderBack = _premium._currentSlider_Background;
                check = _premium._currentCheck;
                checkBack = _premium._currentCheckBackground;

            }
            else
            {
                btnS = _premium._premiumButton;
                panelS = _premium._premiumPanel;
                iPannelS = _premium._premiumInnerPanel;
                sliderFill = _premium._premiumSlider;
                sliderBack = _premium._premiumSlider_Backgroud;
                check = _premium._premiumCheck;
                checkBack = _premium._premiumCheckBackground;

            }

            foreach (Image btn in _premium._buttons)
            {
                btn.sprite = btnS;
            }

            foreach (Image panel in _premium._panels)
            {
                panel.sprite = panelS;
            }

            foreach (Image innerPanel in _premium._innerPanels)
            {
                innerPanel.sprite = iPannelS;
            }

            foreach (Image s in _premium._slidersFill)
            {
                s.sprite = sliderFill;
            }

            foreach (Image sb in _premium._slidersBackground)
                sb.sprite = sliderBack;

            foreach (Image c in _premium._checks)
                c.sprite = check;

            foreach (Image cb in _premium.checks_back)
                cb.sprite = checkBack;
        }
    }

    #endregion

    #region Levels

    public void SpawnLevelsBtns()
    {
        if (!btnsSpawned)
        {
            stageIndex = 0;
            for (int i = 0; i < gs.LevelsNr; i++)
            {

                if (i > 1 && i % 50 == 0)
                {
                    stageIndex++;
                    Debug.Log("StageIndex ++");
                }

                GameObject btn = Instantiate(gs.LevelBtnPrefab, _stages.stage[stageIndex]);

                LevelBtn levelBtn = btn.GetComponent<LevelBtn>();
                btn.GetComponentInChildren<TMP_Text>().text = ((i + 1) - (stageIndex * 50)).ToString();
                levelBtn.Level = i - (stageIndex * 50);
                levelBtn.Stage = stageIndex;
                if (gs.levelsData[i] == '1')
                {
                    levelBtn.completed = true;
                }
                else
                {
                    levelBtn.completed = false;
                }
                if (gs.levelsUnlocked[i] == '1')
                {
                    levelBtn.unlocked = true;
                }
                else
                {
                    levelBtn.unlocked = false;
                }
                levelBtn.CheckCompleted();

            }

            btnsSpawned = true;
        }
    }

    public void SelectLevel(int lvl, int stage)
    {
        gs.SelectedLevel = lvl;
        gs.SelectedStage = stage;
        gs.levelName = "Stage" + stage;
        SceneManager.LoadScene("LoadingScreen");

    }

    #endregion

    public void UseFinger(Toggle t)
    {
        gm.useFinger = t.isOn;
        gs.useFinger = t.isOn;
    }

    public void SetSensitivity(Slider s)
    {
        gs.sensitivity = s.value;
    }

    public void CoinsText()
    {
        _objects.Coins_text.text = gs.Coins.ToString();
        _objects.StoreCoins_text.text = _objects.Coins_text.text;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SetPreviewSprite(Sprite s)
    {
        _objects.preview_Image.sprite = s;
    }

    public void SetContinueSprite(Sprite s)
    {
        _objects.previewContinue_Image.sprite = s;
        gs.selectedSprite = s;
    }

    #region CameraEffects

    public void CameraEffects(Toggle t)
    {
        gs.cameraEffects = t.isOn;
    }
    public void Bloom(Toggle t)
    {
        gs.Bloom = t.isOn;
        SetBloom(gs.Bloom);
        SaveCameraEffects();
    }

    private void SetBloom(bool b)
    {
        if (b)
            GraphicsSettings.renderPipelineAsset = _bloomPipeline;
        else
            GraphicsSettings.renderPipelineAsset = _defaultPipeline;
    }

    public void PostProcessEffects(Toggle t){
        _objects.PP_Object.SetActive(t.isOn);
    }

    public void Vignette(Toggle t)
    {
        gs.Vignette = t.isOn;
        _objects.Vignette.SetActive(t.isOn);
        SaveCameraEffects();
    }

    #endregion 

    public void Back()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _objects._exitPanel.Play("Pop_In");
        }
    }

    public void LoadScene(string lvlName)
    {
        SceneManager.LoadScene("Level");
    }

    public void SetPremiumSkin(Toggle b)
    {
        _premium.UseDefaultSkin = b.isOn;
        CheckPremium();
        Save();
    }

    #region Audio

    public void SetSFXSound(Slider s)
    {
        mixer.SetFloat("SFX", s.value);
        gs.SFX_Volume = s.value;
        SaveCameraEffects();
    }

    public void SetMusicFloat(Slider s)
    {
        mixer.SetFloat("Music", s.value);
        gs.Music_Volume = s.value;
        SaveCameraEffects();
    }

    #endregion

    public void ReloadScene()
    {
        SceneManager.LoadScene("Endless");
    }

    public void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rebuildObj);
    }

}