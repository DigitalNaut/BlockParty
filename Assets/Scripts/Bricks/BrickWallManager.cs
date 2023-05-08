using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[Icon("Assets/Textures/Icons/BrickWallCogs.png")]
public class BrickWallManager : MonoBehaviour
{
  [Header("Dependencies")]
  [Required][SerializeField] BrickWallGenerator brickWallGenerator;
  [Required][SerializeField] BreakableRemover breakableRemover;

  [Foldout("Events")] public UnityEvent OnAllBricksDestroyed;
  [Foldout("Events")] public UnityEvent<int> OnBricksCountChanged;

  public List<Breakable> Breakables { get; private set; }

  public BrickWallGenerator GetBrickWallGenerator() => brickWallGenerator;

  public void SetBrickWallGenerator(BrickWallGenerator newBrickWallGenerator) => brickWallGenerator = newBrickWallGenerator;

  public List<Breakable> Generate(Action<Breakable> onBreakableCreated = null)
  {
    var newBreakables = brickWallGenerator.BuildBreakablesWall();
    breakableRemover.ClearOverlappingBreakables(newBreakables);
    return SetBreakables(newBreakables, onBreakableCreated);
  }

  public List<Breakable> SetBreakables(List<Breakable> newBreakables, Action<Breakable> onBreakableCreated = null)
  {
    Breakables = newBreakables;

    foreach (var breakable in Breakables)
    {
      breakable.OnBreak.AddListener(BreakableDestroyed);
      onBreakableCreated?.Invoke(breakable);
    }

    OnBricksCountChanged?.Invoke(Breakables.Count);

    return Breakables;
  }

  void BreakableDestroyed(Breakable breakable, Collision collision)
  {
    Breakables.Remove(breakable);

    if (Breakables.Count == 0)
      OnAllBricksDestroyed?.Invoke();

    OnBricksCountChanged?.Invoke(Breakables.Count);
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
