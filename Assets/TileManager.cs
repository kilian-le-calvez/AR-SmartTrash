using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    private Dictionary<string, Vector3> detectedTiles = new Dictionary<string, Vector3>();
    private Dictionary<string, GameObject> rectangles = new Dictionary<string, GameObject>(); // Store rectangles

    public Transform target; // Assign the target in the Inspector

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void DrawRectangleDetection(string tileType, Vector3 position, Transform targetTransform, float width, float height)
    {
        // Create a new GameObject for the rectangle
        GameObject rectangle = new GameObject("Rectangle_" + tileType);
        rectangle.transform.position = position;

        // Add LineRenderer component
        LineRenderer lineRenderer = rectangle.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 5; // 4 corners + 1 to close the rectangle
        lineRenderer.widthMultiplier = 0.1f; // Adjust width as needed
        lineRenderer.useWorldSpace = true; // Use world space
        lineRenderer.loop = true; // Ensure the rectangle closes

        // Create a simple material for the LineRenderer
        Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lineMaterial.color = Color.red; // Set the color of the line
        lineRenderer.material = lineMaterial;

        // Set the positions of the corners of the rectangle
        Vector3[] positions = new Vector3[5];
        positions[0] = new Vector3(-width / 2, -height / 2, 0);
        positions[1] = new Vector3(width / 2, -height / 2, 0);
        positions[2] = new Vector3(width / 2, height / 2, 0);
        positions[3] = new Vector3(-width / 2, height / 2, 0);
        positions[4] = positions[0]; // Close the rectangle

        // Convert local positions to world positions
        Vector3[] worldPositions = new Vector3[5];
        for (int i = 0; i < positions.Length; i++)
        {
            worldPositions[i] = targetTransform.TransformPoint(positions[i]);
        }

        lineRenderer.SetPositions(worldPositions);

        // Parent the rectangle to the detected tile's GameObject for correct positioning
        rectangle.transform.SetParent(targetTransform, false);
        rectangle.transform.localPosition = Vector3.zero;
        rectangle.transform.localRotation = Quaternion.identity;

        // Store the rectangle
        rectangles[tileType] = rectangle;
    }

    private IEnumerator WaitAndAddTile(string tileType, Vector3 position, Transform targetTransform, Tile tileModel)
    {
        // Wait for the end of the frame to ensure everything is initialized
        yield return new WaitForEndOfFrame();

        if (!detectedTiles.ContainsKey(tileType))
        {
            // Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
            // TextDebug.SetTextDebug($"{tileType} {screenPosition.x:F2}, {screenPosition.y:F2}, {screenPosition.z:F2}");

            Tuple<Vector3, Tile> tileInfo = new Tuple<Vector3, Tile>(position, tileModel);
            detectedTiles.Add(tileType, position);

            MapGenerator._mapTilesNotInstantied.Add(tileModel);
            MapGenerator._mapTilesInfo.Add(tileInfo);

            if (MapGenerator._mapTilesNotInstantied.Count == 9)
            {
                TextDebug.SetTextDebug("Ready to generate map");
            }
        }
    }

    public void AddTile(string tileType, Vector3 position, Transform targetTransform, Tile tileModel)
    {
        StartCoroutine(WaitAndAddTile(tileType, position, targetTransform, tileModel));
    }

    private void GenerateTile(string tileType, Vector3 position, Transform targetTransform, float width, float height)
    {
        // Implement your tile generation logic here
        DrawRectangleDetection(tileType, position, targetTransform, width, height);
    }

    public void RemoveTile(string tileType, Tile tileModel, Vector3 position)
    {
        if (detectedTiles.ContainsKey(tileType))
        {
            detectedTiles.Remove(tileType);
            Tuple<Vector3, Tile> tileInfo = new Tuple<Vector3, Tile>(position, tileModel);

            rectangles.Remove(tileType);
            MapGenerator._mapTilesNotInstantied.Remove(tileModel);
            MapGenerator._mapTilesInfo.Remove(tileInfo);

            // Destroy the rectangle if it exists
            if (rectangles.ContainsKey(tileType))
            {
                Destroy(rectangles[tileType]);
            }
            if (MapGenerator._mapTilesNotInstantied.Count == 0)
            {
                TextDebug.SetTextDebug("Remove tiles completed");
            }
        }
    }

    public Vector3 CalculateCenter()
    {
        if (detectedTiles.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 sum = Vector3.zero;
        foreach (var tilePosition in detectedTiles.Values)
        {
            sum += tilePosition;
        }

        Vector3 center = sum / detectedTiles.Count;
        return center;
    }

    public string FindTileClosestToCenter()
    {
        Vector3 center = CalculateCenter();
        string closestTile = null;
        float closestDistance = float.MaxValue;

        foreach (var kvp in detectedTiles)
        {
            float distance = Vector3.Distance(center, kvp.Value);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = kvp.Key;
            }
        }

        Debug.Log("Tile closest to center: " + closestTile);
        return closestTile;
    }

    // void Update()
    // {
    //     if (target != null && Camera.main != null)
    //     {
    //         // Get the world position of the target
    //         Vector3 worldPosition = target.position;

    //         // Convert the world position to screen space
    //         Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

    //         // Print the screen position
    //         TextDebug.SetTextDebug($"Target world position: " + worldPosition.x + ", " + worldPosition.y + ", " + worldPosition.z);
    //         TextDebug.SetTextDebug($"Target screen position: " + screenPosition.x + ", " + screenPosition.y + ", " + screenPosition.z);
    //     }
    //     else
    //     {
    //         if (target == null)
    //         {
    //             Debug.LogError("Target is not assigned!");
    //         }

    //         if (Camera.main == null)
    //         {
    //             Debug.LogError("Main camera not found!");
    //         }
    //     }
    // }

}
