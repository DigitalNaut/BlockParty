using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

[Icon("Assets/Textures/Icons/FragileWarning.png")]
public class Breakable : MonoBehaviour
{
  [Header("Dependencies")]
  [Required][SerializeField] AudioClip AudioClip;
  [Required][SerializeField] VisualEffect BreakEffect;

  [Header("Settings")]
  [SerializeField] string BreakEffectEventName = "PlayBurst";

  [Foldout("Events")] public UnityEvent<Breakable> OnBreak;

  void Awake() => Debug.Assert(string.IsNullOrEmpty(BreakEffectEventName) == false, "BreakEffectEventName is empty");

  void Start() => OnBreak ??= new UnityEvent<Breakable>();

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

      OnBreak?.Invoke(this);

      yield return new WaitForFixedUpdate();

      Destroy(gameObject);
    }

    StartCoroutine(breakSubroutine());
  }
}
