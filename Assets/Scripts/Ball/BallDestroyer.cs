using UnityEngine;

public class BallDestroyer : MonoBehaviour
{
  private void OnCollisionEnter(Collision collision)
  {
    BallProjectile targetObject = collision.gameObject.GetComponent<BallProjectile>();

    if (targetObject)
      targetObject.Destroy();
  }
}
