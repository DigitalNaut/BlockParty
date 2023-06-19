using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine;

[Icon("Assets/Icons/Scripts/UI/Overlay.png")]
public class VictoryOverlay : MonoBehaviour
{
  VisualElement root;
  Button continueButton;
  Button mainMenuButton;
  Button exitGameButton;

  void Awake()
  {
    root = GetComponent<UIDocument>().rootVisualElement;
    continueButton = root.Q<Button>("ContinueButton");
    mainMenuButton = root.Q<Button>("MainMenuButton");
    exitGameButton = root.Q<Button>("QuitButton");

    continueButton.clicked += LevelManager.LoadNextLevel;
    mainMenuButton.clicked += LevelManager.LoadMainMenu;
    exitGameButton.clicked += Application.Quit;
  }
}
