using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Collider))]
public class LucidBallSpawner : MonoBehaviour
{
  [SerializeField] LucidBallManager lucidBallManager;

  void Awake() => Debug.Assert(lucidBallManager != null, "Lucid ball manager not set.", transform);

  void OnCollisionEnter(Collision collision) => lucidBallManager.SpawnLucidBall(transform, collision);

  void OnDrawGizmosSelected() => Debug.DrawLine(transform.position, lucidBallManager.gameObject.transform.position, Color.cyan);
}
