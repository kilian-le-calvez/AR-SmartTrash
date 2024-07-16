using UnityEngine;

public class CircleClickHandler : MonoBehaviour
{
    public bool isStartCircle;

    void OnMouseDown()
    {
        if (isStartCircle)
        {
            Clicker clicker = FindObjectOfType<Clicker>();
            clicker.RemoveStartCircle();
            Destroy(gameObject);
        }
        else
        {
            Clicker clicker = FindObjectOfType<Clicker>();
            clicker.RemoveEndCircle();
            Destroy(gameObject);
        }
    }
}
