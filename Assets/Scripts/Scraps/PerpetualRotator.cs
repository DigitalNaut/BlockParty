using UnityEngine;

public class PerpetualRotator : MonoBehaviour
{
  [Range(0f, 270f)]
  public float TurnSpeed = 5f;
  public Vector3 TurnDirection = Vector3.up;

  void FixedUpdate()
  {
    transform.RotateAround(transform.position, TurnDirection, Time.deltaTime * TurnSpeed);
  }
}
