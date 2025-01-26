using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowFacesMovement : MonoBehaviour
{
    [Header("References")]
    public Transform PlayerPos;
    [SerializeField] RectTransform canvasRectTransform;
    private float width;
    private float height;

    [Header("Y Elements")]
    [SerializeField] RectTransform yElement1;
    [SerializeField] RectTransform yElement2;

    [Header("X Elements")]
    [SerializeField] RectTransform xElement1;
    [SerializeField] RectTransform xElement2;

    void Start()
    {
        width = canvasRectTransform.sizeDelta.x * 0.5f;
        height = canvasRectTransform.sizeDelta.y * 0.5f;
    }

    void Update()
    {
        if (PlayerPos == null) return;

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(PlayerPos.position);

        yElement1.anchoredPosition = new Vector2(yElement1.anchoredPosition.x, screenPosition.y - height);
        yElement2.anchoredPosition = new Vector2(yElement2.anchoredPosition.x, screenPosition.y - height);

        xElement1.anchoredPosition = new Vector2(screenPosition.x - width, xElement1.anchoredPosition.y);
        xElement2.anchoredPosition = new Vector2(screenPosition.x - width, xElement2.anchoredPosition.y);

    }
}
