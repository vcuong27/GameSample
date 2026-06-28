public class GameSceneManager : Singleton<GameSceneManager>
{
    public const string SCENE_MAIN = "CSR_Main";
    public const string SCENE_BATTLE = "SCR_Battle";
    public const string SCENE_ANIMATE = "SCR_Animate";
    public const string SCENE_SAMPLE = "CSR_Sample";

    private static string currentScene = SCENE_MAIN;

    public void LoadScene(string sceneName)
    {
        currentScene = sceneName;
        switch (sceneName)
        {
            case SCENE_MAIN:
            case SCENE_BATTLE:
                UnityEngine.SceneManagement.SceneManager.LoadScene(SCENE_ANIMATE);
                break;
            case SCENE_ANIMATE:
            case SCENE_SAMPLE:
                UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene);
                break;
            default:
                return;
        }
    }

    public string GetCurrentScene()
    {
        return currentScene;
    }


}
