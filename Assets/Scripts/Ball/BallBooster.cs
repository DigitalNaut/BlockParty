using UnityEngine;

public class BallBooster : MonoBehaviour
{
  public BallProjectile.BoosterMode BounceMode;
  [Range(0.0f, 1000.0f)]
  public float Boost = 10.0f;
  [Range(0.0f, 1.0f)]
  public float Modifier = 1.0f;

  private void OnCollisionExit(Collision collision)
  {
    BallProjectile launchableObj = collision.gameObject.GetComponent<BallProjectile>();

    if (launchableObj)
      launchableObj.Boost(Boost, BounceMode, Modifier);
  }
}
