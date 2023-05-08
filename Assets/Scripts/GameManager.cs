using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
  [Header("UI")]
  [Required][SerializeField] TextMeshProUGUI BallQueueText;
  [Required][SerializeField] TextMeshProUGUI bricksCountTextbox;
  [Required][SerializeField] CanvasGroup HUD;
  [Required][SerializeField] CanvasGroup VictoryScreen;
  // [Required][SerializeField] CanvasGroup GameOverScreen;

  [Header("Dependencies")]
  [Required][SerializeField] BrickWallManager brickManager;
  [Required][SerializeField] BallManager ballManager;
  [Required][SerializeField] LucidBallManager lucidBallManager;
  [Required][SerializeField] PaddleController Paddle;

  [HideInInspector] public AudioSource audioSource;
  PlayerInput playerInput;

  enum GameState { Playing, Victory, GameOver }
  GameState? gameState = null;

  void Awake()
  {
    audioSource = GetComponent<AudioSource>();
    playerInput = GetComponent<PlayerInput>();
  }

  void OnEnable()
  {
    playerInput.onDispenseBall.AddListener(ballManager.DispenseBallHandler);
    playerInput.onVolleyBall.AddListener(ballManager.VolleyBallHandler);
  }

  void OnDisable()
  {
    playerInput.onDispenseBall.RemoveListener(ballManager.DispenseBallHandler);
    playerInput.onVolleyBall.RemoveListener(ballManager.VolleyBallHandler);
  }

  void Start()
  {
    brickManager.OnBricksCountChanged.AddListener(UpdateBricksCountTextbox);
    brickManager.OnAllBricksDestroyed.AddListener(() => SetGameState(GameState.Victory));

    ballManager.OnBallDispensed.AddListener(UpdateBallsQueueCountTextbox);
    ballManager.OnBallQueueEmpty.AddListener(BallsQueueEmptyHandler);

    SetGameState(GameState.Playing);
  }

  void SetGameState(GameState state)
  {
    if (state == gameState)
      return;

    switch (state)
    {
      case GameState.Playing:
        Debug.Log("Game started");
        ToggleControls(true);
        brickManager.Generate(RegisterLucidBallSpawner);
        ballManager.DispenseNextBall();

        HUD.gameObject.SetActive(true);
        VictoryScreen.gameObject.SetActive(false);
        break;
      case GameState.Victory:
        ToggleControls(false);
        VictoryHandler();

        VictoryScreen.gameObject.SetActive(true);
        HUD.gameObject.SetActive(false);
        break;
      case GameState.GameOver:
        ToggleControls(false);
        HUD.gameObject.SetActive(false);
        break;
      default:
        Debug.LogError($"Unknown game state: {state}");
        break;
    }

    gameState = state;
  }

  void ToggleControls(bool enable)
  {
    playerInput.enabled = enable;
    Paddle.enabled = enable;
  }

  void UpdateBricksCountTextbox(int count) => bricksCountTextbox.text = count.ToString();

  void UpdateBallsQueueCountTextbox(int count) => BallQueueText.text = count > 0 ? string.Concat(Enumerable.Repeat("\uF111 ", count)) : "\uF00D";

  void BallsQueueEmptyHandler() => ballManager.OnAllBallsDestroyed.AddListener(LastBallDestroyedHandler);

  void LastBallDestroyedHandler()
  {
    if (brickManager.Breakables.Count == 0) return;

    ballManager.OnAllBallsDestroyed.RemoveListener(() => SetGameState(GameState.GameOver));
  }

  void RegisterLucidBallSpawner(Breakable brick)
  {
    if (brick.TryGetComponent(out LucidBallSpawner lucidBallSpawner))
      brick.OnBreak.AddListener((breakable, collision) => SpawnLucidBallHandler(breakable.transform, collision));
  }

  void SpawnLucidBallHandler(Transform spawnerTransform, Collision collision)
  {
    if (brickManager.Breakables.Count == 0 || collision == null) return;
    lucidBallManager.SpawnLucidBall(spawnerTransform, collision);
  }

  void VictoryHandler()
  {
    ballManager.VictoryBurst();
    lucidBallManager.VictoryBurst();
  }

  public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
