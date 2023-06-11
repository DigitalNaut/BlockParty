using UnityEngine;

[Icon("Assets/Icons/Scripts/PlayCrossedOut.png")]
public class DisableOnStart : MonoBehaviour
{
  void Start() => transform.gameObject.SetActive(false);
}
