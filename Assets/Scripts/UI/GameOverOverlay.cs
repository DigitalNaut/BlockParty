using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine;

[Icon("Assets/Icons/Scripts/UI/Overlay.png")]
public class GameOverOverlay : MonoBehaviour
{
  VisualElement root;
  Button tryAgainButton;
  Button mainMenuButton;
  Button exitGameButton;

  void Awake()
  {
    root = GetComponent<UIDocument>().rootVisualElement;
    tryAgainButton = root.Q<Button>("TryAgainButton");
    mainMenuButton = root.Q<Button>("MainMenuButton");
    exitGameButton = root.Q<Button>("QuitButton");

    tryAgainButton.clicked += LevelManager.ReloadLevel;
    mainMenuButton.clicked += LevelManager.LoadMainMenu;
    exitGameButton.clicked += Application.Quit;
  }
}
