using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public void SetMap(List<Tile> map)
    {
        MapGenerator._map = map;
    }

    public List<Vector3> ListVectorToPathWaypoints(List<Vector3> list)
    {
        List<Vector3> pathWaypoints = new List<Vector3>();
        for (int i = 1; i < list.Count; i++)
        {
            for (int j = 0; j < MapGenerator._map.Count; j++)
            {
                PathTile path = MapGenerator._map[j].GetPathFromPosition(list[i - 1], list[i]);
                if (path == null)
                {
                    continue;
                }
                // Path is found
                if (path.positionGate1 == list[i - 1] && path.positionGate2 == list[i])
                {
                    pathWaypoints.AddRange(path.pathPoints);
                    break;
                }
                else if (path.positionGate2 == list[i - 1] && path.positionGate1 == list[i])
                {
                    // pathpoints are drawn from gate1 to gate2
                    List<Vector3> pathPointsCopy = new List<Vector3>(path.pathPoints);
                    pathPointsCopy.Reverse();
                    pathWaypoints.AddRange(pathPointsCopy);
                    break;
                }
            }
        }
        return pathWaypoints;
    }

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        Queue<Vector3> queue = new Queue<Vector3>();
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        List<Vector3> fullPath = new List<Vector3>();
        queue.Enqueue(start);
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            Vector3 current = queue.Dequeue();

            if (current == end)
            {
                // Reconstruct the path
                while (current != start)
                {
                    fullPath.Add(current);
                    current = cameFrom[current];
                }
                fullPath.Add(start);
                fullPath.Reverse();
                // Before we have a list end to end of the path, we need to convert it to a list of waypoints
                // They are stored in the Path object, we just need to find the right path so

                // Debug.Log("Path found: " + fullPath.Count.ToString());
                // log all path
                string pathLog = "";
                foreach (Vector3 pos in fullPath)
                {
                    pathLog += pos.ToString() + " -> ";
                }
                Debug.Log("basic path : " + pathLog);
                List<Vector3> fullPathpoints = ListVectorToPathWaypoints(fullPath);
                return fullPathpoints;
            }

            foreach (Tile tile in MapGenerator._map)
            {
                foreach (PathTile path in tile.GetPaths())
                {
                    Vector3 nextPosition = Vector3.zero;
                    if (current == path.positionGate1)
                    {
                        nextPosition = path.positionGate2;
                    }
                    else if (current == path.positionGate2)
                    {
                        nextPosition = path.positionGate1;
                    }

                    if (nextPosition != Vector3.zero && !cameFrom.ContainsKey(nextPosition))
                    {
                        queue.Enqueue(nextPosition);
                        cameFrom[nextPosition] = current;
                    }
                }
            }
        }

        return null; // No path found
    }
}
