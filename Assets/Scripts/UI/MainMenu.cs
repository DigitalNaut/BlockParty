using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;
using System.Linq;

[Icon("Assets/Icons/Scripts/UI/Menu.png")]
public class MainMenu : MonoBehaviour
{
  VisualElement root;
  VisualElement menuButtons;

  Button playButton;
  Button loadGame;
  Button optionsButton;
  Button quitButton;

  [Dropdown("GetSceneNames")]
  [SerializeField] string gameSceneName = "Game";

#if UNITY_EDITOR
  string[] GetSceneNames => EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
#endif

  void OnPlayButtonClicked() => SceneManager.LoadScene(gameSceneName);

  void OnLoadGameButtonClicked()
  {
    Debug.Log("Load Game");
  }

  void OnOptionsButtonClicked()
  {
    Debug.Log("Options");
  }

  void OnQuitButtonClicked() => Application.Quit();

  void Awake()
  {
    root = GetComponent<UIDocument>().rootVisualElement;
    menuButtons = root.Q<VisualElement>("MenuButtons");
    playButton = menuButtons.Q<Button>("NewGameButton");
    loadGame = menuButtons.Q<Button>("LoadGameButton");
    optionsButton = menuButtons.Q<Button>("OptionsButton");
    quitButton = menuButtons.Q<Button>("ExitGameButton");

    playButton.clicked += OnPlayButtonClicked;
    loadGame.clicked += OnLoadGameButtonClicked;
    optionsButton.clicked += OnOptionsButtonClicked;
    quitButton.clicked += OnQuitButtonClicked;
  }
}
