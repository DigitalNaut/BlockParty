using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine;

public class GameOverOverlay : MonoBehaviour
{
  VisualElement root;
  Button tryAgainButton;
  Button exitGameButton;

  void TryAgain() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

  void ExitGame() => Application.Quit();

  void Awake()
  {
    root = GetComponent<UIDocument>().rootVisualElement;
    tryAgainButton = root.Q<Button>("TryAgainButton");
    exitGameButton = root.Q<Button>("QuitButton");

    tryAgainButton.clicked += TryAgain;
    exitGameButton.clicked += ExitGame;
  }
}
