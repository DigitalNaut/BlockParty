using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Collider))]
public class LucidBallSpawner : MonoBehaviour
{
  [Required][SerializeField] LucidBallManager lucidBallManager;

  void OnCollisionEnter(Collision collision) => lucidBallManager.SpawnLucidBall(transform, collision);

  void OnDrawGizmosSelected() => Debug.DrawLine(transform.position, lucidBallManager.gameObject.transform.position, Color.cyan);
}
