using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

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
  [SerializeField][Expandable] VFXEvent recallEffect;
  [Header("SFX")]
  [SerializeField] AudioClip BallUpgradeSound;

  BallManager ballManager;
  BallsHolster ballsHolster;

  AudioSource AudioSource => ballManager.AudioSource;

  void Awake()
  {
    Debug.Assert(LucidBallPrefab != null, "Lucid ball prefab not set.", transform);
    Debug.Assert(BallUpgradeSound != null, "Ball swap sound not set.", transform);
    Debug.Assert(implodeVFXEvent != null, "Implode VFX event not set.", transform);
    Debug.Assert(explodeVFXEvent != null, "Explode VFX event not set.", transform);
    Debug.Assert(recallEffect != null, "Recall VFX event not set.", transform);

    ballManager = GetComponent<BallManager>();
    ballsHolster = gameObject.AddComponent<BallsHolster>();
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

  public void SpawnLucidBall(Collision collision)
  {
    var newLucidBall = ballsHolster.CanAddBalls ? NewLucidBall(false, transform) : ballsHolster.GetOldestLucidBall();
    ballsHolster.AddBall(newLucidBall);

    // Set the lucid ball's position and velocity
    newLucidBall.transform.position = collision.transform.position;
    if (collision.rigidbody != null)
      newLucidBall.GetComponent<Rigidbody>().velocity = collision.rigidbody.velocity;

    // Show lucid ball after VFX
    IEnumerator Animate()
    {
      yield return PlaySpawnVFX(collision.transform.position);
      newLucidBall.gameObject.SetActive(true);
    }

    StartCoroutine(Animate());
  }

  IEnumerator PlaySpawnVFX(Vector3 position)
  {
    yield return StartCoroutine(implodeVFXEvent.PlayEffectAtPosition(position, 0.4f));
    yield return StartCoroutine(explodeVFXEvent.PlayEffectAtPosition(position, 0.4f));
  }
}
