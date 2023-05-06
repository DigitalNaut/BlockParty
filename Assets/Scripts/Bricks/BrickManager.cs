using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
  [SerializeField] TextMeshProUGUI bricksCountTextbox;
  [SerializeField] BrickWallGenerator brickWallGenerator;
  [SerializeField] BreakableRemover breakableRemover;
  List<Breakable> breakables;

  public void SetBrickWallGenerator(BrickWallGenerator newBrickWallGenerator) => brickWallGenerator = newBrickWallGenerator;

  void Awake()
  {
    Debug.Assert(bricksCountTextbox != null, "BricksCountTextbox is null", this);
    Debug.Assert(brickWallGenerator != null, "BrickWallGenerator is null", this);
    Debug.Assert(breakableRemover != null, "BreakableRemover is null", this);
  }

  void Start()
  {
    var newBreakables = brickWallGenerator.BuildBreakablesWall();
    breakableRemover.ClearOverlappingBreakables(newBreakables);
    SetBreakables(newBreakables);
  }

  public void SetBreakables(List<Breakable> newBreakables)
  {
    breakables = newBreakables;

    foreach (var breakable in breakables)
      breakable.OnBreak.AddListener(() => BreakableDestroyed(breakable));

    bricksCountTextbox.text = $"Bricks Left: {breakables.Count}";
  }

  void BreakableDestroyed(Breakable breakable)
  {
    breakables.Remove(breakable);

    bricksCountTextbox.text = $"Bricks Left: {breakables.Count}";
  }

  void OnDrawGizmosSelected()
  {
    if (brickWallGenerator != null)
    {
      Gizmos.color = Color.green;
      Gizmos.DrawLine(transform.position, brickWallGenerator.transform.position);
    }

    if (breakableRemover != null)
    {
      Gizmos.color = Color.red;
      Gizmos.DrawLine(transform.position, breakableRemover.transform.position);
    }
  }
}
