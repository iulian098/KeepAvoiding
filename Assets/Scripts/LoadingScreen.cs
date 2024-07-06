using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {
    public GameSettings gs;
    public Image progressBar;
    public bool customScene;
    public string sceneName;
    string _scene;

    public bool sceneLoaded;
    // Start is called before the first frame update

    void Awake () {
         if (customScene) {
            _scene = sceneName;
        } else {
            _scene = gs.levelName;
        }
        SceneManager.sceneLoaded += SceneFullyLoaded;
    }

    void SceneFullyLoaded (Scene scene, LoadSceneMode mode) {
        if (!sceneLoaded) {
            sceneLoaded = true;
            SceneManager.sceneLoaded -= SceneFullyLoaded;
            StartCoroutine (LoadAsyncOperation ());
        }
        Debug.Log ("<color=orange>Scene fully loaded</color>");
    }

    IEnumerator LoadAsyncOperation () {
        AsyncOperation level = SceneManager.LoadSceneAsync (_scene);

        while (!level.isDone) {
            float progress = Mathf.Clamp01 (level.progress / 0.9f);
            progressBar.fillAmount = progress;
            //Debug.Log("Progress: " + progress);
            yield return null;
        }

        yield return new WaitForEndOfFrame ();
    }
}