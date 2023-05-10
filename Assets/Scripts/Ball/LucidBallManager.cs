using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;

/// <summary>
/// This class is responsible for spawning the lucid ball and playing the VFX animations.
/// </summary>

// Set script icon
[Icon("Assets/Textures/Script Icons/PurpleBallCog.png")]
[AddComponentMenu("Scripts/Ball/LucidBallManager")]
[RequireComponent(typeof(BallManager))]
public class LucidBallManager : MonoBehaviour
{
  [Header("Prefabs")]
  [Required][SerializeField] BallProjectile LucidBallPrefab;

  [Header("Dependencies")]
  [Required][SerializeField] AudioSource AudioSource;

  [Header("VFX")]
  [Required][SerializeField][Expandable] VFXEvent implodeVFX;
  [Required][SerializeField][Expandable] VFXEvent explodeVFX;
  [Required][SerializeField][Expandable] VFXEvent foulBallVFX;
  [Required][SerializeField][Expandable] VFXEvent victoryBurstVFX;
  [Required][SerializeField][Expandable] VFXEvent recallVFX;

  [Header("SFX")]
  [Required][SerializeField] AudioClip BallUpgradeSound;

  [Foldout("Events")] public UnityEvent OnAllBallsDestroyed;

  public ItemsHolster<BallProjectile> BallsHolster { get; private set; }

  void Awake()
  {
    BallsHolster = new ItemsHolster<BallProjectile>();
    BallsHolster.SetMaxLimit(5);
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
    BallsHolster.Remove(ball);
    StartCoroutine(foulBallVFX.Play(ball.transform.position));

    if (BallsHolster.Count == 0)
      OnAllBallsDestroyed?.Invoke();
  }

  public void SpawnLucidBall(Transform spawner, Collision collision)
  {
    BallProjectile newLucidBall;

    if (BallsHolster.CanAddItems)
      newLucidBall = NewLucidBall(false);
    else
    {
      newLucidBall = BallsHolster.GetOldest(false);
      StartCoroutine(recallVFX.Play(newLucidBall.transform.position));
    }


    BallsHolster.Add(newLucidBall);

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
    BallsHolster.Remove(ball);
    Destroy(ball.gameObject);
  }

  void DestroyAllActiveBalls() => BallsHolster.Clear((ball) => ball.gameObject.activeSelf);

  public void VictoryBurst()
  {
    BallsHolster.ForEach(ball =>
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
