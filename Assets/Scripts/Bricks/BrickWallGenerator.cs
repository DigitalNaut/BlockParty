using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class BrickWallGenerator : MonoBehaviour
{
  [Header("Prefabs")]
  [SerializeField] Renderer[] BrickPrefabs = new Renderer[0];
  [Header("Settings")]
  [SerializeField] int Columns = 16;
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

  void Awake() => Debug.Assert(BrickPrefabs.Length > 0, "BrickPrefabs is empty", transform);

  public List<Breakable> BuildBreakablesWall()
  {
    Debug.Log("Building Breakables Wall");

    if (Positions?.Count == 0)
      Positions = CalculatePositions();

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

  List<Vector3> CalculatePositions()
  {
    var positionsList = new List<Vector3>();

    if (BrickPrefabs.Length > 0)
    {
      for (int w = 0; w < Columns; w++)
      {
        for (int z = 0; z < Rows; z++)
        {
          Vector2 local = transform.TransformPoint(w, z, 0);
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
    var indexedPositions = CalculatePositions().Select((position, index) => (index, position));

    foreach (var (index, position) in indexedPositions)
    {
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
