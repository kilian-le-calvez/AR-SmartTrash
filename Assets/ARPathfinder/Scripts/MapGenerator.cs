using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MapGenerator : MonoBehaviour
{
    public Tile[] tilesModel;

    public static List<Tile> _map = new List<Tile>();
    public static List<Tile> _mapTilesNotInstantied = new List<Tile>();
    public static List<Tuple<Vector3, Tile>> _mapTilesInfo = new List<Tuple<Vector3, Tile>>();

    private Vector3[] _tilesPos;
    public float _width = 1;

    public bool isDevMode = false;

    GameObject target;

    public void OnDestroy()
    {
        foreach (Tile tile in _map)
        {
            tile.OnDelete();
            Destroy(tile.gameObject);
        }
        // free all tiles of map
        _map.Clear();
        _tilesPos = new Vector3[0];
        _mapTilesNotInstantied.Clear();
        _mapTilesInfo.Clear();
    }

    public void OrderTilesMapNotInstantied()
    {
        // Create a new list to hold the screen positions and corresponding tiles
        List<Tuple<Vector3, Tile>> screenTilesInfo = new List<Tuple<Vector3, Tile>>();

        foreach (var tuple in _mapTilesInfo)
        {
            // TextDebug.SetTextDebug($"{tuple.Item1.x:F2}, {tuple.Item1.y:F2}");

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(tuple.Item1);
            screenTilesInfo.Add(new Tuple<Vector3, Tile>(screenPosition, tuple.Item2));
        }

        // First, sort the list by the y-coordinate (screen space)
        screenTilesInfo = screenTilesInfo
            .OrderBy(tuple => tuple.Item1.y)
            .ToList();

        // Create a new list to hold the final sorted result
        List<Tuple<Vector3, Tile>> sortedTilesInfo = new List<Tuple<Vector3, Tile>>();

        // Iterate through the sorted list in groups of three
        for (int i = 0; i < screenTilesInfo.Count; i += 3)
        {
            var group = screenTilesInfo.Skip(i).Take(3).OrderBy(tuple => tuple.Item1.x).ToList(); // Sort each group of 3 by x-coordinate
            sortedTilesInfo.AddRange(group); // Add the sorted group to the final result
        }

        // Replace the original list with the sorted one
        _mapTilesInfo = sortedTilesInfo
            .Select(tuple => new Tuple<Vector3, Tile>(tuple.Item1, tuple.Item2)) // Convert back to world positions
            .ToList();

        TextDebug.SetTextDebug("OrderTilesMapNotInstantied");
    }

    public void Init(GameObject target)
    {
        this.target = target;
        OrderTilesMapNotInstantied();
        GenerateTilesPos();
        GenerateMap();
    }

    void GenerateMap()
    {
        // TextDebug.SetTextDebug("GenerateMap " + _mapTilesNotInstantied.Count());
        for (int x = 0; x < _mapTilesInfo.Count; x++)
        {
            if (x >= _tilesPos.Length)
            {
                TextDebug.SetTextDebug("Tile pos not existing " + x + " " + _tilesPos.Length);
                break;
            }

            // For randomize map
            // int randomIndex = Random.Range(0, tilesModel.Length);
            // For predefined map - generate from left to right, bot to top
            Tile instantiatedTile = Instantiate(_mapTilesInfo[x].Item2, Vector3.zero, target.transform.rotation, parent: target.transform);
            // TextDebug.SetTextDebug($"{_mapTilesInfo[x].Item2.name} {_mapTilesInfo[x].Item1.x:F2}, {_mapTilesInfo[x].Item1.z:F2}");

            // Set the local position and other properties
            instantiatedTile.transform.localPosition = _tilesPos[x];
            instantiatedTile.SetSize(_width);
            instantiatedTile.target = target;
            instantiatedTile.name = "Tile_" + x;
            instantiatedTile.SetDevMode(isDevMode);
            instantiatedTile.OnStart();

            // Add the tile to the _map list
            if (_map.Count > x)
            {
                _map[x] = instantiatedTile;
            }
            else
            {
                _map.Add(instantiatedTile);
            }
        }
    }

    // Gen visual tiles without the path
    void GenerateTilesPos()
    {
        _tilesPos = new Vector3[_mapTilesInfo.Count]; // Initialize the _tilesPos array with the correct size

        int _row = _mapTilesInfo.Count / 3;
        int _col = _mapTilesInfo.Count / 3;

        float j = -1f;
        // Change dynamically the number of rows and columns
        for (int x = 0; x < _row; x++)
        {
            float k = -1f;
            for (int y = 0; y < _col; y++)
            {
                _tilesPos[x * _row + y] = new Vector3(k * _width * 10, 0, j * _width * 10);
                k += 1;
            }
            j += 1;
        }
    }

    public List<Tile> GetMap()  // Getter for the _map array
    {
        return _map;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
