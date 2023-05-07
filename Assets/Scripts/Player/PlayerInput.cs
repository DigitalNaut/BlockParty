using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Icon("Assets/Textures/Icons/Joystick.png")]
public class PlayerInput : MonoBehaviour
{
  [Foldout("Events")] public UnityEvent onDispenseBall;
  [Foldout("Events")] public UnityEvent onVolleyBall;

  InputActions inputActions;

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
