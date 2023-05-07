using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "New VFX Event", menuName = "ScriptableObjects/VFX Events")]
public class VFXEvent : ScriptableObject
{
  [Tooltip("The prefab containing the Visual Effect to play.")]
  [Required] public GameObject VFXPrefab;

  [Tooltip("The name of the event to send to the Visual Effect.")]
  public string VFXEventName = "OnPlay";

  [Tooltip("The delay before playing the Visual Effect.")]
  public float delay = 0.4f;

  VisualEffect VFX;

  void Awake()
  {
    if (VFXPrefab != null)
    {
      VFX = Instantiate(VFXPrefab).GetComponent<VisualEffect>();
      VFX.gameObject.SetActive(true);
    }
  }

  /// <summary>
  /// Plays the Visual Effect at the specified position.
  /// </summary>
  public IEnumerator PlayEffectAtPosition(Vector3 position, Vector3? scale = null)
  {
    yield return new WaitForSeconds(delay);

    if (VFX == null)
      yield break;

    VFX.transform.parent = null;
    VFX.transform.position = position;
    if (scale != null) VFX.transform.localScale = (Vector3)scale;

    VFX.SendEvent(VFXEventName);
  }
}
