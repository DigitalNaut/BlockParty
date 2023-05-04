using UnityEngine;

[RequireComponent(typeof(Breakable))]
public class Explosive : MonoBehaviour
{
  [Range(0.1f, 4f)]
  public float scale = 1f;

  [Range(1, 32)]
  public int divisions = 8;

  Breakable breakable;
  Vector3[] angles;

  void OnEnable()
  {
    breakable = GetComponent<Breakable>();
    angles = GetAngles();
  }

  void OnDisable() => breakable.OnBreak -= Explode;

  void Start() => breakable.OnBreak += Explode;

  Vector3[] GetAngles()
  {
    var length = transform.localScale.x * scale;
    var angle = 360f / divisions;
    var angles = new Vector3[divisions];

    for (int i = 1; i <= divisions; i++)
    {
      var direction = Quaternion.Euler(0, 0, angle * i) * Vector3.right * length;
      angles[i - 1] = direction;
    }

    return angles;
  }

  void Explode()
  {
    var hits = new RaycastHit[8];

    foreach (var direction in angles)
    {
      Physics.RaycastNonAlloc(transform.position, direction, hits, transform.forward.x);

      foreach (var hit in hits)
      {
        hit.collider.TryGetComponent(out Breakable hitCollider);
        if (hitCollider != null)
          hitCollider.Break(Random.Range(0.1f, 0.25f));
      }
    }
  }

  void OnDrawGizmosSelected()
  {
    foreach (var direction in angles)
      Gizmos.DrawRay(transform.position, direction);
  }

  void OnDestroy() => breakable.OnBreak -= Explode;
}
