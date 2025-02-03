using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Debug")]
    public Transform playerPos;
    [SerializeField] private float maxStamina;

    [Header("Blow")]
    [SerializeField] List<GameObject> blowFaces = new (4);
    [SerializeField] BlowFacesMovement blowFacesMovement;
    [SerializeField] Color normalTint; 
    [SerializeField] Color tiredTint;
    [SerializeField] AudioClip blowSFX;
    [SerializeField] AudioClip tiredSFX;

    [Header("References")]
    [SerializeField] private Slider blowBar;
    [SerializeField] TMP_Text playButtonText;

    [Header("Resources")]
    [SerializeField] private List<Sprite> spriteFaces = new();

    private void Start()
    {
        blowBar.value = 1;
        SetBlowFacesOff();
    }

    public void NoPreviousProgress() => playButtonText.text = "Jugar";
    public void HasPreviousProgress() => playButtonText.text = "Continuar";

    public IEnumerator SetBlowFace(Vector2 dir)
    {
        blowFacesMovement.PlayerPos = playerPos;

        GameObject newBlowFace;
        if (dir == Vector2.left)
        {
            blowFaces[1].GetComponent<Image>().color = SetAlpha(blowFaces[1].GetComponent<Image>().color, 1f);
            newBlowFace = blowFaces[1];
        }
        else if (dir == Vector2.right)
        {
            blowFaces[0].GetComponent<Image>().color = SetAlpha(blowFaces[0].GetComponent<Image>().color, 1f);
            newBlowFace = blowFaces[0];
        }
        else if (dir == Vector2.up)
        {
            blowFaces[3].GetComponent<Image>().color = SetAlpha(blowFaces[3].GetComponent<Image>().color, 1f);
            newBlowFace = blowFaces[3];
        }
        else if (dir == Vector2.down)
        {
            blowFaces[2].GetComponent<Image>().color = SetAlpha(blowFaces[2].GetComponent<Image>().color, 1f);
            newBlowFace = blowFaces[2];
        }
        else newBlowFace = null;

        Utilities.PlaySoundAndDestroy(blowSFX);

        yield return new WaitForSeconds(.6f);
        SetBlowFaceOff(newBlowFace);
    }
    private void SetBlowFaceOff(GameObject blowFace)
    {
        if (blowFace != null)
        {
            Image blowImg = blowFace.GetComponent<Image>();
            if (blowImg != null)
            {
                blowImg.color = normalTint;
                blowImg.sprite = spriteFaces[Random.Range(0, spriteFaces.Count)];
                blowImg.color = SetAlpha(blowImg.color, 0f);
            }
        }
    }
    private void SetBlowFacesOff()
    {
        foreach (GameObject blowFace in blowFaces)
        {
            SetBlowFaceOff(blowFace);
        }
    }
    public void SetBlowFacesRed(float duration)
    {
        foreach (GameObject blowFace in blowFaces)
        {
            Image blowImg = blowFace.GetComponent<Image>();
            if (blowImg != null)
            {
                StartCoroutine(LerpColor(blowImg, tiredTint, duration));
            }
        }
    }
    public void SetBlowFacesBlue()
    {
        foreach (GameObject blowFace in blowFaces)
        {
            Image blowImg = blowFace.GetComponent<Image>();
            if (blowImg != null)
            {
                Color currentColor = blowImg.color;
                blowImg.color = new Color(normalTint.r, normalTint.g, normalTint.b, currentColor.a);
            }
        }
    }
    private IEnumerator LerpColor(Image blowImg, Color targetColor, float duration)
    {
        Color startColor = blowImg.color; // Color inicial
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration); // Normalización del tiempo

            // Interpolación entre el color inicial y el objetivo, manteniendo el alfa
            blowImg.color = new Color(
                Mathf.Lerp(startColor.r, targetColor.r, t),
                Mathf.Lerp(startColor.g, targetColor.g, t),
                Mathf.Lerp(startColor.b, targetColor.b, t),
                startColor.a // Mantiene el valor original del alfa
            );

            yield return null; // Espera al siguiente frame
        }

        // Asegúrate de que termine exactamente con el color objetivo
        blowImg.color = new Color(targetColor.r, targetColor.g, targetColor.b, startColor.a);
    }
    private Color SetAlpha(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    public void SetBlowBarDefaultValue(float blowStamina) => maxStamina = blowStamina;
    public void SetBlowBarValue(float currentStamina)
    {
        float normalizedStamina = Mathf.Clamp01(currentStamina / maxStamina);
        blowBar.value = normalizedStamina;
    }
}
