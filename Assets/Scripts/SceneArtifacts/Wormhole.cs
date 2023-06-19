using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

[Icon("Assets/Icons/Scripts/Wormhole.png")]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class Wormhole : MonoBehaviour
{
  [Header("Settings")]
  [Tooltip("Delay between teleporting the ball in seconds")]
  [SerializeField] float TeleportDelay = 0.5f;

  [Header("Dependencies")]
  [Tooltip("The visual effect to play when teleporting the ball")]
  [Required][SerializeField] VisualEffect teleportVFX;
  [ValidateInput("IsDifferentWormhole", "Cannot be the same wormhole")]
  [Required][SerializeField] Wormhole OtherWormhole;
  bool IsDifferentWormhole() => OtherWormhole != this;

  Action<Collider> TeleportStrategy;
  BallProjectile lastInTeleportSubject; // Used to stop the teleport stream VFX when the last ball is teleported
  List<BallProjectile> teleportSubjectsPendingExit = new List<BallProjectile>(); // Used to prevent teleporting the exiting ball back and forth between the two wormholes

#if UNITY_EDITOR
  [Tooltip("Connects the two wormholes together")]
  [DisableIf("IsOtherWormholeNullOrThis")]
  [Button]
  void ReciprocateWormholeReferences()
  {
    if (OtherWormhole == null) throw new NullReferenceException("OtherWormhole is null");
    OtherWormhole.OtherWormhole = this;
    PrefabUtility.RecordPrefabInstancePropertyModifications(OtherWormhole);
  }
  bool IsOtherWormholeNullOrThis => OtherWormhole == null || OtherWormhole.OtherWormhole == this;
#endif

  void Start() => SetTeleportStrategy();

  void OnEnable() => SetTeleportStrategy();

  void OnDisable() => SetTeleportStrategy();

  void OnTriggerEnter(Collider other) => TeleportStrategy?.Invoke(other);

  void OnTriggerExit(Collider other)
  {
    if (other.TryGetComponent(out BallProjectile subject))
      teleportSubjectsPendingExit.Remove(subject);
  }

  void SetTeleportStrategy() => TeleportStrategy = enabled ? Teleport : null;

  void Teleport(Collider other)
  {
    var canTeleportSubject = other.TryGetComponent(out BallProjectile subject)
      && !teleportSubjectsPendingExit.Contains(subject)
      && !subject.Rigidbody.isKinematic;

    if (canTeleportSubject)
    {
      StartCoroutine(TeleportRoutine(subject));

      lastInTeleportSubject = subject;
      if (teleportVFX != null)
        teleportVFX.SendEvent("PlayStream");
    }
  }

  IEnumerator TeleportRoutine(BallProjectile subject)
  {
    yield return new WaitForFixedUpdate();

    var savedVelocity = subject.Rigidbody.velocity;
    subject.Stop();
    subject.SetKinematic(true);
    subject.ToggleFX(false);
    subject.Rigidbody.position = transform.position;

    yield return new WaitForSeconds(TeleportDelay);
    OtherWormhole.teleportSubjectsPendingExit.Add(subject);

    if (subject == null)
      yield break;

    TransportSubject(subject);

    subject.SetKinematic(false);
    subject.ToggleFX(true);
    subject.Rigidbody.velocity = savedVelocity;

    if (teleportVFX != null)
      OtherWormhole.teleportVFX.SendEvent("PlayBurst");

    if (lastInTeleportSubject == subject)
    {
      if (teleportVFX != null)
        teleportVFX.SendEvent("StopStream");
      lastInTeleportSubject = null;
    }
  }

  void TransportSubject(BallProjectile subject) => subject.transform.position = OtherWormhole.transform.position;

  void OnDrawGizmosSelected()
  {
    if (OtherWormhole == null)
      return;

    Gizmos.color = Color.green;
    var direction = (OtherWormhole.transform.position - transform.position) * 0.9f;
    Gizmos.DrawRay(transform.position, direction);
  }
}
