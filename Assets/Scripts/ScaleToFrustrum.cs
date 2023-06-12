using UnityEngine;

public class ScaleToFrustrum : MonoBehaviour
{
  void Start() => ScaleObject();
  void OnDrawGizmosSelected() => ScaleObject();

  void ScaleObject()
  {
    if(!enabled) return;

    float distance = Camera.main.farClipPlane - 0.01f;
    transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
    transform.LookAt(Camera.main.transform);
    transform.Rotate(90.0f, 0, 0);

    float h = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * distance / 5.0f; // Don't know why I need to divide by 5

    transform.localScale = new Vector3(h * Camera.main.aspect, 1f, h);
  }
}
