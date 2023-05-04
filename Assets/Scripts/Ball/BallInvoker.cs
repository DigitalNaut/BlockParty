using UnityEngine;
using UnityEngine.VFX;

public class BallInvoker : MonoBehaviour
{
  public BallManager BallManager;
  public VisualEffect RecallEffect;
  public string VFXName = "PlayInvoke";

  new Collider collider;

  void Start()
  {
    collider = GetComponent<Collider>();
  }

  void OnCollisionEnter(Collision collision)
  {
    var lucidBall = BallManager.GetLucidBall(out bool lucidBallWasActive);

    if (lucidBall == null) return;

    var spawner = BallManager.GetComponent<AnimatedLucidBallSpawner>();
    var collidingProjectile = collision.collider.gameObject.GetComponent<BallProjectile>();

    if (spawner == null && collidingProjectile == null) return;

    if (lucidBallWasActive) PlayVFX(RecallEffect, VFXName, lucidBall.transform.position);

    collider.enabled = false;
    lucidBall.transform.position = transform.position;
    lucidBall.gameObject.SetActive(true);
    spawner.StartCoroutine(spawner.Init(collidingProjectile, lucidBall, GetComponent<Breakable>()));
  }

  void OnDisable() => StopAllCoroutines();

  void OnDrawGizmosSelected() => Debug.DrawLine(transform.position, BallManager.gameObject.transform.position, Color.cyan);

  void PlayVFX(VisualEffect vfx, string vfxName, Vector3 effectPosition)
  {
    if (vfx == null) return;

    vfx.transform.parent = null;
    vfx.transform.position = effectPosition;
    vfx.transform.localScale = Vector3.one;

    vfx.gameObject.SetActive(true);
    vfx.SendEvent(vfxName);
  }
}
