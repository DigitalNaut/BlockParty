using UnityEngine;

[Icon("Assets/Textures/Icons/PlayCrossedOut.png")]
public class DisableOnStart : MonoBehaviour
{
  void Start() => transform.gameObject.SetActive(false);
}
