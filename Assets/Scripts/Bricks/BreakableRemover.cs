using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used to remove any breakables that are overlapping the static obstacles.
/// </summary>
public class BreakableRemover : MonoBehaviour
{
  public void ClearOverlappingBreakables(List<Breakable> breakables) => StartCoroutine(ClearOverlap(breakables));

  IEnumerator ClearOverlap(List<Breakable> breakables)
  {
    yield return new WaitForFixedUpdate();

    if (breakables?.Count == 0)
      throw new ArgumentNullException("breakables", "Breakables list is null or empty");

    // Get the child static colliders that overlap the Breakables
    var childColliders = GetComponentsInChildren<Collider>();
    Collider[] collidersOverlapped;

    var removedCount = 0;

    foreach (var collider in childColliders)
    {
#pragma warning disable UNT0028
      collidersOverlapped = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, collider.transform.rotation);
#pragma warning restore UNT0028

      foreach (var overlappedCollider in collidersOverlapped)
      {
        if (overlappedCollider == null || collider == null)
        {
          if (overlappedCollider == null)
            Debug.LogError("Overlapped collider is null");
          else if (collider == null)
            Debug.LogError("Collider is null");
          continue;
        }

        if (Physics.ComputePenetration(collider, collider.transform.position, collider.transform.rotation,
          overlappedCollider, overlappedCollider.transform.position, overlappedCollider.transform.rotation,
          out var _, out var _))
        {
          if (overlappedCollider.TryGetComponent(out Breakable breakable))
          {
            if (overlappedCollider.TryGetComponent(out Explosive explosive))
              explosive.enabled = false;

            if (breakables.Remove(breakable))
            {
              Destroy(overlappedCollider.gameObject);
              removedCount++;
            }
            else Debug.LogWarning($"Failed to remove {overlappedCollider.name} from {collider.name}", transform);
          }
        }
      }

    }

    Debug.Log($"Removed {removedCount} Overlapping Breakables");
  }
}
