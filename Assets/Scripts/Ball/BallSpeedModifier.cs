using System;
using UnityEngine;

[Icon("Assets/Icons/Scripts/Spedometer.png")]
public class BallSpeedModifier : MonoBehaviour
{
  [Header("Settings")]
  [Tooltip("The operation to use when boosting the ball when comparing the current or the new velocity.")]
  public BallProjectile.BoosterMode BounceMode;
  [Range(0.0f, 100.0f)] public float Boost = 10.0f;
  [Range(0.0f, 2.0f)] public float Modifier = 1.0f;

  delegate void ProjectileModifier(Collider collider);
  ProjectileModifier modifyProjectileSpeed;

  void OnEnable() => modifyProjectileSpeed = ModifyProjectileSpeed;
  void OnDisable() => modifyProjectileSpeed = null;

  void ModifyProjectileSpeed(Collider collider)
  {
    if (collider.TryGetComponent(out BallProjectile launchableObj))
      launchableObj.Modify(Boost, BounceMode, Modifier);
  }

  void OnCollisionEnter(Collision collision) => modifyProjectileSpeed?.Invoke(collision.collider);

  void OnTriggerEnter(Collider collider) => modifyProjectileSpeed?.Invoke(collider);
}
