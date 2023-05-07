using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
  [Header("UI")]
  [Required][SerializeField] TextMeshProUGUI BallQueueText;
  [Required][SerializeField] TextMeshProUGUI bricksCountTextbox;

  [Header("Dependencies")]
  [Required][SerializeField] BrickManager brickManager;
  [Required][SerializeField] BallManager ballManager;

  [HideInInspector] public AudioSource audioSource;
  PlayerInput playerInput;

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
    brickManager.OnAllBricksDestroyed.AddListener(() => Debug.Log("All bricks destroyed"));

    ballManager.OnBallDispensed.AddListener(UpdateBallsQueueCountTextbox);
    ballManager.OnAllBallsDestroyed.AddListener(() => Debug.Log("All balls destroyed"));
  }

  void UpdateBricksCountTextbox(int count) => bricksCountTextbox.text = count.ToString();

  void UpdateBallsQueueCountTextbox(int count)
  {
    // Set the text to the number of balls in the queue using emojis
    if (count > 0)
      BallQueueText.text = string.Concat(Enumerable.Repeat("\uf111 ", count));
    else
      BallQueueText.text = "None";
  }
}
