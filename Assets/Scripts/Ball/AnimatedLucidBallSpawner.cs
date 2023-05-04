using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// This class is responsible for spawning the lucid ball and playing the animation
/// </summary>
public class AnimatedLucidBallSpawner : MonoBehaviour
{
  public VisualEffect ImplodeEffect;
  public string vfxImplodeName = "PlayImplode";
  public VisualEffect ExplodeEffect;
  public string vfxExplodeName = "PlayExplode";
  public float ImplosionAnimationLength = 0.4f;
  public float ExplosionAnimationLength = 0.4f;

  Vector3 animationPosition;

  void OnDisable() => StopAllCoroutines();

  public IEnumerator Init(BallProjectile collidingBall, BallProjectile lucidBall, Breakable brick)
  {
    if (collidingBall == null)
      yield break;

    // Hide lucid ball
    lucidBall.gameObject.SetActive(false);

    // Set the lucid ball's position and velocity
    lucidBall.GetComponent<Rigidbody>().velocity = collidingBall.GetComponent<Rigidbody>().velocity;
    animationPosition = (brick != null) ? brick.transform.position : transform.position;

    yield return Animate();

    // Show lucid ball
    lucidBall.gameObject.SetActive(true);
  }

  void PlayVFX(VisualEffect vfx, string effectName)
  {
    if (vfx == null) return;

    // Play the animation
    vfx.transform.parent = null;
    vfx.transform.position = animationPosition;
    vfx.transform.localScale = Vector3.one;
    vfx.SendEvent(effectName);
  }

  IEnumerator Animate()
  {
    yield return StartCoroutine(Implode());
    yield return StartCoroutine(Explode());
  }

  IEnumerator Implode()
  {
    yield return new WaitForSeconds(ImplosionAnimationLength);
    PlayVFX(ImplodeEffect, vfxImplodeName);

    yield break;
  }

  IEnumerator Explode()
  {
    yield return new WaitForSeconds(ExplosionAnimationLength);
    PlayVFX(ExplodeEffect, vfxExplodeName);

    yield break;
  }
}
