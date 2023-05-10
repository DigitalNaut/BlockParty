using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[Icon("Assets/Textures/Script Icons/TrowelBrickWall.png")]
public class BrickWallGenerator : MonoBehaviour
{
  [Header("Prefabs")]
  [SerializeField] Renderer[] BrickPrefabs;

  [Header("Settings")]
  [OnValueChanged("CalculatePositions")]
  [SerializeField] int Columns = 16;
  [OnValueChanged("CalculatePositions")]
  [SerializeField] int Rows = 9;
  [OnValueChanged("CalculatePositions")]
  [SerializeField] Vector2 Padding = new Vector2(0.6f, 0.6f);

  [Header("Debug")]
  [SerializeField] bool debug = false;
  [ReadOnly] public List<Vector3> Positions;
  [Button("Make Brick Manager's Wall Generator", EButtonEnableMode.Editor)]
  [DisableIf("IsBrickManagerWallGenerator")]
  void MakeBrickManagerWallGenerator()
  {
    if (brickManager)
    {
      gameObject.SetActive(true);
      brickManager.SetBrickWallGenerator(this);
    }
    else Debug.LogWarning("No Brick Manager found in scene");
  }
  bool IsBrickManagerWallGenerator() => brickManager.GetBrickWallGenerator() == this;

  BrickWallManager brickManager;
  Debugger debugger;

  float ItemWidth { get => BrickPrefabs.Length > 0 && !!BrickPrefabs[0] ? BrickPrefabs[0].bounds.size.x + Padding.x : 0; }
  float ItemHeight { get => BrickPrefabs.Length > 0 && !!BrickPrefabs[0] ? BrickPrefabs[0].bounds.size.y + Padding.y : 0; }

  float WallWidth { get => ItemWidth * (Columns - 1); }
  float WallHeight { get => ItemHeight * (Rows - 1); }

  void Awake() => Debug.Assert(BrickPrefabs.Length > 0, "BrickPrefabs is empty");

  void OnValidate()
  {
    brickManager = FindObjectOfType<BrickWallManager>();
    if (debug) debugger = new Debugger();
  }

  void CalculatePositions() => Positions = CalculateGrid();

  public List<Breakable> BuildBreakablesWall()
  {
    CalculatePositions();

    if (Positions?.Count == 0)
      Positions = CalculateGrid();

    GameObject newBrick;

    List<Breakable> newBreakables = new List<Breakable>();

    foreach (var position in Positions)
    {
      newBrick = GetRandomBrick();
      newBrick.transform.position = transform.TransformPoint(position);
      newBrick.transform.SetParent(transform);
      newBrick.SetActive(true);

      Breakable newBreakable = newBrick.GetComponent<Breakable>();
      if (newBreakable) newBreakables.Add(newBreakable);
    }

    if (debug) debugger.LogBrickCount(newBreakables.Count);

    return newBreakables;
  }

  [Button("Recalculate Grid")]
  List<Vector3> CalculateGrid()
  {
    var positionsList = new List<Vector3>();

    if (BrickPrefabs.Length > 0)
    {
      for (int col = 0; col < Columns; col++)
      {
        for (int row = 0; row < Rows; row++)
        {
          Vector2 local = new Vector3(col, row, 0);
          Vector3 position = new Vector3(
              local.x * ItemWidth - WallWidth * 0.5f,
              local.y * ItemHeight - WallHeight * 0.5f,
              transform.position.z);

          positionsList.Add(position);
        }
      }
    }

    if (debug) debugger.LogGeneration(positionsList.Count);

    return positionsList;
  }

  GameObject GetRandomBrick()
  {
    int randInt = Random.Range(0, BrickPrefabs.Length);
    return Instantiate(BrickPrefabs[randInt].gameObject);
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y, Mathf.Epsilon), new Vector3(WallWidth + ItemWidth, WallHeight + ItemHeight, Mathf.Epsilon));
  }

  void OnDrawGizmosSelected()
  {
    if (BrickPrefabs.Length == 0) return;

    Color meshColor;
    Renderer sample;
    Mesh mesh;

    for (int index = 0; index < Positions.Count; index++)
    {
      Vector3 position = Positions[index];

      sample = BrickPrefabs[index % BrickPrefabs.Length];
      if (!sample) continue;

      mesh = sample.GetComponent<MeshFilter>().sharedMesh;
      meshColor = sample.sharedMaterial.color;
      meshColor.a = 0.1f;

      Gizmos.color = meshColor;
      Gizmos.DrawWireMesh(mesh, transform.TransformPoint(position), sample.transform.rotation, sample.transform.localScale);
    }
  }

  class Debugger
  {
    public void LogGeneration(int count) => Debug.Log($"Calculated {count} positions.");
    public void LogBrickCount(int count) => Debug.Log($"Generated {count} bricks.");
  }
}

