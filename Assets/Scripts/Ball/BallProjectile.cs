using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[Icon("Assets/Textures/Icons/ProjectileWithTrail.png")]
public class BallProjectile : MonoBehaviour
{
  [Header("Properties")]
  [SerializeField][MinMaxSlider(0.0f, 100.0f)] Vector2 speedLimits = new Vector2(1f, 20f);

  public Rigidbody Rigidbody { get; private set; }
  AudioSource audioSource;
  Action speedModulator;

  public UnityEvent<BallProjectile> OnDestroyCallback;

  public float Speed
  {
    get { return Rigidbody.velocity.magnitude; }
    private set { Rigidbody.velocity = Rigidbody.velocity.normalized * Math.Clamp(value, speedLimits.x, speedLimits.y); }
  }

  public enum BoosterMode
  {
    Absolute,
    KeepSlowest,
    KeepFastest,
  }

  void Awake()
  {
    Rigidbody = GetComponent<Rigidbody>();
    audioSource = GetComponent<AudioSource>();

    SetKinematic(false);
  }

  void OnCollisionExit(Collision collision)
  {
    Breakable breakableObj = collision.gameObject.GetComponent<Breakable>();

    if (breakableObj != null && audioSource != null && audioSource.enabled)
      audioSource.Play();
  }

  void OnDestroy()
  {
    StopAllCoroutines();
    OnDestroyCallback.RemoveAllListeners();
  }

  public void SetKinematic(bool isKinematic) => Rigidbody.isKinematic = isKinematic;

  public void ToggleFX(bool isActive)
  {
    foreach (var trail in GetComponentsInChildren<TrailRenderer>(isActive))
    {
      trail.Clear();
      trail.gameObject.SetActive(isActive);
    }
    foreach (var particleSystem in GetComponentsInChildren<ParticleSystem>(isActive))
    {
      particleSystem.Clear();
      particleSystem.gameObject.SetActive(isActive);
    }
  }

  public void DestroyProjectile()
  {
    Rigidbody.velocity = Vector3.zero;
    OnDestroyCallback?.Invoke(this);

    Destroy(gameObject);
  }

  public void Launch(Vector3 launchVelocity, BoosterMode mode = BoosterMode.Absolute, float modifier = 1.0f)
  {
    Rigidbody.velocity = launchVelocity.normalized;
    Modify(launchVelocity.magnitude, mode, modifier);
  }

  public void Stop() => Rigidbody.velocity = Vector3.zero;

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

  internal void SetDirection(Vector3 newDirection) => Rigidbody.velocity = newDirection.normalized * Speed;
}
