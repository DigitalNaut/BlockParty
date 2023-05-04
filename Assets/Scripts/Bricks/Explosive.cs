using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Breakable))]
public class Explosive : MonoBehaviour
{
  [Range(0.1f, 4f)] public float scale = 1f;
  [Range(1, 32)] public int divisions = 8;

  Breakable breakable;
  Vector3[] angles;
  UnityEvent OnExplode = new UnityEvent();

  void Awake()
  {
    OnExplode = new UnityEvent();
    breakable = GetComponent<Breakable>();
  }

  void OnEnable()
  {
    angles = GetAngles();
    breakable.OnBreak.AddListener(Explode);
  }

  void OnDisable() => breakable.OnBreak.RemoveListener(Explode);

  void OnDestroy() => OnExplode.RemoveAllListeners();

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

  void Explode()
  {
    var hits = new RaycastHit[16];

    foreach (var direction in angles)
    {
      Physics.RaycastNonAlloc(transform.position, direction, hits, scale);

      foreach (var hit in hits)
      {
        if (hit.collider && hit.collider.TryGetComponent(out Breakable breakable))
          breakable.Break(Random.Range(0.1f, 0.25f));
      }
    }

    OnExplode?.Invoke();
  }

  void OnDrawGizmosSelected()
  {
    angles ??= GetAngles();
    foreach (var direction in angles) Gizmos.DrawRay(transform.position, direction * scale);
  }
}
