using System.Collections;
using UnityEngine;

/// <summary>
/// This script is used to remove any breakables that are overlapping the static obstacles.
/// </summary>
public class BreakableRemover : MonoBehaviour
{
  void Start() => StartCoroutine(ClearOverlappingBreakables());

  IEnumerator ClearOverlappingBreakables()
  {
    yield return new WaitForFixedUpdate();

    var childColliders = GetComponentsInChildren<Collider>();
    var hits = new RaycastHit[12];

    foreach (var collider in childColliders)
    {
      Physics.SphereCastNonAlloc(collider.transform.position, collider.transform.localScale.x * 0.5f, Vector3.forward, hits);

      foreach (var hit in hits)
      {
        if (hit.collider && hit.collider.GetComponent<Breakable>())
        {
          var explosive = hit.collider.GetComponent<Explosive>();
          if (explosive) explosive.enabled = false;

          Destroy(hit.collider.gameObject);
        }
      }
    }
  }
}
