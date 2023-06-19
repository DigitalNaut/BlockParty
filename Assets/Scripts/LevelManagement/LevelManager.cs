using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A singleton class that handles additive scene loading for game levels
/// </summary>
public class LevelManager : MonoBehaviour
{
  LevelManagerSettings ControllerSettings;

  public static LevelManager Instance { get; private set; }

  static LevelManager()
  {
    var newLevelManager = new GameObject("LevelManager");
    Instance = newLevelManager.AddComponent<LevelManager>();

    DontDestroyOnLoad(newLevelManager);
  }

  void Awake()
  {
    if (Instance != null && Instance != this)
      Destroy(gameObject);
    else
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }

    if (ControllerSettings == null)
    {
      ControllerSettings = Resources.Load<LevelManagerSettings>("Settings/LevelManagerSettings");
      if (ControllerSettings == null)
        Debug.LogError("LevelManagerSettings not found in Resources folder");
    }
  }

  public static async void LoadLevel(string sceneName)
  {
    var loaderOverlay = FindObjectOfType<LoaderOverlay>(true);
    loaderOverlay.Show(true);

    var scene = SceneManager.LoadSceneAsync(sceneName);
    scene.allowSceneActivation = false;

    while (!scene.isDone)
    {
      loaderOverlay.SetProgress(scene.progress);
      if (scene.progress >= 0.9f)
      {
        scene.allowSceneActivation = true;
        break;
      }
      await Task.Yield();
    }
  }

  public static void LoadMainMenu() => LoadLevel(Instance.ControllerSettings.MainMenuSceneName);

  public static void ReloadLevel() => LoadLevel(SceneManager.GetActiveScene().name);

  public static void LoadNextLevel()
  {
    int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
    if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
      LoadMainMenu();
    else
      LoadLevel(SceneUtility.GetScenePathByBuildIndex(nextSceneIndex));
  }
}
