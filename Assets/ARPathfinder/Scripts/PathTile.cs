using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Cross,
    RoundCross,
    DoubleLeft,
    DoubleRight
}

public enum Gate
{
    Top,
    Bottom,
    Left,
    Right
}

public class PathTile
{
    public Gate gate1;
    public Gate gate2;
    // Drawn in Tile.cs from gate1 to gate2
    public List<Vector3> pathPoints;
    public Vector3 positionGate1;
    public Vector3 positionGate2;
    // TODO change to 0
    public const float pathHeight = 0f;

    public PathTile(Gate gate1, Gate gate2, Vector3 objPos, float size)
    {
        this.gate1 = gate1;
        this.gate2 = gate2;
        positionGate1 = GateToPosition(gate1, objPos, size);
        positionGate2 = GateToPosition(gate2, objPos, size);
        pathPoints = new List<Vector3>();
    }

    private Vector3 GateToPosition(Gate gate, Vector3 objPos, float size)
    {
        switch (gate)
        {
            case Gate.Top:
                return objPos + new Vector3(0, pathHeight, size / 2);
            case Gate.Bottom:
                return objPos + new Vector3(0, pathHeight, -size / 2);
            case Gate.Left:
                return objPos + new Vector3(-size / 2, pathHeight, 0);
            case Gate.Right:
                return objPos + new Vector3(size / 2, pathHeight, 0);
            default:
                return objPos;
        }
    }
}