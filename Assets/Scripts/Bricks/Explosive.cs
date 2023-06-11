using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[Icon("Assets/Icons/Scripts/Explosive.png")]

[RequireComponent(typeof(Breakable))]
public class Explosive : MonoBehaviour
{
  [Header("Settings")]
  [Range(0.1f, 4f)] public float scale = 1f;
  [Range(1, 32)] public int divisions = 8;

  [Foldout("Events")] UnityEvent<Explosive> OnExplode;

  Breakable breakable;
  Vector3[] angles;

  void Awake() => breakable = GetComponent<Breakable>();

  void Start() => OnExplode ??= new UnityEvent<Explosive>();

  void OnEnable()
  {
    angles = GetAngles();
    breakable.OnBreak.AddListener(Explode);
  }

  void OnDisable() => breakable.OnBreak.RemoveListener(Explode);

  void OnDestroy() => OnExplode?.RemoveAllListeners();

  Vector3[] GetAngles()
  {
    if (divisions < 1)
      throw new System.Exception("Divisions must be greater than 0");

    var length = transform.localScale.x * scale;
    var angle = 360f / divisions;
    var angles = new Vector3[divisions];

    for (int i = 0; i < divisions; i++)
      angles[i] = Quaternion.Euler(0, 0, angle * i) * Vector3.right * length;

    return angles;
  }

  void Explode(Breakable breakable, Collision collision)
  {
    var hits = new RaycastHit[1];

    foreach (var direction in angles)
    {
      Physics.RaycastNonAlloc(transform.position, direction, hits, scale);

      var hit = hits[0];

      if (hit.collider && hit.collider.TryGetComponent(out Breakable other))
        other.BreakStrategy?.Invoke(null, Random.Range(0.1f, 0.25f));
    }

    OnExplode?.Invoke(this);
  }

  void OnDrawGizmosSelected()
  {
    angles ??= GetAngles();
    foreach (var direction in angles) Gizmos.DrawRay(transform.position, direction * scale);
  }
}
