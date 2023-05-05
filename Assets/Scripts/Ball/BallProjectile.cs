using System;
using UnityEngine;
using UnityEngine.VFX;

public class BallProjectile : MonoBehaviour
{
  [Header("Properties")]
  public float MinSpeed = 1.0f;
  public float MaxSpeed = Mathf.Infinity;

  [Range(0f, 5f)]
  public float SpeedModifier = 0.1f;
  [Header("References")]
  public VisualEffect DestroyEffect;
  public VisualEffect BallSwapEffect;

  new Rigidbody rigidbody;
  AudioSource audioSource;
  Action speedModulator;

  public Action<BallProjectile> OnDestroyCallback;

  float Speed
  {
    get { return rigidbody.velocity.magnitude; }
    set { rigidbody.velocity = rigidbody.velocity.normalized * value; }
  }

  public enum BoosterMode
  {
    Absolute,
    KeepSlowest,
    KeepFastest,
  }

  public void SetKinematic(bool isKinematic)
  {
    rigidbody.isKinematic = isKinematic;
    speedModulator = isKinematic ? null : ManageSpeed;
  }

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

  void ManageSpeed()
  {
    if (rigidbody.isKinematic) return;

    bool isTooSlow = Speed < MinSpeed;
    bool isSpeeding = Speed > MaxSpeed;
    bool isOutsideOfBounds = isTooSlow || isSpeeding;

    if (isOutsideOfBounds)
    {
      // Find which bound is breaking and assign a target speed
      float resolvedPeed = isSpeeding ? MaxSpeed : isTooSlow ? MinSpeed : Speed;

      Speed = Mathf.Lerp(Speed, resolvedPeed, SpeedModifier * Time.deltaTime);
    }
  }

  void FixedUpdate() => speedModulator?.Invoke();

  void OnDisable() => StopAllCoroutines();

  public void DestroyProjectile()
  {
    StopAllCoroutines();

    if (DestroyEffect)
    {
      DestroyEffect.transform.parent = null;
      DestroyEffect.transform.position = transform.position;
      DestroyEffect.SendEvent("PlayBurst");
    }

    // Call back if it's the main ball registered by the BallManager
    if (OnDestroyCallback != null)
    {
      rigidbody.velocity = Vector3.zero; // Bugfix: stops VFX from inheriting death velocity on respawn
      OnDestroyCallback.Invoke(this);
    }
    // Otherwise, disable
    else Destroy(gameObject);
  }

  public void PlaySwapEffect()
  {
    if (BallSwapEffect)
    {
      BallSwapEffect.transform.parent = null;
      BallSwapEffect.transform.position = transform.position;
      BallSwapEffect.SendEvent("PlayBurst");
    }
  }

  public void Launch(Vector3 launchVelocity, BoosterMode mode = BoosterMode.Absolute, float modifier = 1.0f)
  {
    if (rigidbody == null)
      return;

    rigidbody.velocity = launchVelocity.normalized;
    Boost(launchVelocity.magnitude, mode, modifier);
  }

  public void Boost(float launchVelocity, BoosterMode mode = BoosterMode.Absolute, float modifier = 1.0f)
  {
    float newVelocity = Speed * modifier + launchVelocity;

    Speed = mode switch
    {
      BoosterMode.KeepSlowest => Math.Min(Speed, newVelocity),
      BoosterMode.KeepFastest => Math.Max(Speed, newVelocity),
      _ => newVelocity,
    };
  }

  public void Bump(float boostSpeed, float modifier = 1.0f)
  {
    Vector3 rbVel = rigidbody.velocity;

    float newVelocity = rbVel.magnitude * modifier + boostSpeed;

    Speed = newVelocity;
  }

  internal void SetDirection(Vector3 newDirection) => rigidbody.velocity = newDirection.normalized * Speed;
}
