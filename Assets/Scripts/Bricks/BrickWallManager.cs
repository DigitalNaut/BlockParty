using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[Icon("Assets/Textures/Icons/BrickWallCogs.png")]
public class BrickManager : MonoBehaviour
{
  [Header("Dependencies")]
  [Required][SerializeField] BrickWallGenerator brickWallGenerator;
  [Required][SerializeField] BreakableRemover breakableRemover;

  [Foldout("Events")] public UnityEvent OnAllBricksDestroyed;
  [Foldout("Events")] public UnityEvent<int> OnBricksCountChanged;

  List<Breakable> breakables;

  public void SetBrickWallGenerator(BrickWallGenerator newBrickWallGenerator) => brickWallGenerator = newBrickWallGenerator;

  void Start() => Generate();

  void Generate()
  {
    var newBreakables = brickWallGenerator.BuildBreakablesWall();
    breakableRemover.ClearOverlappingBreakables(newBreakables);
    SetBreakables(newBreakables);
  }

  public void SetBreakables(List<Breakable> newBreakables)
  {
    breakables = newBreakables;

    foreach (var breakable in breakables)
      breakable.OnBreak.AddListener(BreakableDestroyed);

    OnBricksCountChanged?.Invoke(breakables.Count);
  }

  void BreakableDestroyed(Breakable breakable)
  {
    breakables.Remove(breakable);

    if (breakables.Count == 0)
      OnAllBricksDestroyed?.Invoke();

    OnBricksCountChanged?.Invoke(breakables.Count);
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
