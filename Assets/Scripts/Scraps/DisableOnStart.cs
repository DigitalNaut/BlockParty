using UnityEngine;

[Icon("Assets/Textures/Script Icons/PlayCrossedOut.png")]
public class DisableOnStart : MonoBehaviour
{
  void Start() => transform.gameObject.SetActive(false);
}
