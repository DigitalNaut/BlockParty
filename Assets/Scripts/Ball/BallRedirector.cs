using UnityEngine;


[RequireComponent(typeof(Renderer))]
public class BallRedirector : MonoBehaviour
{
  [Range(0f, 90f)]
  [Tooltip("In degrees, which result in a bouncing angle between -x° and x°.")]
  public float RedirectionAngle = 45f;

  private new Renderer renderer;

  private void Start() => renderer = GetComponent<Renderer>();

  private void OnCollisionEnter(Collision collision)
  {
    BallProjectile launchableObj = collision.gameObject.GetComponent<BallProjectile>();

    if (launchableObj)
    {
      ContactPoint contact = collision.GetContact(0);

      // Returns -1 to 1 based on collision relative to bounds width
      Vector3 normalizedCollisionPoint =
          (renderer.bounds.center - contact.point)        // Center to point of contact
          / renderer.bounds.size.x                        // Dived by bounds width
          * 2f;                                           // *2f since bounds are half size

      // Turns previous calculation into angle between -45f and 45f
      Vector3 newDirection =
          Quaternion.AngleAxis(
              Mathf.Pow(normalizedCollisionPoint.x, 3)    // Sigmoidal function x^3 for narrow angles near center, wider angles near the edges
              * RedirectionAngle, Vector3.forward         // Vector3.forward to rotate on Z axis (0, 0, 1)
          ) * Vector3.up;

      //Debug.DrawRay(transform.position, newDirection, Color.red, 100f);

      launchableObj.SetDirection(newDirection);
    }
  }
}
