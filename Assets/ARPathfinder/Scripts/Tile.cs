using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType type;
    private List<PathTile> paths = new();
    public float size = 1f;

    public GameObject target;

    public List<DrawLine> listDrawers = new();

    public bool isDevMode = false;

    public void SetSize(float _size)
    {
        transform.localScale = new Vector3(_size, _size, _size);
        size = _size * 10;
    }

    public void OnDelete()
    {
        foreach (DrawLine drawer in listDrawers)
        {
            if (drawer._lineRenderer != null)
            {
                Destroy(drawer._lineRenderer.gameObject);
            }
        }
        listDrawers.Clear();
    }

    public void SetDevMode(bool devMode)
    {
        isDevMode = devMode;
        gameObject.SetActive(isDevMode);
    }

    public void OnStart()
    {
        AddPaths();
        DrawPaths();
    }

    public PathTile GetPathFromPosition(Vector3 position1, Vector3 position2)
    {
        foreach (PathTile path in paths)
        {
            if (path.pathPoints != null)
            {
                if (path.positionGate1 == position1 && path.positionGate2 == position2)
                {
                    return path;
                }
                else if (path.positionGate2 == position1 && path.positionGate1 == position2)
                {
                    return path;
                }
            }
        }
        return null;
    }

    public List<PathTile> GetPaths()
    {
        return paths;
    }

    void AddPaths()
    {
        if (type == TileType.DoubleLeft || type == TileType.RoundCross || type == TileType.Cross)
        {
            paths.Add(new PathTile(Gate.Bottom, Gate.Left, transform.localPosition, size));
            paths.Add(new PathTile(Gate.Right, Gate.Top, transform.localPosition, size));
        }
        if (type == TileType.DoubleRight || type == TileType.RoundCross || type == TileType.Cross)
        {
            paths.Add(new PathTile(Gate.Bottom, Gate.Right, transform.localPosition, size));
            paths.Add(new PathTile(Gate.Left, Gate.Top, transform.localPosition, size));
        }
        if (type == TileType.RoundCross || type == TileType.Cross)
        {
            paths.Add(new PathTile(Gate.Bottom, Gate.Top, transform.localPosition, size));
            paths.Add(new PathTile(Gate.Left, Gate.Right, transform.localPosition, size));
        }
    }

    void DrawPathByTypeGate(Vector3 startPos, Vector3 endPos, Gate gate, PathTile path)
    {
        DrawLine drawer = new DrawLine("PathLine : " + path.gate1 + " - " + path.gate2, target.transform);
        listDrawers.Add(drawer);
        if (type == TileType.DoubleLeft || type == TileType.DoubleRight)
            path.pathPoints = drawer.GenerateArc(startPos, endPos, size / 2.25f, gate, type).ToList();
        else if (type == TileType.RoundCross &&
        ((path.gate1 == Gate.Bottom && path.gate2 == Gate.Right)
        || (path.gate1 == Gate.Bottom && path.gate2 == Gate.Left)
        || (path.gate1 == Gate.Left && path.gate2 == Gate.Top)
        || (path.gate1 == Gate.Right && path.gate2 == Gate.Top)
        ))
            path.pathPoints = drawer.GenerateComplexPath(startPos, endPos, path, size).ToList();
        // Handles the rest of the cases
        else
            path.pathPoints = drawer.GenerateStraightLine(startPos, endPos).ToList();
    }

    void DrawPaths()
    {
        foreach (PathTile path in paths)
        {
            // LineRenderer lineRenderer = NewLineRenderer(path);

            Vector3 startPos = path.positionGate1;
            Vector3 endPos = path.positionGate2;

            if (path.gate1 == Gate.Top || path.gate1 == Gate.Bottom)
                DrawPathByTypeGate(startPos, endPos, path.gate1, path);
            else
                DrawPathByTypeGate(startPos, endPos, path.gate2, path);
        }
    }
}
