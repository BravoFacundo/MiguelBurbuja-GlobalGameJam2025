using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSphere2D : MonoBehaviour
{
    public float radius = 1f;
    public Vector2 offset;
    public LayerMask layerMask;

    private void Update()
    {
        Vector2 position = (Vector2)transform.position + offset;
        Collider2D hit = Physics2D.OverlapCircle(position, radius, layerMask);

        Debug.Log($"OverlapCircle Hit: {hit != null}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + (Vector3)offset, radius);
    }
}
