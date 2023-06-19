using UnityEngine.UIElements;
using UnityEngine;

[Icon("Assets/Icons/Scripts/UI/Overlay.png")]
public class LoaderOverlay : MonoBehaviour
{
  VisualElement root;
  ProgressBar progressBar;

  void Awake()
  {
    root = GetComponent<UIDocument>().rootVisualElement;
    progressBar = root.Q<ProgressBar>("ProgressBar");
  }

  public void Show(bool isActive) => gameObject.SetActive(isActive);

  public void SetProgress(float progress) => progressBar.value = progress;
}
