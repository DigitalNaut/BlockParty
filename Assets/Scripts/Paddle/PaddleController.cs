using UnityEngine;
using UnityEngine.InputSystem;

[Icon("Assets/Textures/Script Icons/Joystick.png")]
[RequireComponent(typeof(Rigidbody))]
public class PaddleController : MonoBehaviour
{
  [SerializeField] float SlideSpeed = 10000f;
  [SerializeField] float MinSpeed = 4f;
  new Rigidbody rigidbody;
  Camera mainCamera;
  Mouse currentMouse;
  private Vector2 previousMousePosition;

  delegate void OnMouseMoved();
  OnMouseMoved onMouseMoved;

  private void Awake()
  {
    rigidbody = GetComponent<Rigidbody>();
    mainCamera = Camera.main;
    currentMouse = Mouse.current;
  }

  void OnEnable() => onMouseMoved = TrackMouse;

  void OnDisable()
  {
    onMouseMoved = null;
    rigidbody.velocity = Vector3.zero;
  }

  private void FixedUpdate() => onMouseMoved();

  private void TrackMouse()
  {
    Vector2 currentMousePosition = currentMouse.position.ReadValue();
    if (currentMousePosition == previousMousePosition)
    {
      rigidbody.velocity = Vector3.zero;
      return;
    }

    previousMousePosition = currentMousePosition;

    Vector3 mousePositionWorld = mainCamera.ScreenToWorldPoint(
        new Vector3(
              currentMousePosition.x,
              currentMousePosition.y,
              -mainCamera.transform.position.z
            ));

    var distance = mousePositionWorld.x - transform.position.x;
    var slideForce = new Vector3(distance * Time.fixedDeltaTime * SlideSpeed, 0, 0);
    rigidbody.AddForce(slideForce);

    rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, Mathf.Min(distance * distance, MinSpeed));
  }
}
