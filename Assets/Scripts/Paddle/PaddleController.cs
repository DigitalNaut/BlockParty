using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PaddleController : MonoBehaviour
{
  [SerializeField]
  float SlideSpeed = 12f;
  [SerializeField]
  float MaxSpeed = 4f;
  new Rigidbody rigidbody;

  private void Start()
  {
    rigidbody = GetComponent<Rigidbody>();
  }

  private void FixedUpdate()
  {
    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
        new Vector3(
            Mouse.current.position.ReadValue().x,
            Mouse.current.position.ReadValue().y,
            -Camera.main.transform.position.z
            ));

    var d = (mousePosition.x - transform.position.x);
    var v = new Vector3(
            d * Time.deltaTime * SlideSpeed,
            0,
            0);
    rigidbody.AddForce(v);
    rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, Mathf.Min(d * d, MaxSpeed));
  }
}
