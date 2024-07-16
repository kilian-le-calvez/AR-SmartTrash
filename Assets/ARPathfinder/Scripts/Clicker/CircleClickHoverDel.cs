using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleClickHoverDel : MonoBehaviour
{
    float lifeTime = 0.5f;

    void OnMouseExit()
    {
        Destroy(gameObject);
        Destroy(this);
    }

    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
            Destroy(this);
        }
    }
}
