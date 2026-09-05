using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private string currentWorldScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadWorld(string sceneName)
    {
        StartCoroutine(LoadWorldRoutine(sceneName));
    }

    private IEnumerator LoadWorldRoutine(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("SceneLoader: Scene name is empty.");
            yield break;
        }

        Debug.Log("Loading world: " + sceneName);

        // Load the new world asynchronously
        AsyncOperation loadOperation =
            SceneManager.LoadSceneAsync(sceneName);

        if (loadOperation == null)
        {
            Debug.LogError(
                "SceneLoader: Could not load scene: " + sceneName
            );

            yield break;
        }

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        currentWorldScene = sceneName;

        Debug.Log("World loaded: " + sceneName);
    }

    public string GetCurrentWorldScene()
    {
        return currentWorldScene;
    }
}