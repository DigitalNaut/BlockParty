using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(BrickWallManager))]
public class WallGeneratorsOrganizer : MonoBehaviour
{
  [SerializeField] BrickWallManager brickWallManager;

  [Dropdown("brickWallGenerators")]
  [OnValueChanged("SetBrickWallGenerator")]
  [SerializeField] BrickWallGenerator currentBrickWallGenerator;

  List<BrickWallGenerator> brickWallGenerators;

  void OnValidate()
  {
    if (brickWallManager == null)
      brickWallManager = GetComponent<BrickWallManager>();

    if (currentBrickWallGenerator == null)
      currentBrickWallGenerator = brickWallManager.GetBrickWallGenerator();

    brickWallGenerators = new List<BrickWallGenerator>(GetComponentsInChildren<BrickWallGenerator>());
  }

  void SetBrickWallGenerator()
  {
    brickWallManager.SetBrickWallGenerator(currentBrickWallGenerator);
  }
}
