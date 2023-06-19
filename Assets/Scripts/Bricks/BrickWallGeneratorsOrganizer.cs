using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(BrickWallManager))]
public class BrickWallGeneratorsOrganizer : MonoBehaviour
{
  [SerializeField] BrickWallManager brickWallManager;

  [Dropdown("brickWallGenerators")]
  [OnValueChanged("SetBrickWallGenerator")]
  [SerializeField] BrickWallGenerator currentBrickWallGenerator;

  List<BrickWallGenerator> brickWallGenerators = new List<BrickWallGenerator>();

  void OnValidate()
  {
    if (brickWallManager == null)
      brickWallManager = GetComponent<BrickWallManager>();

    if (currentBrickWallGenerator == null)
      currentBrickWallGenerator = brickWallManager.GetBrickWallGenerator();

    brickWallGenerators.AddRange(GetComponentsInChildren<BrickWallGenerator>());
  }

  void SetBrickWallGenerator()
  {
    brickWallManager.SetBrickWallGenerator(currentBrickWallGenerator);
  }
}
