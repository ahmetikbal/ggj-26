using UnityEditor;
using UnityEditor.SceneManagement;

public class Scenes
{
    [MenuItem("SCENES/Elephant Scene %e", priority = 0)]
    private static void OpenElephantScene()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Elephant/elephant_scene.unity", OpenSceneMode.Single);
        }
    }

    [MenuItem("SCENES/Splash Scene", priority = 0)]
    private static void OpenSplashScene()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/0-Project/Scenes/Splash.unity", OpenSceneMode.Single);
        }
    }

    [MenuItem("SCENES/Game Scene %g")]
    private static void OpenGameScene()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/0-Project/Scenes/Game.unity", OpenSceneMode.Single);
        }
    }

    [MenuItem("SCENES/RESTART SCENE %&r")]
    private static void RestartScene()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(EditorSceneManager.GetActiveScene().path, OpenSceneMode.Single);
        }
    }
}