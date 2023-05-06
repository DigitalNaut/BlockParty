using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class Breakable : MonoBehaviour
{
  [SerializeField] AudioClip AudioClip;
  [SerializeField] VisualEffect BreakEffect;
  [SerializeField] string BreakEffectEventName = "PlayBurst";

  public UnityEvent OnBreak;

  void Awake() => OnBreak = new UnityEvent();

  void OnCollisionStay(Collision collision) => Break();

  void OnCollisionEnter(Collision collision) => Break();

  void OnTriggerEnter() => Break();

  void OnDestroy() => OnBreak.RemoveAllListeners();

  public void Break(float delay = 0)
  {
    IEnumerator breakSubroutine()
    {
      if (delay > 0)
        yield return new WaitForSeconds(delay);

      AudioSource.PlayClipAtPoint(AudioClip, transform.position);

      if (BreakEffect)
      {
        BreakEffect.transform.parent = null;
        BreakEffect.SendEvent(BreakEffectEventName);
        Destroy(BreakEffect.gameObject, 1f);
      }

      OnBreak?.Invoke();

      yield return new WaitForFixedUpdate();

      Destroy(gameObject);
    }

    StartCoroutine(breakSubroutine());
  }
}
