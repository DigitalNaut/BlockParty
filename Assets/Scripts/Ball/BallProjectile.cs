using System;
using NaughtyAttributes;
using UnityEngine;

public class BallProjectile : MonoBehaviour
{
  [Header("Properties")]
  [SerializeField][MinMaxSlider(0.0f, 100.0f)] Vector2 speedLimits = new Vector2(1f, 20f);

  new Rigidbody rigidbody;
  AudioSource audioSource;
  Action speedModulator;

  public Action<BallProjectile> OnDestroyCallback;

  float Speed
  {
    get { return rigidbody.velocity.magnitude; }
    set { rigidbody.velocity = rigidbody.velocity.normalized * Math.Clamp(value, speedLimits.x, speedLimits.y); }
  }

  public enum BoosterMode
  {
    Absolute,
    KeepSlowest,
    KeepFastest,
  }

  public void SetKinematic(bool isKinematic) => rigidbody.isKinematic = isKinematic;

  void Awake()
  {
    rigidbody = GetComponent<Rigidbody>();
    audioSource = GetComponent<AudioSource>();

    SetKinematic(false);
  }

  void OnCollisionExit(Collision collision)
  {
    Breakable breakableObj = collision.gameObject.GetComponent<Breakable>();

    if (breakableObj != null && audioSource != null && audioSource.enabled)
      audioSource.Play();
  }

  void OnDisable() => StopAllCoroutines();

  public void DestroyProjectile()
  {
    StopAllCoroutines();

    // Call back if it's the main ball registered by the BallManager
    if (OnDestroyCallback != null)
    {
      rigidbody.velocity = Vector3.zero; // Bugfix: stops VFX from inheriting death velocity on respawn
      OnDestroyCallback.Invoke(this);
    }
    // Otherwise, disable
    else Destroy(gameObject);
  }

  public void Launch(Vector3 launchVelocity, BoosterMode mode = BoosterMode.Absolute, float modifier = 1.0f)
  {
    rigidbody.velocity = launchVelocity.normalized;
    Modify(launchVelocity.magnitude, mode, modifier);
  }

  public void Modify(float launchVelocity, BoosterMode mode = BoosterMode.Absolute, float modifier = 1.0f)
  {
    float newVelocity = Speed * modifier + launchVelocity;

    Speed = mode switch
    {
      BoosterMode.KeepSlowest => Math.Min(Speed, newVelocity),
      BoosterMode.KeepFastest => Math.Max(Speed, newVelocity),
      _ => newVelocity,
    };
  }

  internal void SetDirection(Vector3 newDirection) => rigidbody.velocity = newDirection.normalized * Speed;
}
