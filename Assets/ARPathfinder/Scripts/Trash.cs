using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Trash : MonoBehaviour
{
    const int tilted = 0;
    const int vertical = -90;

    public bool isCollected = true;

    private PlayerAction playerAction;

    public void Collect()
    {
        isCollected = true;
        gameObject.transform.rotation = Quaternion.Euler(vertical, 0, 0);
    }

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

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Handle the hit object
            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log("Trash clicked");
                isCollected = false;
                gameObject.transform.rotation = Quaternion.Euler(tilted, Random.Range(0, 360), 0);
            }
        }
    }

    // public void OnMouseDown()
    // {
    //     Debug.Log("Trash clicked");
    //     isCollected = false;
    //     gameObject.transform.rotation = Quaternion.Euler(tilted, Random.Range(0, 360), 0);
    // }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");
        // if (other.gameObject.tag == "Truck")
        Collect();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
