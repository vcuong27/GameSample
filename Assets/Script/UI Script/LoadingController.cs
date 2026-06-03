using System;
using System.Collections;
using Unity.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : IMenuStack
{

    private void Start()
    {
        StartCoroutine(LoadGame());
    }

    public IEnumerator LoadGame()
    {
        bool rs = true;

        // Khởi tạo Kết nối
        Debug.Log("Initializing Connection...");
        OnlineManager.Instance.ConnectToServer();
        yield return new WaitUntil(() => OnlineManager.Instance.IsConnected());

        // Login
        Debug.Log("Logging in to Server...");
        OnlineManager.Instance.LoginToServer();
        yield return new WaitUntil(() => OnlineManager.Instance.IsLoggedIn());

        // build player profile
        Debug.Log("Player Profile...");
        PlayerProfile.Instance.RequestPlayerProfile();
        yield return new WaitUntil(() => PlayerProfile.Instance.IsInitialize());

        // load local data  
        Debug.Log("Loading Local Data...");
        DataManager.Instance.Initlize();
        yield return new WaitUntil(() => DataManager.Instance.IsInitialized());

        // chuyển scene
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(1);
        while (!sceneLoading.isDone)
        {
            float progress = Mathf.Clamp01(sceneLoading.progress / 1.0f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");
            yield return null;
        }

    }

}
