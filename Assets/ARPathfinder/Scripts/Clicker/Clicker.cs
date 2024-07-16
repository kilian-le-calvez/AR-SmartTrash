using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CircleType
{
    Start,
    End,
    Trash,
    Undefined
}

public class Clicker : MonoBehaviour
{
    private CircleType _circleType = CircleType.Undefined;
    private GameObject _startCircle;
    private GameObject _endCircle;
    private Vector3? _startPosition = null;
    private Vector3? _endPosition = null;
    private List<Vector3> _trashPositions = new List<Vector3>();
    private Vector3? _lastTrashPosition = null;

    private List<GameObject> listBlueCircles = new List<GameObject>();

    //Prefabs
    public GameObject outlinedBlueCirclePrefab;
    public GameObject outlinedGreenCirclePrefab;
    public GameObject outlinedRedCirclePrefab;

    public GameObject filledBlueCirclePrefab;
    public GameObject filledGreenCirclePrefab;
    public GameObject filledRedCirclePrefab;

    public GameObject target;
    public float circleSize = 0.5f;

    public void OnDestroy()
    {
        RemoveStartCircle();
        RemoveEndCircle();
    }

    public Vector3? getStartPos()
    {
        return _startPosition;
    }

    public Vector3? getEndPos()
    {
        return _endPosition;
    }

    public List<Vector3> getTrashPositions()
    {
        return _trashPositions;
    }

    public Vector3? getLastTrashPosition()
    {
        return _lastTrashPosition;
    }

    public void HandleTypeCircleEdition(CircleType circleType)
    {
        if (listBlueCircles.Count == 0)
        {
            _circleType = circleType;
            DrawBlueCircles();
        }
    }

    void DrawBlueCircles()
    {
        foreach (Tile tile in MapGenerator._map)
        {
            foreach (PathTile path in tile.GetPaths())
            {
                GameObject circle1 = Instantiate(outlinedBlueCirclePrefab, Vector3.zero, Quaternion.identity, parent: target.transform);
                circle1.transform.localPosition = path.positionGate1;
                circle1.transform.rotation = target.transform.rotation;
                circle1.transform.localRotation = outlinedBlueCirclePrefab.transform.rotation;
                circle1.transform.localScale = new Vector3(circleSize, circleSize, circleSize);

                GameObject circle2 = Instantiate(outlinedBlueCirclePrefab, Vector3.zero, Quaternion.identity, parent: target.transform);
                circle2.transform.localPosition = path.positionGate2;
                circle2.transform.rotation = target.transform.rotation;
                circle2.transform.localRotation = outlinedBlueCirclePrefab.transform.rotation;
                circle2.transform.localScale = new Vector3(circleSize, circleSize, circleSize);

                CircleHoverAdd circleHover1 = circle1.AddComponent<CircleHoverAdd>();
                circleHover1.filledCirclePrefab = filledBlueCirclePrefab;
                circleHover1.target = target;
                circleHover1.scale = circleSize;

                CircleHoverAdd circleHover2 = circle2.AddComponent<CircleHoverAdd>();
                circleHover2.filledCirclePrefab = filledBlueCirclePrefab;
                circleHover2.target = target;
                circleHover2.scale = circleSize;

                listBlueCircles.Add(circle1);
                listBlueCircles.Add(circle2);
            }
        }
    }

    public CircleType HandleClick()
    {
        if (_circleType == CircleType.Undefined) return CircleType.Undefined;
        // Check if the touch position is valid
        Vector2 touchPosition;
        if (Touchscreen.current == null)
        {
            touchPosition = Mouse.current.position.ReadValue();
            Debug.Log("Touchscreen not detected. using mouse position.");
        }
        else
        {
            Debug.Log("Touchscreen detected. playing on mobile.");
            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (!hit.collider.CompareTag("trash"))
            {
                listBlueCircles.ForEach(Destroy);
                listBlueCircles.Clear();
                _circleType = CircleType.Undefined;
                return CircleType.Undefined;
            }

            if (_circleType == CircleType.Start)
            {
                // RemoveStartCircle();
                _startPosition = hit.transform.localPosition;
                listBlueCircles.ForEach(Destroy);
                listBlueCircles.Clear();
                _circleType = CircleType.Undefined;
                return CircleType.Start;
            }
            else if (_circleType == CircleType.End)
            {
                // RemoveEndCircle();
                _endPosition = hit.transform.localPosition;
                listBlueCircles.ForEach(Destroy);
                listBlueCircles.Clear();
                _circleType = CircleType.Undefined;
                return CircleType.End;
            }
            else if (_circleType == CircleType.Trash)
            {
                _trashPositions.Add(hit.transform.localPosition);
                _lastTrashPosition = hit.transform.localPosition;
                listBlueCircles.ForEach(Destroy);
                listBlueCircles.Clear();
                _circleType = CircleType.Undefined;
                return CircleType.Trash;
            }
        }
        return CircleType.Undefined;
    }

    public void RemoveStartCircle()
    {
        if (_startCircle != null)
        {
            Destroy(_startCircle);
            _startCircle = null;
        }
    }

    public void RemoveStart()
    {
        _startPosition = null;
        RemoveStartCircle();
    }

    public void RemoveEndCircle()
    {
        if (_endCircle != null)
        {
            Destroy(_endCircle);
            _endCircle = null;
        }
    }
}