using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHUD : MonoBehaviour
{
  VisualElement root;
  Label bricksCountLabel;
  Label ballQueueLabel;

  [Required][SerializeField] BrickWallManager brickManager;
  [Required][SerializeField] BallManager ballManager;

  void UpdateBricksCount(int count) => bricksCountLabel.text = count.ToString();

  void UpdateBallsQueueCount(int count) => ballQueueLabel.text = count > 0 ? string.Concat(Enumerable.Repeat("\uF111 ", count)) : "\uF00D";

  void Awake()
  {
    root = GetComponent<UIDocument>().rootVisualElement;
    bricksCountLabel = root.Q<Label>("BricksCount");
    ballQueueLabel = root.Q<Label>("BallQueue");

    brickManager.OnBricksCountChanged.AddListener(UpdateBricksCount);
    ballManager.OnBallDispensed.AddListener(UpdateBallsQueueCount);
  }

  void OnDestroy()
  {
    brickManager.OnBricksCountChanged.RemoveListener(UpdateBricksCount);
    ballManager.OnBallDispensed.RemoveListener(UpdateBallsQueueCount);
  }
}