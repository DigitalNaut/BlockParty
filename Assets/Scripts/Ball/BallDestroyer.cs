using UnityEngine;

public class BallDestroyer : MonoBehaviour
{
  private void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.TryGetComponent(out BallProjectile ball))
      ball.DestroyProjectile();
  }

  private void OnTriggerEnter(Collider collider)
  {
    if (collider.gameObject.TryGetComponent(out BallProjectile ball))
      ball.DestroyProjectile();
  }
}
