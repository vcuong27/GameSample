#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class ForceStartScene
{
    // ĐƯỜNG DẪN ĐẾN SCENE LOADING CỦA BẠN
    // Thay đổi "Assets/Scenes/LoadingScene.unity" thành đường dẫn thực tế trong Project của bạn
    private static readonly string LoadingScenePath = "Assets/Scenes/SCR_Loading.unity";

    static ForceStartScene()
    {
        // Hàm này tự động chạy mỗi khi Unity load lại script (khi mở dự án hoặc sửa code)
        SetPlayModeStartScene();
    }

    private static void SetPlayModeStartScene()
    {
        SceneAsset loadingScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(LoadingScenePath);

        if (loadingScene != null)
        {
            // Ép Unity Editor luôn luôn chạy scene này đầu tiên khi nhấn Play
            EditorSceneManager.playModeStartScene = loadingScene;
            Debug.Log($"<color=green>[ForceStartScene]</color> Đã cấu hình chạy từ: <b>{LoadingScenePath}</b>");
        }
        else
        {
            Debug.LogWarning($"<color=red>[ForceStartScene]</color> Không tìm thấy scene tại đường dẫn: {LoadingScenePath}. Vui lòng kiểm tra lại đường dẫn!");
        }
    }
}
#endif