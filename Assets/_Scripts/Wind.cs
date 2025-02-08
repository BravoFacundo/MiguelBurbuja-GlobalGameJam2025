using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector2 windDirection;
    public float windStrength = 10f;

    private void Start()
    {
        UpdateWindDirection();
    }
    private void UpdateWindDirection()
    {
        Vector3 worldDirection = transform.up;
        windDirection = new Vector2(worldDirection.x, worldDirection.y).normalized;
    }

}
