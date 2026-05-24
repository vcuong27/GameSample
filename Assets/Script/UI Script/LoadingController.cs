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


        // Login server
        Debug.Log("Logging in to Server...");
        OnlineManager.Instance.LoginToServer();
        yield return new WaitUntil(() => OnlineManager.Instance.IsLoggedIn());

        // load local data  
        Debug.Log("Loading Local Data...");
        yield return new WaitForSeconds(1f);

        // build player profile
        Debug.Log("Building Player Profile...");
        PlayerProfile.Instance.RequestPlayerProfile();
        while (!PlayerProfile.Instance.IsInitialize())
        {
            yield return new WaitForSeconds(1f);
        }

        // Khởi tạo UI
        Debug.Log("Initializing UI...");
        yield return new WaitForSeconds(1f);

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
