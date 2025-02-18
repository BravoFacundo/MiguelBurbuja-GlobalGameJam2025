using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private float maxStamina;

    [Header("References")]
    [SerializeField] private Slider blowBar;

    private void Start() => blowBar.value = 1;

    public void SetBlowBarDefaultValue(float blowStamina) => maxStamina = blowStamina;
    public void SetBlowBarValue(float currentStamina)
    {
        float normalizedStamina = Mathf.Clamp01(currentStamina / maxStamina);
        blowBar.value = normalizedStamina;
    }
}
