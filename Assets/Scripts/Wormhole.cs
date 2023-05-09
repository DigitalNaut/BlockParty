using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Wormhole : MonoBehaviour
{
  [Header("Settings")]
  [Tooltip("Delay between teleporting the ball in seconds")]
  [SerializeField] float TeleportDelay = 0.5f;
  [Tooltip("Cooldown between teleporting the same ball in seconds")]
  [SerializeField] float TeleportCooldown = 1f;

  [Header("Dependencies")]
  [Tooltip("The visual effect to play when teleporting the ball")]
  [Required][SerializeField] VisualEffect teleportVFX;
  [ValidateInput("IsDifferentWormhole", "Cannot be the same wormhole")]
  [Required][SerializeField] Wormhole OtherWormhole;
  bool IsDifferentWormhole() => OtherWormhole != this;

  [ReadOnly][SerializeField] bool canTeleport = true;

  Action<Collider> TeleportStrategy;
  Coroutine teleportCooldownRoutine;
  BallProjectile lastTeleportSubject;

  [Tooltip("Connects the two wormholes together")]
  [DisableIf("IsOtherWormholeNullOrThis")]
  [Button]
  void ReciprocalWormhole()
  {
    if (OtherWormhole != null)
      OtherWormhole.OtherWormhole = this;
  }
  bool IsOtherWormholeNullOrThis => OtherWormhole == null || OtherWormhole.OtherWormhole == this;

  void Start() => SetTeleportStrategy();

  void OnTriggerEnter(Collider other) => TeleportStrategy?.Invoke(other);

  void TransportSubject(BallProjectile subject) => subject.transform.position = OtherWormhole.transform.position;

  void SetTeleportStrategy() => TeleportStrategy = canTeleport ? Teleport : null;

  IEnumerator TeleportRoutine(BallProjectile subject)
  {
    yield return new WaitForFixedUpdate();

    var savedVelocity = subject.Rigidbody.velocity;
    subject.Stop();
    subject.SetKinematic(true);
    subject.ToggleFX(false);
    subject.Rigidbody.position = transform.position;

    yield return new WaitForSeconds(TeleportDelay);
    OtherWormhole.SetOnCooldown();

    if (subject == null)
      yield break;

    TransportSubject(subject);

    subject.SetKinematic(false);
    subject.ToggleFX(true);
    subject.Rigidbody.velocity = savedVelocity;

    if (teleportVFX != null)
      OtherWormhole.teleportVFX.SendEvent("PlayBurst");

    if (lastTeleportSubject == subject)
    {
      if (teleportVFX != null)
        teleportVFX.SendEvent("StopStream");
      lastTeleportSubject = null;
    }
  }

  void Teleport(Collider other)
  {
    if (other.TryGetComponent(out BallProjectile subject) && subject.Rigidbody.isKinematic == false)
    {
      StartCoroutine(TeleportRoutine(subject));

      lastTeleportSubject = subject;
      if (teleportVFX != null)
        teleportVFX.SendEvent("PlayStream");
    }
  }

  IEnumerator TeleportCooldownRoutine()
  {
    yield return new WaitForSeconds(TeleportCooldown);

    canTeleport = true;
    SetTeleportStrategy();
  }

  public void SetOnCooldown()
  {
    if (teleportCooldownRoutine != null)
      StopCoroutine(teleportCooldownRoutine);

    canTeleport = false;
    SetTeleportStrategy();

    teleportCooldownRoutine = StartCoroutine(TeleportCooldownRoutine());
  }

  void OnDrawGizmosSelected()
  {
    if (OtherWormhole == null)
      return;

    Gizmos.color = Color.green;
    var direction = (OtherWormhole.transform.position - transform.position) * 0.9f;
    Gizmos.DrawRay(transform.position, direction);
  }
}
