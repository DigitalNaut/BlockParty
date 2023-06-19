using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelManagerSettings", menuName = "Settings/LevelManagerSettings")]
public class LevelManagerSettings : ScriptableObject
{
  [field: Dropdown("GetSceneNames")]
  [field: SerializeField] public string MainMenuSceneName { get; private set; } = "MainMenu";

#if UNITY_EDITOR
  string[] GetSceneNames => EditorBuildSettings.scenes
            .Select(scene => scene.path)
            .ToArray();
#endif
}
