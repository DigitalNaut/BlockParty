using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class BrickWallGenerator : MonoBehaviour
{
  public Renderer[] Bricks;

  public int Columns = 0;
  public int Rows = 0;
  public Vector2 Padding;
  [ReadOnly] public List<Vector3> Positions;
  public BallManager ballManager;

  void Start() => BuildWall();

  float ItemWidth { get => Bricks.Length > 0 && !!Bricks[0] ? Bricks[0].bounds.size.x + Padding.x : 0; }
  float ItemHeight { get => Bricks.Length > 0 && !!Bricks[0] ? Bricks[0].bounds.size.y + Padding.y : 0; }

  float WallWidth { get => ItemWidth * (Columns - 1); }
  float WallHeight { get => ItemHeight * (Rows - 1); }


  List<Vector3> CalculatePositions()
  {
    var positionsList = new List<Vector3>();

    if (Bricks.Length > 0)
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

  void BuildWall()
  {
    if (Positions?.Count == 0)
      Positions = CalculatePositions();

    GameObject newBrick;

    foreach (var position in Positions)
    {
      newBrick = GetRandomBrick();
      newBrick.transform.position = position;
      newBrick.transform.SetParent(transform);
      newBrick.SetActive(true);
    }
  }

  GameObject GetRandomBrick()
  {
    int randInt = UnityEngine.Random.Range(0, Bricks.Length);

    return Instantiate(Bricks[randInt].gameObject);
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y, Mathf.Epsilon), new Vector3(WallWidth + ItemWidth, WallHeight + ItemHeight, Mathf.Epsilon));
  }

  void OnDrawGizmosSelected()
  {
    foreach (var position in Positions)
    {
      Gizmos.color = Color.yellow;

      Gizmos.DrawWireCube(new Vector3(position.x, position.y, Mathf.Epsilon), new Vector3(ItemWidth, ItemHeight, Mathf.Epsilon));

      if (Bricks.Length > 0 && Bricks[0])
      {
        Renderer sample = Bricks[0];
        Mesh mesh = sample.GetComponent<MeshFilter>().sharedMesh;
        Gizmos.DrawWireMesh(mesh, position, sample.transform.rotation, sample.transform.localScale);
      }
    };
  }
}
