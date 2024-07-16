using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public InitTruck initTruck;
    public Clicker clicker;
    public PathFinding pathfinding;
    public Vector3 startPos;
    public Vector3 endPos;

    public GameObject target;

    private PlayerAction playerAction;

    // trash
    public Trash trashPrefab;
    private List<Trash> trashes = new List<Trash>();
    public float trashScale = 0.2f;

    private void Awake()
    {
        // Initialize the input actions
        playerAction = new PlayerAction();
        // Register the click event
        playerAction.Player.Click.performed += OnTapPerformed;
        // Enable the input actions
        playerAction.Player.Enable();
    }

    private void OnDestroy()
    {
        // Unregister the click event
        playerAction.Player.Click.performed -= OnTapPerformed;
        // Disable the input actions
        playerAction.Player.Disable();
    }

    private void OnTapPerformed(InputAction.CallbackContext context)
    {
        // Handle actual click input
        CircleType typeCircle = clicker.HandleClick();
        // When setting start, spawn truck
        if (typeCircle == CircleType.Start)
        {
            if (initTruck != null && mapGenerator != null && clicker.getStartPos() != null)
            {
                startPos = clicker.getStartPos().Value;
                initTruck.SpawnTruck(startPos);
            }

        }
        // When setting end, move truck
        else if (typeCircle == CircleType.End && clicker.getEndPos() != null)
        {
            HandleMoveTruck(clicker.getEndPos().Value);
        }
        // Generate trash on click
        else if (typeCircle == CircleType.Trash && clicker.getLastTrashPosition() != null)
        {
            Vector3 trashPos = clicker.getLastTrashPosition().Value;

            Trash trash = Instantiate(trashPrefab, Vector3.zero, Quaternion.identity, parent: target.transform);
            trash.transform.localRotation = trashPrefab.transform.rotation;
            trash.transform.localPosition = trashPos;
            trash.transform.localScale = new Vector3(trashScale, trashScale, trashScale);
            trashes.Add(trash);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (mapGenerator == null || initTruck == null || clicker == null)
        {
            Debug.LogError("MapGenerator or InitTruck or Clicker is not assigned.");
        }
        // else
        // {
        //     StartInit();
        // }
    }

    void StartInit()
    {
        mapGenerator.Init(target);
        // clicker.SetMap(mapGenerator.GetMap());
        clicker.target = target;
        initTruck.target = target;
        // pathfinding.SetMap(mapGenerator.GetMap());
    }

    void HandleMoveTruck(Vector3 newEndPos)
    {
        endPos = newEndPos;
        List<Vector3> pathPoints = pathfinding.FindPath(startPos, endPos);
        if (pathPoints.Count == 0)
        {
            Debug.Log("No path found.");
            return;
        }
        startPos = endPos;

        // Give new way to truck to follow + start moving
        initTruck.Move(pathPoints);
    }

    public void OnclickStartSelection()
    {
        // Display blue circle + setting set start position
        clicker.HandleTypeCircleEdition(CircleType.Start);
    }
    public void OnclickTrashSelection()
    {
        // Display blue circle + setting set trash position
        clicker.HandleTypeCircleEdition(CircleType.Trash);
    }

    public void OnclickReset()
    {
        mapGenerator.OnDestroy();
        initTruck.OnDestroy();
        clicker.OnDestroy();
        startPos = Vector3.zero;
        endPos = Vector3.zero;
        foreach (Trash trash in trashes)
        {
            Debug.Log("Destroying trash...");
            Destroy(trash.gameObject);
        }
        trashes.Clear();
        TextDebug.SetTextDebug("Reset done.");
    }

    public void OnClickStart()
    {
        StartInit();
        TextDebug.SetTextDebug("Init done.");
    }

    // Update is called once per frame
    void Update()
    {
        if (initTruck.truckMovement && !initTruck.truckMovement.isMoving)
        {
            Debug.Log("Truck ended - Next trash to collect...");
            // for each trash in a random order
            foreach (Trash trash in trashes)
            {
                if (!trash.isCollected)
                {
                    Debug.Log("Trash need to be collected...");
                    HandleMoveTruck(trash.transform.localPosition);
                    break;
                }
            }
        }
    }
}
