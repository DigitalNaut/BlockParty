using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine;

[Icon("Assets/Icons/Scripts/UI/Overlay.png")]
public class VictoryOverlay : MonoBehaviour
{
  VisualElement root;
  Button continueButton;
  Button exitGameButton;

  void Continue() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

  void ExitGame() => Application.Quit();

  void Awake()
  {
    root = GetComponent<UIDocument>().rootVisualElement;
    continueButton = root.Q<Button>("ContinueButton");
    exitGameButton = root.Q<Button>("QuitButton");

    continueButton.clicked += Continue;
    exitGameButton.clicked += ExitGame;
  }
}
