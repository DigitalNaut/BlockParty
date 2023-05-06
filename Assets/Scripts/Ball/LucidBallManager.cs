using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// This class is responsible for spawning the lucid ball and playing the VFX animations.
/// </summary>
[RequireComponent(typeof(BallManager))]
public class LucidBallManager : MonoBehaviour
{
  [Header("Prefabs")]
  [SerializeField] BallProjectile LucidBallPrefab;
  [Header("VFX")]
  [SerializeField][Expandable] VFXEvent implodeVFXEvent;
  [SerializeField][Expandable] VFXEvent explodeVFXEvent;
  [Header("SFX")]
  [SerializeField] AudioClip BallUpgradeSound;

  VFXEvent implodeVFX;
  VFXEvent explodeVFX;

  BallManager ballManager;
  BallsHolster ballsHolster;

  AudioSource AudioSource => ballManager.AudioSource;

  void Awake()
  {
    Debug.Assert(LucidBallPrefab != null, "Lucid ball prefab not set.", transform);
    Debug.Assert(BallUpgradeSound != null, "Ball swap sound not set.", transform);
    Debug.Assert(implodeVFXEvent != null, "Implode VFX event not set.", transform);
    Debug.Assert(explodeVFXEvent != null, "Explode VFX event not set.", transform);

    implodeVFX = Instantiate(implodeVFXEvent);
    explodeVFX = Instantiate(explodeVFXEvent);

    ballManager = GetComponent<BallManager>();
    ballsHolster = gameObject.AddComponent<BallsHolster>();
    ballsHolster.SetBallsMaxLimit(5);
  }

  void OnDisable() => StopAllCoroutines();

  BallProjectile NewLucidBall(bool isActive = false, Transform parent = null)
  {
    var newBall = Instantiate(LucidBallPrefab, parent);
    newBall.gameObject.SetActive(isActive);
    newBall.SetKinematic(false);

    newBall.OnDestroyCallback += ballsHolster.RemoveBall;

    return newBall;
  }

  public void SpawnLucidBall(Transform spawner, Collision collision)
  {
    var newLucidBall = ballsHolster.CanAddBalls ? NewLucidBall(false) : ballsHolster.GetOldestLucidBall(false);
    ballsHolster.AddBall(newLucidBall);

    // Set the lucid ball's position and velocity
    newLucidBall.transform.position = spawner.position;
    if (collision.rigidbody != null)
      newLucidBall.GetComponent<Rigidbody>().velocity = collision.rigidbody.velocity;

    // Show lucid ball after VFX
    IEnumerator Animate()
    {
      yield return PlaySpawnVFX(spawner.position);
      newLucidBall.gameObject.SetActive(true);
    }

    StartCoroutine(Animate());
  }

  IEnumerator PlaySpawnVFX(Vector3 position)
  {
    Debug.DrawLine(transform.position, position, Color.green, 1f);
    yield return StartCoroutine(explodeVFX.PlayEffectAtPosition(position));
    yield return StartCoroutine(implodeVFX.PlayEffectAtPosition(position));
  }
}
