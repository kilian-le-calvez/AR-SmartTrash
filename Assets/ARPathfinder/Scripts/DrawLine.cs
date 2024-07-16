using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class DrawLine
{
    private int _precision;
    private string _nameObj;
    public LineRenderer _lineRenderer;
    private Transform _parent;
    public float smallDistance = 0.04f;
    public float mediumDistance = 0.06f;
    public float lineWidth = 0.01f;

    public DrawLine(string nameObj, Transform parentObj)
    {
        _precision = 30;
        _nameObj = nameObj;
        _parent = parentObj;
    }

    LineRenderer NewLineRenderer()
    {
        GameObject lineObj = new(_nameObj);
        lineObj.transform.SetParent(_parent);

        LineRenderer pathLineRenderer = lineObj.AddComponent<LineRenderer>();

        // pathLineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply")); // Example material
        pathLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        pathLineRenderer.startWidth = pathLineRenderer.endWidth = lineWidth;
        pathLineRenderer.startColor = pathLineRenderer.endColor = Color.red;
        return pathLineRenderer;
    }

    public void DrawRedLine(List<Vector3> localLinePoints)
    {
        _lineRenderer = NewLineRenderer();
        _lineRenderer.positionCount = localLinePoints.Count;

        for (int i = 0; i < localLinePoints.Count; i++)
        {
            Vector3 worldPos = _parent.transform.TransformPoint(localLinePoints[i]);
            _lineRenderer.SetPosition(i, worldPos);
        }
    }

    public Vector3[] GenerateStraightLine(Vector3 startPos, Vector3 endPos)
    {
        Vector3[] linePoints = new Vector3[_precision];

        for (int i = 0; i < _precision; i++)
        {
            float t = i / (float)(_precision - 1); // Normalized position along the line
            Vector3 localPoint = Vector3.Lerp(startPos, endPos, t); // Linear interpolation between startPos and endPos
            linePoints[i] = localPoint;
        }
        // DrawRedLine(linePoints.ToList());
        return linePoints;
    }

    // The draw function draw points in world space and return points in local space
    public Vector3[] GenerateArc(Vector3 startPos, Vector3 endPos, float radius, Gate gate, TileType type)
    {
        Vector3[] localArcPoints = new Vector3[_precision];

        Vector3 midpoint = (startPos + endPos) / 2f;
        Vector3 normal = Vector3.Cross(endPos - startPos, Vector3.up).normalized; // Use Vector3.up for XZ plane
        Vector3 center = Vector3.zero;

        if (type == TileType.DoubleRight)
        {
            if (gate == Gate.Top)
                center = midpoint + normal * Mathf.Sqrt(radius * radius - (Vector3.Distance(startPos, midpoint) * Vector3.Distance(startPos, midpoint)));
            else
                center = midpoint - normal * Mathf.Sqrt(radius * radius - (Vector3.Distance(startPos, midpoint) * Vector3.Distance(startPos, midpoint)));
        }

        if (type == TileType.DoubleLeft)
        {
            if (gate == Gate.Bottom)
                center = midpoint + normal * Mathf.Sqrt(radius * radius - (Vector3.Distance(startPos, midpoint) * Vector3.Distance(startPos, midpoint)));
            else
                center = midpoint - normal * Mathf.Sqrt(radius * radius - (Vector3.Distance(startPos, midpoint) * Vector3.Distance(startPos, midpoint)));
        }

        float startAngle = Mathf.Atan2(startPos.z - center.z, startPos.x - center.x) * Mathf.Rad2Deg;
        float endAngle = Mathf.Atan2(endPos.z - center.z, endPos.x - center.x) * Mathf.Rad2Deg;

        // Ensure the arc always goes the shortest way around the circle
        if (Mathf.Abs(endAngle - startAngle) > 180)
        {
            if (endAngle > startAngle)
                endAngle -= 360;
            else
                startAngle -= 360;
        }

        // Interpolate points along the arc on the XZ plane
        for (int i = 0; i < _precision; i++)
        {
            float t = i / (float)(_precision - 1);
            float angle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;
            Vector3 localPoint = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius + center; // Z component for up direction
            localArcPoints[i] = localPoint;
        }
        // DrawRedLine(localArcPoints.ToList());
        return localArcPoints;
    }

    public Vector3[] GenerateComplexPath(Vector3 startPos, Vector3 endPos, PathTile path, float size)
    {
        List<Vector3> complexPoints = new()
    {
        startPos
    };
        Vector3 currentPos = startPos;

        // Determine the path based on gate positions
        if (path.gate1 == Gate.Bottom && path.gate2 == Gate.Left)
        {
            currentPos.z += smallDistance;
            complexPoints.Add(currentPos);
            currentPos.x -= mediumDistance;
            complexPoints.Add(currentPos);
            currentPos.z += mediumDistance;
            complexPoints.Add(currentPos);
            currentPos.x -= smallDistance;
            complexPoints.Add(currentPos);
        }
        else if (path.gate1 == Gate.Bottom && path.gate2 == Gate.Right)
        {
            currentPos.z += smallDistance;
            complexPoints.Add(currentPos);
            currentPos.x += mediumDistance;
            complexPoints.Add(currentPos);
            currentPos.z += mediumDistance;
            complexPoints.Add(currentPos);
            currentPos.x += smallDistance;
            complexPoints.Add(currentPos);
        }
        else if (path.gate1 == Gate.Left && path.gate2 == Gate.Top)
        {
            currentPos.x += smallDistance;
            complexPoints.Add(currentPos);
            currentPos.z += mediumDistance;
            complexPoints.Add(currentPos);
            currentPos.x += mediumDistance;
            complexPoints.Add(currentPos);
            currentPos.z += smallDistance;
            complexPoints.Add(currentPos);
        }
        else if (path.gate1 == Gate.Right && path.gate2 == Gate.Top)
        {
            currentPos.x -= smallDistance;
            complexPoints.Add(currentPos);
            currentPos.z += mediumDistance;
            complexPoints.Add(currentPos);
            currentPos.x -= mediumDistance;
            complexPoints.Add(currentPos);
            currentPos.z += smallDistance;
            complexPoints.Add(currentPos);
        }
        else
        {
            // Default straight path if gates are not handled
            complexPoints.Add(endPos);
        }

        // Ensure the final position is the end position
        complexPoints.Add(endPos);

        // Calculate the total length of the path
        float totalLength = 0f;
        for (int i = 0; i < complexPoints.Count - 1; i++)
        {
            totalLength += Vector3.Distance(complexPoints[i], complexPoints[i + 1]);
        }

        // Calculate the distance between each interpolated point
        float stepLength = totalLength / (_precision - 1);

        // Create the final path with the desired number of points
        List<Vector3> finalPath = new List<Vector3>
    {
        startPos
    };

        float remainingLength = stepLength;
        for (int i = 0; i < complexPoints.Count - 1; i++)
        {
            Vector3 start = complexPoints[i];
            Vector3 end = complexPoints[i + 1];
            float segmentLength = Vector3.Distance(start, end);

            while (segmentLength >= remainingLength)
            {
                float t = remainingLength / segmentLength;
                Vector3 interpolatedPoint = Vector3.Lerp(start, end, t);
                finalPath.Add(interpolatedPoint);

                start = interpolatedPoint;
                segmentLength -= remainingLength;
                remainingLength = stepLength;
            }

            remainingLength -= segmentLength;
        }

        // Ensure the final position is the end position
        if (finalPath.Count < _precision)
        {
            finalPath.Add(endPos);
        }
        // DrawRedLine(finalPath);
        return finalPath.ToArray();
    }



}
