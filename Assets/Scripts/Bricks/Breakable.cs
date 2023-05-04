using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Breakable : MonoBehaviour
{
  public AudioClip AudioClip;
  public VisualEffect BreakEffect;
  public Action OnBreak;

  void OnCollisionStay(Collision collision) => Break();

  void OnCollisionEnter(Collision collision) => Break();

  void OnTriggerEnter() => Break();

  void OnDestroy() => OnBreak?.Invoke();

  public void Break(float delay = 0)
  {
    IEnumerator subroutine()
    {
      if (delay > 0)
        yield return new WaitForSeconds(delay);

      AudioSource.PlayClipAtPoint(AudioClip, transform.position);

      if (BreakEffect)
      {
        BreakEffect.transform.parent = null;
        BreakEffect.SendEvent("PlayBurst");
        Destroy(BreakEffect.gameObject, 1f);
      }

      yield return new WaitForFixedUpdate();

      Destroy(gameObject);

      yield break;
    }

    StartCoroutine(subroutine());
  }
}
