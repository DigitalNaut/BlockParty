using System;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
  public Action<Collider> OnCollisionDetected;

  void OnCollisionEnter(Collision collision) => OnCollisionDetected?.Invoke(collision.collider);
}
