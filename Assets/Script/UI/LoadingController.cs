using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : IMenuStack
{

    public Slider slider;

    private void Start()
    {
        slider.value = 0;
        StartCoroutine(LoadGame());
    }

    public IEnumerator LoadGame()
    {
        bool rs = true;

        // Khởi tạo Kết nối
        slider.value = 0.1f;
        Debug.Log("Initializing Connection...");
        OnlineManager.Instance.ConnectToServer();
        yield return new WaitUntil(() => OnlineManager.Instance.IsConnected());
        slider.value = 0.2f;

        // Login
        Debug.Log("Logging in to Server...");
        OnlineManager.Instance.LoginToServer();
        yield return new WaitUntil(() => OnlineManager.Instance.IsLoggedIn());
        slider.value = 0.3f;

        // build player profile
        Debug.Log("Player Profile...");
        PlayerProfile.Instance.RequestPlayerProfile();
        yield return new WaitUntil(() => PlayerProfile.Instance.IsInitialize());
        slider.value = 0.4f;

        //load Clan info
        ClanManager.Instance.Initlize();
        if (PlayerProfile.Instance.getClanID() > 0)
        {
            ClanManager.Instance.GetClanInfo(PlayerProfile.Instance.getClanID());
            yield return new WaitUntil(() => ClanManager.Instance.IsClanInfoReceived());
        }
        slider.value = 0.5f;

        // load local data  
        Debug.Log("Loading Local Data...");
        DataManager.Instance.Initlize();
        yield return new WaitUntil(() => DataManager.Instance.IsInitialized());
        slider.value = 0.8f;

        // chuyển scene
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(1);
        while (!sceneLoading.isDone)
        {
            float progress = Mathf.Clamp01(sceneLoading.progress / 1.0f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");
            yield return null;
        }
        slider.value = 1;

    }

}
