// GridManager.cs
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [Min(0.1f)] public float cellSize = 1f;
    [Min(1)] public int width = 20;
    [Min(1)] public int height = 20;

    [Header("Origin")]
    [Tooltip("ON, origin = transform.position. OFF originOverride.")]
    public bool useTransformAsOrigin = true;
    public Vector3 originOverride = Vector3.zero;

    [Header("Debug")]
    public bool drawGizmos = true;
    public bool drawCellCenters = true;
    public bool drawGridLines = true;

    public BuildingType[][] GridMattrix;


    private static GridManager _instance;
    public static GridManager Instance => _instance;


    private void Start()
    {
        GridMattrix = new BuildingType[width][];
        for (int i = 0; i < width; i++)
        {
            GridMattrix[i] = new BuildingType[height];
            for (int j = 0; j < height; j++)
            {
                GridMattrix[i][j] = BuildingType.None;
            }
        }
        _instance = this;
    }


    public bool CheckPlaceBuilding(Vector2Int pos, Vector2Int size)
    {
        for (int x = pos.x; x < pos.x + size.x; x++)
        {
            for (int y = pos.y; y < pos.y + size.y; y++)
            {
                if (GridMattrix[x][y] != 0)
                    return false;
            }
        }
        return true;
    }

    public bool PlaceBuilding(Vector2Int pos, Vector2Int size, BuildingType buildingID)
    {

        if (!CheckPlaceBuilding(pos, size))
            return false;

        for (int x = pos.x; x < pos.x + size.x; x++)
        {
            for (int y = pos.y; y < pos.y + size.y; y++)
            {
                GridMattrix[x][y] = buildingID;
            }
        }

        return true;
    }

    public Vector2Int FindPlaceForBuilding(Vector2Int size)
    {

        for (int x = 0; x <= width - size.x; x++)
        {
            for (int y = 0; y <= height - size.y; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (CheckPlaceBuilding(pos, size))
                {
                    return pos;
                }
            }
        }

        return new Vector2Int(-1, -1);
    }


    public Vector3 Origin => useTransformAsOrigin ? transform.position : originOverride;


    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        Vector3 local = worldPos - Origin;

        int x = Mathf.FloorToInt(local.x / cellSize);
        int y = Mathf.FloorToInt(local.z / cellSize);

        return new Vector2Int(x, y);
    }

    public Vector3 CellToWorldCenter(Vector2Int cell, float y = 0f)
    {
        float worldX = Origin.x + (cell.x + 0.5f) * cellSize;
        float worldZ = Origin.z + (cell.y + 0.5f) * cellSize;
        return new Vector3(worldX, y, worldZ);
    }

    public Vector3 CellToWorldCenter(Vector2Int cell, Vector2Int Size)
    {
        float worldX = Origin.x + (cell.x + (Size.x * 0.5f)) * cellSize;
        float worldZ = Origin.z + (cell.y + (Size.y * 0.5f)) * cellSize;
        return new Vector3(worldX, Origin.y, worldZ);
    }

    public Vector3 CellToWorldCorner(Vector2Int cell, float y = 0f)
    {
        float worldX = Origin.x + cell.x * cellSize;
        float worldZ = Origin.z + cell.y * cellSize;
        return new Vector3(worldX, y, worldZ);
    }

    public bool IsInside(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;
    }

    public Vector2Int ClampCell(Vector2Int cell)
    {
        int x = Mathf.Clamp(cell.x, 0, width - 1);
        int y = Mathf.Clamp(cell.y, 0, height - 1);
        return new Vector2Int(x, y);
    }

    public bool IsWorldInside(Vector3 worldPos)
    {
        Vector2Int cell = WorldToCell(worldPos);
        return IsInside(cell);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Vector3 origin = Origin;

        Gizmos.matrix = Matrix4x4.identity;

        float totalW = width * cellSize;
        float totalH = height * cellSize;

        Gizmos.DrawWireCube(
            new Vector3(origin.x + totalW * 0.5f, origin.y, origin.z + totalH * 0.5f),
            new Vector3(totalW, 0f, totalH)
        );

        if (drawGridLines)
        {
            for (int x = 0; x <= width; x++)
            {
                Vector3 p1 = new Vector3(origin.x + x * cellSize, origin.y, origin.z);
                Vector3 p2 = new Vector3(origin.x + x * cellSize, origin.y, origin.z + totalH);
                Gizmos.DrawLine(p1, p2);
            }

            for (int y = 0; y <= height; y++)
            {
                Vector3 p1 = new Vector3(origin.x, origin.y, origin.z + y * cellSize);
                Vector3 p2 = new Vector3(origin.x + totalW, origin.y, origin.z + y * cellSize);
                Gizmos.DrawLine(p1, p2);
            }
        }

        if (drawCellCenters)
        {
            float dot = Mathf.Max(0.05f, cellSize * 0.08f);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 c = CellToWorldCenter(new Vector2Int(x, y), origin.y);
                    Gizmos.DrawSphere(c, dot);
                }
            }
        }
    }

 
}
