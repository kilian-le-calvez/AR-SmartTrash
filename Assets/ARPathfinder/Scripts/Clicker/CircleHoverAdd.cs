using UnityEngine;

public class CircleHoverAdd : MonoBehaviour
{
    public GameObject filledCirclePrefab;
    private GameObject filledCircleObj;

    public GameObject target;

    public float scale;

    void OnMouseEnter()
    {
        if (filledCirclePrefab != null)
        {
            filledCircleObj = Instantiate(filledCirclePrefab, Vector3.zero, Quaternion.identity, target.transform);
            filledCircleObj.transform.localPosition = transform.localPosition;
            filledCircleObj.transform.localRotation = filledCirclePrefab.transform.rotation;
            filledCircleObj.transform.localScale = new Vector3(scale, scale, scale);
            filledCircleObj.AddComponent<CircleClickHoverDel>();
        }
    }
}
