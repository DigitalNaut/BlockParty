using System;
using UnityEngine;

[Icon("Assets/Textures/Icons/Bounce.png")]
[RequireComponent(typeof(Renderer))]
public class BouncePaddle : MonoBehaviour
{
  enum InfluenceMode { Linear, Square, Sigmoidal }

  [Header("Properties")]
  [Tooltip("Influence mode determines how the paddle's width affects the bounce direction.")]
  [SerializeField] InfluenceMode influenceMode = InfluenceMode.Square;
  [Tooltip("In degrees, which result in a bouncing angle between -x° and x°.")]
  [Range(0f, 90f)] public float RedirectionAngle = 45f;

  [Header("Debug")]
  [SerializeField] bool debug = false;

  new Renderer renderer;
  Func<float, float> influenceFunction;
  Action<Collision> CollisionStrategy;
  Debugger debugger;

  void Awake() => renderer = GetComponent<Renderer>();

  void OnValidate()
  {
    SetInfluenceFunction(influenceMode);
    if (debug) debugger = new Debugger();
  }

  void OnEnable() => CollisionStrategy = InfluenceCollision;
  void OnDisable() => CollisionStrategy = null;

  void OnCollisionEnter(Collision collision) => CollisionStrategy?.Invoke(collision);

  void InfluenceCollision(Collision collision)
  {
    if (collision.gameObject.TryGetComponent(out BallProjectile launchableObj))
    {
      var newDirection = GetBounceDirection(collision);
      launchableObj.SetDirection(newDirection);
    }
  }

  void SetInfluenceFunction(InfluenceMode mode)
  {
    switch (mode)
    {
      case InfluenceMode.Linear:
        influenceFunction = x => x;
        break;
      case InfluenceMode.Square:
        influenceFunction = x => x * Math.Abs(x);
        break;
      case InfluenceMode.Sigmoidal:
        influenceFunction = x => x * x * x;
        break;
    }
  }

  Vector3 GetBounceDirection(Collision collision)
  {
    var contact = collision.GetContact(0);
    var directionInfluence = GetDirectionInfluence(contact.point);

    // Cross product to get a perpendicular vector
    var ballBounceDirection = Vector3.Reflect(collision.rigidbody.velocity.normalized, contact.normal);
    var intermediateAngle = Vector3.Angle(ballBounceDirection, directionInfluence) * Mathf.Deg2Rad * 0.5f;
    var newDirection = Vector3.RotateTowards(ballBounceDirection, directionInfluence, intermediateAngle, 0f);

    if (debug)
      debugger.TraceBounceDirection(contact, ballBounceDirection, directionInfluence, newDirection);

    return newDirection;
  }

  Vector3 GetDirectionInfluence(Vector3 contactPoint)
  {
    // Returns -1 to 1 based on collision relative to bounds width
    Vector3 relativeContactPoint = (renderer.bounds.center - contactPoint) / renderer.bounds.extents.x;

    // Returns -1 to 1 based on influence mode
    var influence = influenceFunction(relativeContactPoint.x);

    // Turns previous calculation into angle between -45f and 45f around the z axis
    Vector3 directionInfluence = Quaternion.AngleAxis(influence * RedirectionAngle, Vector3.forward) * Vector3.up;

    return directionInfluence;
  }

  void OnDrawGizmosSelected()
  {
    if (renderer == null)
      renderer = GetComponent<Renderer>();

    Gizmos.color = Color.yellow;

    var sections = 20;
    var sectionSize = renderer.bounds.size.x / sections;

    for (var i = 0; i <= sections; i++)
    {
      var pointOffset = new Vector3(
        sectionSize * i - renderer.bounds.extents.x,
        renderer.bounds.extents.y,
        0f
      );
      var point = renderer.bounds.center + pointOffset;
      var direction = GetDirectionInfluence(point);

      Gizmos.DrawRay(point, direction);
    }
  }

  class Debugger
  {
    public void TraceBounceDirection(ContactPoint contact, Vector3 ballBounceDirection, Vector3 directionInfluence, Vector3 newDirection)
    {
      Debug.Log($"BallBounceDirection: {Vector3.Angle(ballBounceDirection, directionInfluence)}");
      Debug.DrawRay(contact.point, ballBounceDirection, Color.blue, Mathf.Infinity);
      Debug.DrawRay(contact.point, directionInfluence * 3f, Color.yellow, Mathf.Infinity);
      Debug.DrawRay(contact.point, newDirection * 2f, Color.green, Mathf.Infinity);
    }
  }
}