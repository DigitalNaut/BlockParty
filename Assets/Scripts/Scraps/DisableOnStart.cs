using UnityEngine;

public class DisableOnStart : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
    transform.gameObject.SetActive(false);
  }
}
