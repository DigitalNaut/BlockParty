using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BrickManager : MonoBehaviour
{
  [SerializeField]
  TextMeshProUGUI textbox;
  List<Breakable> breakables;
  UnityEvent destroyEvent;

  void Start()
  {
    destroyEvent = new UnityEvent();
    destroyEvent.AddListener(BreakableDestroyed);
    breakables = new List<Breakable>(FindObjectsOfType<Breakable>());

    foreach (var breakable in breakables)
      breakable.OnBreak = () => destroyEvent.Invoke();

    textbox.text = $"Bricks Left: {breakables.Count}";
  }

  void BreakableDestroyed()
  {
    breakables = new List<Breakable>(FindObjectsOfType<Breakable>());

    textbox.text = $"Bricks Left: {breakables.Count}";
  }
}
