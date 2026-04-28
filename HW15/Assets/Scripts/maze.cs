using System;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [Header("Maze size (cells)")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    [Header("Wall dimensions")]
    [SerializeField] private float wallThickness = 0.1f;
    [SerializeField] private float wallHeight = 1.0f;
    [SerializeField] private float wallLength = 0.9f;

    [Header("Placement")]
    [Tooltip("Cell size in world units. For a 10x10 Plane scaled so it spans 10 units, keep this as 1.")]
    [SerializeField] private float cellSize = 1.0f;

    [Tooltip("If set, instantiated walls will be parented under this Transform.")]
    [SerializeField] private Transform wallParent;

    [Tooltip("Flip the ASCII rows so the first line is at the top (+Z) in world.")]
    [SerializeField] private bool flipRows = true;

    [Tooltip("Prefab used for vertical '|' walls (optional). If not set, cubes will be used.")]
    [SerializeField] private GameObject wallPrefabV;

    [Tooltip("Prefab used for horizontal '-' walls (optional). If not set, cubes will be used.")]
    [SerializeField] private GameObject wallPrefabH;

    [Tooltip("Material used for walls when using cubes (optional).")]
    [SerializeField] private Material wallMaterial;

    [TextArea(10, 40)]
    [SerializeField]
    private string asciiMaze =
@"0|00000|0000|
-0---0--0-
00|00|0000|00|
----0-----
00|00|00|0000|
00----00--
000|000|00|00|
------0000
|00|0000|0000|
|0000|000|000|
00|00|0000|00|
----0-----
00|00|00|0000|
00---000--
000|000|00|00|
----0000--
|00|0000|0000|
----------";

    private readonly List<GameObject> _spawned = new List<GameObject>();

    private void Reset()
    {
        wallParent = transform;
    }

    private void Start()
    {
        BuildFromAscii(asciiMaze);
    }

    [ContextMenu("Rebuild Maze")]
    public void Rebuild()
    {
        BuildFromAscii(asciiMaze);
    }

    private void BuildFromAscii(string input)
    {
        ClearSpawned();

        if (string.IsNullOrWhiteSpace(input))
        {
            Debug.LogWarning("asciiMaze is empty.");
            return;
        }

        var lines = NormalizeLines(input);
        if (lines.Count == 0)
        {
            Debug.LogWarning("asciiMaze has no valid lines.");
            return;
        }

        int currentRow = 0;
        int currentBoundary = 1;

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];

            bool hasVertical = line.IndexOf('|') >= 0;
            bool hasHorizontal = line.IndexOf('-') >= 0;

            if (hasVertical)
            {
                if (currentRow >= height)
                    continue;

                ParseVerticalWalls(line, currentRow);
                currentRow++;
                currentBoundary = currentRow;
            }
            else if (hasHorizontal)
            {
                if (currentBoundary >= 0 && currentBoundary <= height)
                    ParseHorizontalWalls(line, currentBoundary);
            }
        }
    }

    private static List<string> NormalizeLines(string input)
    {
        var list = new List<string>();
        var raw = input.Replace("\r", string.Empty).Split('\n');
        foreach (var s in raw)
        {
            var t = s.TrimEnd();
            if (!string.IsNullOrWhiteSpace(t))
                list.Add(t);
        }
        return list;
    }

    private int MapRow(int row)
    {
        return flipRows ? (height - 1 - row) : row;
    }

    private int MapBoundaryRow(int boundaryRow)
    {
        return flipRows ? (height - boundaryRow) : boundaryRow;
    }

    private static bool LooksLikeBoundaryVLine(string line)
    {

        return line.TrimEnd().EndsWith("|", StringComparison.Ordinal);
    }

    private void ParseVerticalWalls(string line, int row)
    {

        int cellIndex = 0;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '0')
            {
                cellIndex++;
            }
            else if (c == '|')
            {
                int boundaryX = cellIndex;
                if (boundaryX > 0 && boundaryX < width)
                    SpawnVerticalWall(boundaryX, row);
            }
        }

        if (LooksLikeBoundaryVLine(line))
        {

            int boundaryX = cellIndex;
            if (boundaryX == width)
                SpawnVerticalWall(width, row);
        }
    }

    private void ParseHorizontalWalls(string line, int boundaryRow)
    {
        int col = 0;
        for (int i = 0; i < line.Length && col < width; i++)
        {
            char c = line[i];
            if (c == '-')
            {
                SpawnHorizontalWall(col, boundaryRow);
                col++;
            }
            else if (c == '0')
            {
                col++;
            }
        }
    }

    private Transform GetParentOrSelf()
    {
        return wallParent != null ? wallParent : transform;
    }

    private Vector3 GetMazeLocalOriginCentered()
    {
        float halfW = width * cellSize * 0.5f;
        float halfH = height * cellSize * 0.5f;
        return new Vector3(-halfW, 0f, -halfH);
    }

    private GameObject CreateWallGO(bool vertical)
    {
        GameObject prefab = vertical ? wallPrefabV : wallPrefabH;
        if (prefab != null)
            return Instantiate(prefab);

        return GameObject.CreatePrimitive(PrimitiveType.Cube);
    }

    private void SpawnVerticalWall(int boundaryX, int row)
    {
        var parent = GetParentOrSelf();
        var wall = CreateWallGO(vertical: true);
        wall.name = $"VWall_{boundaryX}_{row}";

        wall.transform.SetParent(parent, worldPositionStays: false);

        int r = MapRow(row);
        Vector3 origin = GetMazeLocalOriginCentered();
        float x = origin.x + boundaryX * cellSize;
        float z = origin.z + (r + 0.5f) * cellSize;
        float y = wallHeight * 0.5f;

        wall.transform.localPosition = new Vector3(x, y, z);
        wall.transform.localRotation = Quaternion.identity;
        wall.transform.localScale = new Vector3(wallThickness, wallHeight, wallLength);

        ApplyMaterialIfNeeded(wall);
        _spawned.Add(wall);
    }

    private void SpawnHorizontalWall(int col, int boundaryRow)
    {
        var parent = GetParentOrSelf();
        var wall = CreateWallGO(vertical: false);
        wall.name = $"HWall_{col}_{boundaryRow}";

        wall.transform.SetParent(parent, worldPositionStays: false);

        int br = MapBoundaryRow(boundaryRow);
        Vector3 origin = GetMazeLocalOriginCentered();
        float x = origin.x + (col + 0.5f) * cellSize;
        float z = origin.z + br * cellSize;
        float y = wallHeight * 0.5f;

        wall.transform.localPosition = new Vector3(x, y, z);
        wall.transform.localRotation = Quaternion.identity;
        wall.transform.localScale = new Vector3(wallLength, wallHeight, wallThickness);

        ApplyMaterialIfNeeded(wall);
        _spawned.Add(wall);
    }

    private void ApplyMaterialIfNeeded(GameObject wall)
    {
        if (wallMaterial == null)
            return;

        var renderer = wall.GetComponent<Renderer>();
        if (renderer != null)
            renderer.sharedMaterial = wallMaterial;
    }

    private void ClearSpawned()
    {
        for (int i = _spawned.Count - 1; i >= 0; i--)
        {
            if (_spawned[i] != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(_spawned[i]);
                else
#endif
                    Destroy(_spawned[i]);
            }
        }
        _spawned.Clear();

        var parent = GetParentOrSelf();
        if (parent != null)
        {
            var toRemove = new List<Transform>();
            foreach (Transform child in parent)
            {
                if (child != null &&
                    (child.name.StartsWith("VWall_", StringComparison.Ordinal) || child.name.StartsWith("HWall_", StringComparison.Ordinal)))
                    toRemove.Add(child);
            }

            foreach (var t in toRemove)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(t.gameObject);
                else
#endif
                    Destroy(t.gameObject);
            }
        }
    }
}
