using System.Collections;
using UnityEngine;
using NaughtyAttributes;

/// <summary>
/// This class is responsible for spawning the lucid ball and playing the VFX animations.
/// </summary>

// Set script icon
[Icon("Assets/Textures/Icons/PurpleBallCog.png")]
[AddComponentMenu("Scripts/Ball/LucidBallManager")]
[RequireComponent(typeof(BallManager))]
public class LucidBallManager : MonoBehaviour
{
  [Header("Prefabs")]
  [Required][SerializeField] BallProjectile LucidBallPrefab;

  [Header("Dependencies")]
  [Required][SerializeField] AudioSource AudioSource;

  [Header("VFX")]
  [Required][SerializeField][Expandable] VFXEvent implodeVFXPrefab;
  [Required][SerializeField][Expandable] VFXEvent explodeVFXPrefab;
  [Required][SerializeField][Expandable] VFXEvent foulBallVFXPrefab;
  [Required][SerializeField][Expandable] VFXEvent victoryBurstVFXPrefab;

  [Header("SFX")]
  [Required][SerializeField] AudioClip BallUpgradeSound;

  VFXEvent implodeVFX;
  VFXEvent explodeVFX;
  VFXEvent foulBallVFX;
  VFXEvent victoryBurstVFX;
  ItemsHolster<BallProjectile> ballsHolster;

  void Awake()
  {
    implodeVFX = Instantiate(implodeVFXPrefab);
    explodeVFX = Instantiate(explodeVFXPrefab);
    foulBallVFX = Instantiate(foulBallVFXPrefab);
    victoryBurstVFX = Instantiate(victoryBurstVFXPrefab);

    ballsHolster = new ItemsHolster<BallProjectile>();
    ballsHolster.SetMaxLimit(5);
  }

  void OnDisable() => StopAllCoroutines();

  BallProjectile NewLucidBall(bool isActive = false, Transform parent = null)
  {
    var newBall = Instantiate(LucidBallPrefab, parent);
    newBall.gameObject.SetActive(isActive);
    newBall.SetKinematic(false);
    newBall.OnDestroyCallback.AddListener(BallDestroyedCallback);

    return newBall;
  }

  public void BallDestroyedCallback(BallProjectile ball)
  {
    ballsHolster.Remove(ball);
    StartCoroutine(foulBallVFX.Play(ball.transform.position));
  }

  public void SpawnLucidBall(Transform spawner, Collision collision)
  {
    var newLucidBall = ballsHolster.CanAddItems ? NewLucidBall(false) : ballsHolster.GetOldest(false);
    ballsHolster.Add(newLucidBall);

    // Set the lucid ball's position and velocity
    newLucidBall.transform.position = spawner.position;
    if (collision?.rigidbody != null)
      newLucidBall.GetComponent<Rigidbody>().velocity = collision.rigidbody.velocity;

    // Show lucid ball after VFX
    IEnumerator Animate()
    {
      yield return PlaySpawnVFXSequence(spawner.position);
      if (newLucidBall)
        newLucidBall.gameObject.SetActive(true);
    }

    StartCoroutine(Animate());
  }

  void DestroyBall(BallProjectile ball)
  {
    ballsHolster.Remove(ball);
    Destroy(ball.gameObject);
  }

  void DestroyAllActiveBalls() => ballsHolster.Clear((ball) => ball.gameObject.activeSelf);

  public void VictoryBurst()
  {
    ballsHolster.ForEach(ball =>
    {
      if (ball.gameObject.activeSelf)
        StartCoroutine(victoryBurstVFX.Play(ball.transform.position));
    });

    DestroyAllActiveBalls();
  }

  IEnumerator PlaySpawnVFXSequence(Vector3 position)
  {
    yield return explodeVFX.Play(position);
    yield return implodeVFX.Play(position);
  }
}
