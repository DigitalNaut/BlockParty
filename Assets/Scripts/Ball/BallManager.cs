using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class BallManager : MonoBehaviour
{
  public BallProjectile MainBallPrefab;
  public BallProjectile LucidBallPrefab;
  public AudioClip BallSwapSound;

  public float VolleyThrust = 15f;
  public float VolleyOffsetDistance = 0.5f;
  BallProjectile lucidBall = null;
  Coroutine dispenseRoutine;
  AudioSource audioSource;

  public BallProjectile MainBall { get; private set; } = null;

  public BallProjectile GetLucidBall(out bool lucidBallWasActive)
  {
    if (lucidBall == null)
    {
      lucidBall = Instantiate(LucidBallPrefab);
      var tracker = lucidBall.gameObject.AddComponent<CollisionTracker>();
      tracker.CollisionAction = SwapLucidBall;
    }

    lucidBallWasActive = lucidBall.gameObject.activeInHierarchy;

    lucidBall.gameObject.SetActive(false);
    return lucidBall;
  }

  void SwapLucidBall(Collider collider)
  {
    // Guard
    if (!collider.GetComponent<PaddleController>()) return;
    if (MainBall.gameObject.activeInHierarchy) return;

    // Swap lucid ball for regular ball
    lucidBall.transform.gameObject.SetActive(false);
    MainBall.transform.position = lucidBall.transform.position;
    MainBall.gameObject.SetActive(true);
    Volley(lucidBall.GetComponent<Rigidbody>().velocity);

    // Play VFX
    lucidBall.PlaySwapEffect();
    MainBall.PlaySwapEffect();

    // Play sound
    if (BallSwapSound && audioSource != null) audioSource.PlayOneShot(BallSwapSound);

    // Prevent from dispensing a new ball
    if (dispenseRoutine != null) StopCoroutine(dispenseRoutine);
  }

  void Start()
  {
    StartCoroutine(Dispense(0f));

    if (MainBallPrefab == null)
      Debug.LogError("Main ball not set.", transform);
    if (GetLucidBall(out _) == null)
      Debug.LogError("Lucid ball not set.", transform);
    if (BallSwapSound == null)
      Debug.LogError("Ball swap sound not set.", transform);

    audioSource = GetComponent<AudioSource>();
  }

  void Update()
  {
    var keyboard = Keyboard.current;
    if (keyboard == null) return;

    if (keyboard.spaceKey.wasPressedThisFrame)
    {
      Debug.Log("Dispense Cr: " + dispenseRoutine);
      if (!MainBall.isActiveAndEnabled && dispenseRoutine == null)
        dispenseRoutine = StartCoroutine(Dispense(0.5f));
    }
  }

  IEnumerator PaddleTrackBall()
  {
    while (MainBall)
    {
      MainBall.transform.position =
          transform.position +
          Vector3.up * VolleyOffsetDistance;

      if (Mouse.current.leftButton.isPressed)
      {
        Volley(Vector3.up * VolleyThrust);
        yield break;
      }
      else yield return new WaitForFixedUpdate();
    }
  }

  void Volley(Vector3 vector)
  {
    // Set ball properties
    MainBall.GetComponent<Rigidbody>().useGravity = true;
    MainBall.GetComponent<Collider>().enabled = true;
    MainBall.transform.parent = transform.parent.transform;
    MainBall.OnDestroyCallback = BallDestroyedCallback;

    // Throw it
    MainBall.Launch(vector);
  }

  IEnumerator Dispense(float waitTime)
  {
    if (MainBallPrefab == null)
    {
      Debug.LogError("Volley object not set.", transform);
      yield return null;
    }

    yield return new WaitForSeconds(waitTime);

    if (MainBall == null)
      MainBall = Instantiate(MainBallPrefab);
    if (lucidBall == null)
      lucidBall = GetLucidBall(out _);

    MainBall.GetComponent<Rigidbody>().useGravity = false;
    MainBall.GetComponent<Collider>().enabled = false;
    MainBall.gameObject.SetActive(true);

    StartCoroutine(PaddleTrackBall());
  }

  void OnDisable() => StopAllCoroutines();

  void OnDestroy() => StopAllCoroutines();

  void OnDrawGizmosSelected()
  {
    Debug.DrawRay(transform.position, Vector3.up * VolleyOffsetDistance, Color.blue);

    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position + Vector3.up * VolleyOffsetDistance, 0.2f);
  }

  public void BallDestroyedCallback(BallProjectile target) => target.transform.gameObject.SetActive(false);
}

public class CollisionTracker : MonoBehaviour
{
  public Action<Collider> CollisionAction;
  void OnCollisionEnter(Collision collision)
  {
    CollisionAction?.Invoke(collision.collider);
  }
}
