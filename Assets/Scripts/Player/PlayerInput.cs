using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
  InputActions inputActions;
  public UnityEvent onDispenseBall;
  public UnityEvent onVolleyBall;

  void Awake() => inputActions = new InputActions();

  void OnEnable()
  {
    inputActions.Enable();
    inputActions.Player.DispenseBall.performed += DispenseBallListener;
    inputActions.Player.VolleyBall.performed += VolleyBallListener;
  }

  void OnDisable()
  {
    inputActions.Disable();
    inputActions.Player.DispenseBall.performed -= DispenseBallListener;
    inputActions.Player.VolleyBall.performed -= VolleyBallListener;
  }

  void DispenseBallListener(InputAction.CallbackContext ctx) => onDispenseBall.Invoke();

  void VolleyBallListener(InputAction.CallbackContext ctx) => onVolleyBall.Invoke();
}
