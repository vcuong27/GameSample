using Assets.Script.Manager;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimateController : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadGame());
    }

    public IEnumerator LoadGame()
    {

        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(GameSceneManager.GetCurrentScene());
        while (!sceneLoading.isDone)
        {
            float progress = Mathf.Clamp01(sceneLoading.progress / 1.0f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");
            yield return null;
        }
    }
}
