using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "New VFX Event", menuName = "ScriptableObjects/VFX Events")]
public class VFXEvent : ScriptableObject
{
  [Tooltip("The prefab containing the Visual Effect to play.")]
  public GameObject VFXPrefab;

  [Tooltip("The name of the event to send to the Visual Effect.")]
  public string VFXEventName = "OnPlay";

  [Tooltip("The delay before playing the Visual Effect.")]
  public float delay = 0.4f;

  VisualEffect VFX;

  void Awake()
  {
    Debug.Assert(VFXPrefab != null, "VFX Prefab is null", this);

    if (VFXPrefab != null)
    {
      VFX = Instantiate(VFXPrefab).GetComponent<VisualEffect>();
      VFX.gameObject.SetActive(true);
    }
  }

  void PlayVFX(Vector3 position)
  {
    if (VFX == null)
      return;

    VFX.transform.parent = null;
    VFX.transform.position = position;
    VFX.transform.localScale = Vector3.one;

    VFX.SendEvent(VFXEventName);
  }

  /// <summary>
  /// Plays the Visual Effect at the specified position.
  /// </summary>
  public IEnumerator PlayEffectAtPosition(Vector3 position)
  {
    yield return new WaitForSeconds(delay);

    PlayVFX(position);
  }
}
