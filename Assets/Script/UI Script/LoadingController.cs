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
        yield return new WaitForSeconds(1f);

        // Loading data từ server
        Debug.Log("Loading Data from Server...");
        yield return new WaitForSeconds(1f);

        // load local data  
        Debug.Log("Loading Local Data...");
        yield return new WaitForSeconds(1f);

        // build player profile
        Debug.Log("Building Player Profile...");
        PlayerProfile.Instance.Initialize();
        while (!PlayerProfile.Instance.IsInitialize())
        {
            yield return new WaitForSeconds(1f);
        }

        // Khởi tạo UI
        Debug.Log("Initializing UI...");
        yield return new WaitForSeconds(1f);

        // chuyển scene
        SceneManager.LoadScene(1);
        yield return new WaitForSeconds(1f);

    }

}
