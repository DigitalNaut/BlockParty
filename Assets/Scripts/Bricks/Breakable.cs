using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

[Icon("Assets/Textures/Script Icons/FragileWarning.png")]
public class Breakable : MonoBehaviour
{
  [Header("Dependencies")]
  [Required][SerializeField] AudioClip AudioClip;
  [Required][SerializeField] VFXEvent BreakEffect;

  [Header("Settings")]
  [SerializeField] string BreakEffectEventName = "PlayBurst";

  [Foldout("Events")] public UnityEvent<Breakable, Collision> OnBreak;

  public delegate void BreakStrategyDelegate(Collision collision = null, float delay = 0);
  public BreakStrategyDelegate BreakStrategy { get; private set; }

  void Awake() => Debug.Assert(string.IsNullOrEmpty(BreakEffectEventName) == false, "BreakEffectEventName is empty");

  void Start()
  {
    OnBreak ??= new UnityEvent<Breakable, Collision>();
    BreakStrategy = Break;
  }

  void OnCollisionStay(Collision collision) => BreakStrategy?.Invoke(collision);

  void OnCollisionEnter(Collision collision) => BreakStrategy?.Invoke(collision);

  void OnTriggerEnter() => BreakStrategy?.Invoke();

  void OnDestroy() => OnBreak.RemoveAllListeners();

  void Break(Collision collision = null, float delay = 0)
  {
    BreakStrategy = null;

    IEnumerator breakSubroutine()
    {
      if (delay > 0)
        yield return new WaitForSeconds(delay);

      AudioSource.PlayClipAtPoint(AudioClip, transform.position);

      if (BreakEffect != null)
        BreakEffect.PlayEffect(transform.position);

      OnBreak?.Invoke(this, collision);

      yield return new WaitForFixedUpdate();

      Destroy(gameObject);
    }

    StartCoroutine(breakSubroutine());
  }
}
