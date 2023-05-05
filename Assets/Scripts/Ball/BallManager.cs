using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerInput))]
public class BallManager : MonoBehaviour
{
  [Header("Volley")]
  [SerializeField][Range(0.1f, 30f)] float VolleyThrustSpeed = 15f;
  [SerializeField][Range(0f, 1f)] float VolleyOffsetDistance = 0.5f;

  [Header("Prefabs")]
  [SerializeField] Transform Paddle;
  [SerializeField] BallProjectile[] BallPrefabs;

  PlayerInput playerInput;
  Coroutine dispenseRoutine;
  BallProjectile ballOnPaddle;
  Queue<BallProjectile> BallQueue = new Queue<BallProjectile>();
  BallsHolster activeBalls;

  Vector3 PositionOnPaddle => Paddle.position + Vector3.up * VolleyOffsetDistance;
  public AudioSource AudioSource { get; private set; }

  void Awake()
  {
    AudioSource = GetComponent<AudioSource>();
    playerInput = GetComponent<PlayerInput>();

    Debug.Assert(Paddle != null, "Paddle not set.", transform);
    Debug.Assert(BallPrefabs != null && BallPrefabs.Length > 0, "No ball prefabs set.", transform);

    activeBalls = gameObject.AddComponent<BallsHolster>();

    InitBallQueue();
  }

  void Start() => DispenseNextBall();

  void OnEnable()
  {
    playerInput.onDispenseBall.AddListener(DispenseBallHandler);
    playerInput.onVolleyBall.AddListener(VolleyBallHandler);
  }

  void OnDisable()
  {
    playerInput.onDispenseBall.RemoveListener(DispenseBallHandler);
    playerInput.onVolleyBall.RemoveListener(VolleyBallHandler);
  }

  void OnDestroy() => StopAllCoroutines();

  void InitBallQueue()
  {
    foreach (var ball in BallPrefabs)
    {
      var ballInstance = Instantiate(ball);
      ballInstance.gameObject.SetActive(false);
      BallQueue.Enqueue(ballInstance);
    }
  }

  public void BallDestroyedCallback(BallProjectile target)
  {
    target.transform.gameObject.SetActive(false);
    activeBalls.RemoveBall(target);
  }

  public void VolleyBall(BallProjectile ball, Vector3 vector)
  {
    TakeBallOffPaddle();

    // Set ball properties
    ball.GetComponent<Rigidbody>().useGravity = true;
    ball.GetComponent<Collider>().enabled = true;
    ball.transform.parent = transform.parent.transform;

    // Throw it
    ball.Launch(vector);

    // Add it to the active balls list
    activeBalls.AddBall(ball);
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
    ball.OnDestroyCallback += BallDestroyedCallback;

    PutBallOnPaddle(ball);

    dispenseRoutine = null;
  }

  public BallProjectile DispenseNextBall(float delay = 0f, bool reuseBall = false)
  {
    if (BallQueue.Count == 0)
    {
      Debug.Log("No balls in queue.");
      return null;
    }

    if (!activeBalls.CanAddBalls)
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

    ballOnPaddle = ball;
  }

  BallProjectile TakeBallOffPaddle()
  {
    if (ballOnPaddle == null)
    {
      Debug.LogWarning("No ball on paddle.");
      return null;
    }

    var ball = ballOnPaddle;
    ballOnPaddle = null;
    ball.transform.parent = null;
    ball.SetKinematic(false);

    return ball;
  }

  void DispenseBallHandler()
  {
    if (dispenseRoutine == null)
    {
      Debug.Log("Dispensing ball");
      DispenseNextBall();
    }
    else Debug.Log("Dispense already in progress");
  }

  void VolleyBallHandler()
  {
    if (ballOnPaddle == null)
    {
      Debug.Log("No ball on paddle.");
      return;
    }

    var ball = TakeBallOffPaddle();

    VolleyBall(ball, Vector3.up * VolleyThrustSpeed);
  }

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
