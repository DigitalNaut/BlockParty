using System.Collections;
using UnityEngine;
using NaughtyAttributes;

/// <summary>
/// This class is responsible for spawning the lucid ball and playing the VFX animations.
/// </summary>

// Set script icon
[AddComponentMenu("Scripts/Ball/LucidBallManager")]
[Icon("Assets/Textures/Icons/PurpleBallCog.png")]
[RequireComponent(typeof(BallManager))]
public class LucidBallManager : MonoBehaviour
{
  [Header("Prefabs")]
  [Required][SerializeField] BallProjectile LucidBallPrefab;

  [Header("Dependencies")]
  [Required][SerializeField] AudioSource AudioSource;

  [Header("VFX")]
  [Required][SerializeField][Expandable] VFXEvent implodeVFXEvent;
  [Required][SerializeField][Expandable] VFXEvent explodeVFXEvent;
  [Required][SerializeField][Expandable] VFXEvent foulBallVFXEvent;

  [Header("SFX")]
  [Required][SerializeField] AudioClip BallUpgradeSound;

  VFXEvent implodeVFX;
  VFXEvent explodeVFX;
  VFXEvent foulBallVFX;
  ItemsHolster<BallProjectile> ballsHolster;

  void Awake()
  {
    implodeVFX = Instantiate(implodeVFXEvent);
    explodeVFX = Instantiate(explodeVFXEvent);
    foulBallVFX = Instantiate(foulBallVFXEvent);

    ballsHolster = new ItemsHolster<BallProjectile>();
    ballsHolster.SetMaxLimit(5);
  }

  void OnDisable() => StopAllCoroutines();

  BallProjectile NewLucidBall(bool isActive = false, Transform parent = null)
  {
    var newBall = Instantiate(LucidBallPrefab, parent);
    newBall.gameObject.SetActive(isActive);
    newBall.SetKinematic(false);
    newBall.OnDestroyCallback += BallDestroyedCallback;

    newBall.OnDestroyCallback += ballsHolster.Remove;

    return newBall;
  }

  public void BallDestroyedCallback(BallProjectile ball)
  {
    StartCoroutine(foulBallVFX.PlayEffectAtPosition(ball.transform.position));
  }

  public void SpawnLucidBall(Transform spawner, Collision collision)
  {
    var newLucidBall = ballsHolster.CanAddItems ? NewLucidBall(false) : ballsHolster.GetOldest(false);
    ballsHolster.Add(newLucidBall);

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
