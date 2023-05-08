using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using UnityEditor;

[Icon("Assets/Textures/Icons/YellowBallCog.png")]
public class BallManager : MonoBehaviour
{
  [Header("Settings")]
  [SerializeField][Range(0.1f, 30f)] float VolleyThrustSpeed = 15f;
  [SerializeField][Range(0f, 1f)] float VolleyOffsetDistance = 0.5f;

  [Header("Prefabs")]
  [Required][SerializeField] Transform Paddle;
  [SerializeField] BallProjectile[] BallPrefabs;

  [Header("Dependencies")]
  [Required][SerializeField] AudioSource AudioSource;

  [Header("VFX")]
  [Required][SerializeField][Expandable] VFXEvent foulBallVFXPrefab;
  [Required][SerializeField][Expandable] VFXEvent victoryBurstVFXPrefab;

  [Foldout("Events")] public UnityEvent<int> OnBallDispensed;
  [Foldout("Events")] public UnityEvent OnBallVolleyed;
  [Foldout("Events")] public UnityEvent<int> OnBallDestroyed;
  [Foldout("Events")] public UnityEvent OnAllBallsDestroyed;
  [Foldout("Events")] public UnityEvent OnBallQueueEmpty;

  [ShowIf("IsNotPlaying")][ExecuteInEditMode][Button] void EmptyBallPrefabs() => BallPrefabs = new BallProjectile[0];
  bool IsNotPlaying() => !EditorApplication.isPlaying;

  VFXEvent foulBallVFX;
  VFXEvent victoryBurstVFX;
  BallProjectile ballOnPaddle;
  Coroutine dispenseRoutine;
  Queue<BallProjectile> BallQueue = new Queue<BallProjectile>();
  ItemsHolster<BallProjectile> activeBalls;

  Vector3 PositionOnPaddle => Paddle.position + Vector3.up * VolleyOffsetDistance;

  void Awake()
  {
    Debug.Assert(BallPrefabs.Length > 0, "BallPrefabs is empty");

    foulBallVFX = Instantiate(foulBallVFXPrefab);
    victoryBurstVFX = Instantiate(victoryBurstVFXPrefab);

    activeBalls = new ItemsHolster<BallProjectile>();

    InitBallQueue();
  }

  void OnDestroy()
  {
    OnBallDispensed.RemoveAllListeners();
    OnBallVolleyed.RemoveAllListeners();
    OnBallDestroyed.RemoveAllListeners();
    OnAllBallsDestroyed.RemoveAllListeners();
    OnBallQueueEmpty.RemoveAllListeners();

    StopAllCoroutines();
  }

  void InitBallQueue()
  {
    foreach (var ball in BallPrefabs)
    {
      var ballInstance = Instantiate(ball);
      ballInstance.gameObject.SetActive(false);
      BallQueue.Enqueue(ballInstance);
    }
  }

  public void DestroyBall(BallProjectile ball)
  {
    ball.transform.gameObject.SetActive(false);
    activeBalls.Remove(ball);
    Destroy(ball.gameObject);

    StartCoroutine(foulBallVFX.Play(ball.transform.position));

    OnBallDestroyed?.Invoke(activeBalls.Count);

    if (activeBalls.Count == 0)
      OnAllBallsDestroyed?.Invoke();
  }

  public void DestroyAllActiveBalls() => activeBalls.Clear();

  public void VictoryBurst()
  {
    activeBalls.ForEach(ball => StartCoroutine(victoryBurstVFX.Play(ball.transform.position)));

    DestroyAllActiveBalls();
  }

  public void VolleyBallOnPaddle(Vector3 vector)
  {
    var ball = TakeBallOffPaddle();

    if (ball == null)
    {
      Debug.LogWarning("No ball on paddle.");
      return;
    }

    // Set ball properties
    ball.GetComponent<Rigidbody>().useGravity = true;
    ball.GetComponent<Collider>().enabled = true;
    ball.transform.parent = transform.parent.transform;

    // Throw it
    ball.Launch(vector);

    // Add it to the active balls list
    activeBalls.Add(ball);

    OnBallVolleyed?.Invoke();
  }

  IEnumerator DispenseBall(BallProjectile ball, float delay)
  {
    if (ballOnPaddle != null)
    {
      Debug.LogWarning("Paddle tracking routine running; can't dispense ball.");
      yield break;
    }

    yield return new WaitForSeconds(delay);

    ball.GetComponent<Rigidbody>().useGravity = false;
    ball.GetComponent<Collider>().enabled = false;
    ball.gameObject.SetActive(true);
    ball.transform.parent = null;
    ball.OnDestroyCallback.AddListener(DestroyBall);

    PutBallOnPaddle(ball);

    dispenseRoutine = null;

    if (BallQueue.Count == 0)
      OnBallQueueEmpty?.Invoke();

    OnBallDispensed?.Invoke(BallQueue.Count);
  }

  public BallProjectile DispenseNextBall(float delay = 0f, bool reuseBall = false)
  {
    if (BallQueue.Count == 0)
    {
      Debug.Log("No balls in queue.");
      return null;
    }

    if (!activeBalls.CanAddItems)
    {
      Debug.LogWarning("No more balls allowed.");
      return null;
    }

    var ball = BallQueue.Dequeue();
    if (reuseBall) BallQueue.Enqueue(ball);

    dispenseRoutine = StartCoroutine(DispenseBall(ball, delay));

    return ball;
  }

  void PutBallOnPaddle(BallProjectile ball)
  {
    ball.transform.parent = Paddle;
    ball.transform.position = PositionOnPaddle;
    ball.SetKinematic(true);

    ToggleFX(ball, false);

    ballOnPaddle = ball;
  }

  void ToggleFX(BallProjectile ball, bool isActive)
  {
    foreach (var trail in ball.GetComponentsInChildren<TrailRenderer>(isActive))
    {
      trail.Clear();
      trail.gameObject.SetActive(isActive);
    }
    foreach (var particleSystem in ball.GetComponentsInChildren<ParticleSystem>(isActive))
    {
      particleSystem.Clear();
      particleSystem.gameObject.SetActive(isActive);
    }
  }

  BallProjectile TakeBallOffPaddle()
  {
    if (ballOnPaddle == null)
      return null;

    var ball = ballOnPaddle;
    ballOnPaddle = null;
    ball.transform.parent = null;
    ball.SetKinematic(false);

    ToggleFX(ball, true);

    return ball;
  }

  public void DispenseBallHandler()
  {
    if (dispenseRoutine == null)
    {
      Debug.Log("Dispensing ball");
      DispenseNextBall();
    }
    else Debug.Log("Dispense already in progress");
  }

  public void VolleyBallHandler() => VolleyBallOnPaddle(Vector3.up * VolleyThrustSpeed);

  void OnDrawGizmosSelected()
  {
    // Draw distance offset of the ball from the paddle
    Gizmos.color = Color.red;
    Gizmos.DrawRay(Paddle.position, Vector3.up * VolleyOffsetDistance);

    // Draw a line pointing to the paddle
    Gizmos.color = Color.green;
    Gizmos.DrawLine(transform.position, Paddle.position);

    // Draw the volley offset point
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(PositionOnPaddle, 0.2f);
  }
}
