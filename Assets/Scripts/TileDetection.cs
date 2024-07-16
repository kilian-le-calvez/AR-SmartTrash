using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class TileDetection : MonoBehaviour
{
    private ObserverBehaviour mObserverBehaviour;
    public Tile tileModel;
    public Camera cam;

    void Start()
    {
        mObserverBehaviour = GetComponent<ObserverBehaviour>();
        if (mObserverBehaviour)
        {
            mObserverBehaviour.OnTargetStatusChanged += OnObserverStatusChanged;
            mObserverBehaviour.OnBehaviourDestroyed += OnObserverDestroyed;
        }
    }

    private void OnDestroy()
    {
        if (mObserverBehaviour)
        {
            mObserverBehaviour.OnTargetStatusChanged -= OnObserverStatusChanged;
            mObserverBehaviour.OnBehaviourDestroyed -= OnObserverDestroyed;
        }
    }

    private void OnObserverStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        if (targetStatus.Status == Status.TRACKED ||
            targetStatus.Status == Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else if (targetStatus.Status == Status.NO_POSE)
        {
            OnTrackingLost();
        }
        else
        {
            OnTrackingLost();
        }
    }

    private void OnObserverDestroyed(ObserverBehaviour behaviour)
    {
        // Handle any cleanup if needed when the observer is destroyed
    }

    private void OnTrackingFound()
    {
        // Get the 3D world position of the target
        Vector3 worldLocalPosition = mObserverBehaviour.transform.localPosition;
        // TextDebug.SetTextDebug($"Target world position: {worldPosition}");

        // Verify Camera.main is not null
        if (Camera.main == null)
        {
            TextDebug.SetTextDebug("Main camera not found!");
            return;
        }

        // if (screenPosition.x == 0 && screenPosition.y == 0)
        // {
        //     // TextDebug.SetTextDebug("Target is not in view!");
        //     return;
        // }

        // Use the screenPosition as needed
        // For example, pass it to your TileManager
        TileManager.Instance.AddTile(mObserverBehaviour.TargetName, worldLocalPosition, mObserverBehaviour.transform, tileModel);
    }


    private void OnTrackingLost()
    {
        Debug.Log("Trackable " + mObserverBehaviour.TargetName + " lost");
        TileManager.Instance.RemoveTile(mObserverBehaviour.TargetName, tileModel, mObserverBehaviour.transform.position);
    }
}
