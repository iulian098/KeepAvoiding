using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PhpDatabase : MonoBehaviour {

    public static PhpDatabase instance;

    public GameSettings gs;

    bool error;
    bool update_success, add_success, get_success;

    public Animator _waitingPanel;

    public Animator Error_Panel;

    void Awake () {
        instance = this;
    }

    public void GetData () {
        if (Application.internetReachability != NetworkReachability.NotReachable) {
            _waitingPanel.Play ("Pop_In");
            StartCoroutine (getFromDB ());
        }else{
            GameManager.instance.NoInternetConnection();
        }
    }

    public void Upload () {
        if (Application.internetReachability != NetworkReachability.NotReachable) {
            _waitingPanel.Play ("Pop_In");
            StartCoroutine (uploadToDB ());

        }else{
            GameManager.instance.NoInternetConnection();
        }
    }

    IEnumerator getFromDB () {

        WWWForm form = new WWWForm ();

        form.AddField ("playerName", gs.playerName);

        //WWW request = new WWW ("http://www.studio-ci.com/get.php", form);

        using (UnityWebRequest web_request = UnityWebRequest.Post ("http://www.studio-ci.com/get.php", form)) {
            yield return web_request.SendWebRequest ();

            if (!web_request.isNetworkError || !web_request.isHttpError) {

                if (web_request.downloadHandler.text != "0") {

                    Debug.Log (web_request.downloadHandler.text);
                    string _dataText = web_request.downloadHandler.text;

                    string[] _data = _dataText.Split ('\t');

                    gs.coins1 = int.Parse (_data[0]);
                    gs.coins2 = int.Parse (_data[1]);
                    gs.coins3 = int.Parse (_data[2]);

                    for (int i = 0; i < gs.coins1; i++) {
                        GameManager.instance.Coins += 1000;
                    }

                    for (int j = 0; j < gs.coins2; j++) {
                        GameManager.instance.Coins += 2500;
                    }

                    for (int k = 0; k < gs.coins3; k++) {
                        GameManager.instance.Coins += 5000;
                    }

                    gs.restored = true;
                    ZPlayerPrefs.SetInt ("Restored", 1);

                    ZPlayerPrefs.SetInt ("Coins", gs.Coins);

                } else {
                    gs.restored = true;
                }

            } else if (web_request.isNetworkError) {
                GameManager.instance.NoInternetConnection ();
                Debug.Log ("Check internet connection");
            } else {
                Debug.Log ("Error");
                Error_Panel.Play("Pop_In");
            }
        }

        _waitingPanel.Play ("Pop_Out");

    }

    IEnumerator uploadToDB () {

        error = false;

        WWWForm form = new WWWForm ();

        form.AddField ("playerName", gs.playerName);
        form.AddField ("coins1", gs.coins1);
        form.AddField ("coins2", gs.coins2);
        form.AddField ("coins3", gs.coins3);

        //WWW www = new WWW ("http://www.studio-ci.com/upload.php", form);

        using (UnityWebRequest web_request = UnityWebRequest.Post ("http://www.studio-ci.com/upload.php", form)) {
            yield return web_request.SendWebRequest ();

            if (!web_request.isHttpError || !web_request.isNetworkError) {
                if(web_request.downloadHandler.text == "11"){
                    Debug.Log("Update Successfully");
                }else if(web_request.downloadHandler.text == "10"){
                    Debug.Log("Added Successfully");
                }else{
                    Debug.Log("Something went wrong");
                    Error_Panel.Play("Pop_In");
                }
            }else if(web_request.isNetworkError){
                Debug.Log("Network error.");
            }else if(web_request.isHttpError){
                Debug.Log("Upload failed.");
            }else{
                Error_Panel.Play("Pop_In");
            }
        }

        _waitingPanel.Play ("Pop_Out");
    }
}