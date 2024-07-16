using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckMovement : MonoBehaviour
{
    public List<Vector3> waypoints;  // Array of positions for the truck to move through
    public float speed = 1f;     // Speed at which the truck moves

    private int currentWaypointIndex = 0;  // Current target waypoint index
    public bool isMoving = false;
    public GameObject target;

    public DrawLine gps;

    public void StartCamion()
    {
        gps = new DrawLine("gps path", target.transform);
        gps.DrawRedLine(waypoints);
        isMoving = true;
    }

    void Move()
    {
        if (currentWaypointIndex >= waypoints.Count)
        {
            isMoving = false;
            Destroy(gps._lineRenderer);
            currentWaypointIndex = 0;
            Debug.Log("Waypoint index out of range " + currentWaypointIndex.ToString() + " : " + waypoints.Count.ToString());
            return;
        }
        transform.LookAt(target.transform.TransformPoint(waypoints[currentWaypointIndex]));
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, waypoints[currentWaypointIndex], speed * Time.deltaTime);
        if (TruckOnPoint(waypoints[currentWaypointIndex]))
        {
            currentWaypointIndex++;  // Move to the next waypoint
        }
    }

    public bool TruckOnEnd()
    {
        if (waypoints == null || waypoints.Count == 0) return false;
        return TruckOnPoint(waypoints[waypoints.Count - 1]);
    }

    public bool TruckOnPoint(Vector3 point)
    {
        return Vector3.Distance(transform.localPosition, point) < 0.1f;
    }

    void Update()
    {
        if (waypoints == null || waypoints.Count == 0) return;
        if (isMoving) Move();
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     isMoving = !isMoving;
        // }
        // if (isMoving && TruckOnEnd())
        // {
        //     isMoving = false;
        // }
    }
}
