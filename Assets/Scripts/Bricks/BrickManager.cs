using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
  [SerializeField]
  TextMeshProUGUI bricksCountTextbox;
  List<Breakable> breakables;

  List<Breakable> GetBreakables() => new List<Breakable>(FindObjectsOfType<Breakable>());

  void Start()
  {
    breakables = GetBreakables();

    foreach (var breakable in breakables)
      breakable.OnBreak.AddListener(() => BreakableDestroyed(breakable));

    bricksCountTextbox.text = $"Bricks Left: {breakables.Count}";
  }

  void BreakableDestroyed(Breakable breakable)
  {
    breakables.Remove(breakable);

    bricksCountTextbox.text = $"Bricks Left: {breakables.Count}";
  }
}
