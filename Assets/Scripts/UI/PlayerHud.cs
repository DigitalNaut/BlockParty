using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

[Icon("Assets/Icons/Scripts/UI/Overlay.png")]
public class PlayerHUD : MonoBehaviour
{
  VisualElement root;
  Label bricksCountLabel;
  Label ballQueueLabel;

  [InfoBox("The BrickWallManager will be loaded automatically at runtime")]
  [ReadOnly][SerializeField] BrickWallManager brickManager;
  [Required][SerializeField] BallManager ballManager;

  void UpdateBricksCount(int count) => bricksCountLabel.text = count.ToString();

  void UpdateBallsQueueCount(int count) => ballQueueLabel.text = count > 0 ? string.Concat(Enumerable.Repeat("\uF111 ", count)) : "\uF00D";

  void OnValidate()
  {
    if (brickManager == null) brickManager = FindObjectOfType<BrickWallManager>();
    if (ballManager == null) ballManager = FindObjectOfType<BallManager>();
  }

  void Awake()
  {
    root = GetComponent<UIDocument>().rootVisualElement;
    bricksCountLabel = root.Q<Label>("BricksCount");
    ballQueueLabel = root.Q<Label>("BallQueue");

    brickManager = FindObjectOfType<BrickWallManager>();

    brickManager.OnBricksCountChanged.AddListener(UpdateBricksCount);
    ballManager.OnBallDispensed.AddListener(UpdateBallsQueueCount);
  }

  void OnDestroy()
  {
    brickManager.OnBricksCountChanged.RemoveListener(UpdateBricksCount);
    ballManager.OnBallDispensed.RemoveListener(UpdateBallsQueueCount);
  }
}