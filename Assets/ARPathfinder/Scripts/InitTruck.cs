using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitTruck : MonoBehaviour
{
    public GameObject camion;
    public TruckMovement truckMovement;

    public GameObject target;

    public float truckScale = 1f;

    public void OnDestroy()
    {
        if (truckMovement != null)
            Destroy(truckMovement.gameObject);
    }

    public void SpawnTruck(Vector3 initPoint)
    {
        OnDestroy();
        CreateTruck(initPoint);
    }

    public void Move(List<Vector3> waypoints)
    {
        truckMovement.waypoints = waypoints;
        truckMovement.target = target;
        truckMovement.StartCamion();
    }

    void CreateTruck(Vector3 startPosition)
    {
        GameObject truck = Instantiate(camion, Vector3.zero, target.transform.rotation, parent: target.transform);
        truck.transform.localPosition = startPosition;
        truck.transform.localScale = new Vector3(truckScale, truckScale, truckScale);
        truckMovement = truck.GetComponent<TruckMovement>();
    }

    public bool TruckEnded()
    {
        if (truckMovement == null)
            return false;
        return truckMovement.TruckOnEnd() && !truckMovement.isMoving;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
