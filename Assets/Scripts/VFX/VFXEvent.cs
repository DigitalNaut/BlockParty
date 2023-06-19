using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// A ScriptableObject that plays a given Visual Effect at its parent's position.
/// </summary>
/// <remarks>
/// It'll detach the Visual Effect from its parent and destroy it after a given duration.
/// </remarks>
[CreateAssetMenu(fileName = "New VFX Event", menuName = "ScriptableObjects/VFX Events")]
public class VFXEvent : ScriptableObject
{
  [Tooltip("The prefab containing the Visual Effect to play.")]
  [Required] public GameObject VFXPrefab;

  [Tooltip("The name of the event to send to the Visual Effect.")]
  public string VFXEventName = "OnPlay";

  [Tooltip("The delay before playing the Visual Effect.")]
  public float delay = 0.4f;
  [Tooltip("The duration of the Visual Effect.")]
  public float duration = 1f;

  /// <summary>
  /// Plays the Visual Effect at the specified position.
  /// </summary>
  public IEnumerator Play(Vector3 position, float? differentDelay = null, Vector3? differentScale = null)
  {
    yield return new WaitForSeconds(differentDelay ?? delay);

    PlayEffect(position, differentScale);
  }

  public void PlayEffect(Vector3 position, Vector3? scale = null)
  {
    var VFX = Instantiate(VFXPrefab, null);

    VFX.transform.parent = null;
    VFX.transform.position = position;
    if (scale != null) VFX.transform.localScale = (Vector3)scale;

    if (VFX.TryGetComponent(out VisualEffect effect))
      effect.SendEvent(VFXEventName);

    Destroy(VFX, duration);
  }
}
