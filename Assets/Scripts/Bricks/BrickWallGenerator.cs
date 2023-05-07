using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[Icon("Assets/Textures/Icons/TrowelBrickWallIcon.png")]
public class BrickWallGenerator : MonoBehaviour
{
  [Header("Prefabs")]
  [SerializeField] Renderer[] BrickPrefabs;

  [Header("Settings")]
  [OnValueChanged("CalculatePositions")]
  [SerializeField] int Columns = 16;
  [OnValueChanged("CalculatePositions")]
  [SerializeField] int Rows = 9;
  [SerializeField] Vector2 Padding = new Vector2(0.6f, 0.6f);

  [Header("Debug")]
  [ReadOnly] public List<Vector3> Positions;
  [Button("Make Brick Manager's Wall Generator", EButtonEnableMode.Editor)]
  void MakeBrickManagerWallGenerator()
  {
    var brickManager = FindObjectOfType<BrickManager>();
    if (brickManager) brickManager.SetBrickWallGenerator(this);
  }

  float ItemWidth { get => BrickPrefabs.Length > 0 && !!BrickPrefabs[0] ? BrickPrefabs[0].bounds.size.x + Padding.x : 0; }
  float ItemHeight { get => BrickPrefabs.Length > 0 && !!BrickPrefabs[0] ? BrickPrefabs[0].bounds.size.y + Padding.y : 0; }

  float WallWidth { get => ItemWidth * (Columns - 1); }
  float WallHeight { get => ItemHeight * (Rows - 1); }

  void Awake() => Debug.Assert(BrickPrefabs.Length > 0, "BrickPrefabs is empty");

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
      newBrick.transform.position = position;
      newBrick.transform.SetParent(transform);
      newBrick.SetActive(true);

      Breakable newBreakable = newBrick.GetComponent<Breakable>();
      if (newBreakable) newBreakables.Add(newBreakable);
    }

    Debug.Log($"Built {newBreakables.Count} Breakables");

    return newBreakables;
  }

  List<Vector3> CalculateGrid()
  {
    Debug.Log("Calculating Grid");

    var positionsList = new List<Vector3>();

    if (BrickPrefabs.Length > 0)
    {
      for (int col = 0; col < Columns; col++)
      {
        for (int row = 0; row < Rows; row++)
        {
          Vector2 local = transform.TransformPoint(col, row, 0);
          Vector3 position = new Vector3(
              local.x * ItemWidth - WallWidth * 0.5f,
              local.y * ItemHeight - WallHeight * 0.5f,
              transform.position.z);

          positionsList.Add(position);
        }
      }
    }

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
      Gizmos.DrawWireMesh(mesh, position, sample.transform.rotation, sample.transform.localScale);
    }

  }
}
