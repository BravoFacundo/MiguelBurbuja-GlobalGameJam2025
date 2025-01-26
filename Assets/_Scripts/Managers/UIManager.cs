using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameManager gameManager;
    [HideInInspector] public Transform playerPos;
    private float maxStamina;

    [Header("Blow")]
    [SerializeField] List<GameObject> blowFaces = new (4);
    GameObject newBlowFace;
    [SerializeField] BlowFacesMovement blowFacesMovement;
    [SerializeField] Color normalTint; 
    [SerializeField] Color tiredTint;
    [SerializeField] AudioClip blowSFX;
    [SerializeField] AudioClip tiredSFX;

    [Header("Bar")]
    [SerializeField] private Slider blowBar;

    [Header("Resources")]
    [SerializeField] private List<Sprite> spriteFaces = new();

    private void Awake()
    {
        Transform parentObj = transform.parent;
        gameManager = parentObj.GetComponentInChildren<GameManager>();
    }

    private void Start()
    {
        blowBar.value = 1;
        SetBlowFacesOff();
    }

    public IEnumerator SetBlowFace(Vector2 dir)
    {
        blowFacesMovement.PlayerPos = playerPos;

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

        Utilities.PlaySoundAndDestroy(blowSFX);

        yield return new WaitForSeconds(.6f);
        SetBlowFaceOff(newBlowFace);
    }
    private void SetBlowFaceOff(GameObject blowFace)
    {
        Image blowImg = blowFace.GetComponent<Image>();
        if (blowImg != null)
        {
            blowImg.color = normalTint;
            blowImg.sprite = spriteFaces[Random.Range(0, spriteFaces.Count)];
            blowImg.color = SetAlpha(blowImg.color, 0f);
        }
    }
    private void SetBlowFacesOff()
    {
        foreach (GameObject blowFace in blowFaces)
        {
            SetBlowFaceOff(blowFace);
        }
    }
    private void SetBlowFacesRed()
    {
        foreach (GameObject blowFace in blowFaces)
        {
            Image blowImg = blowFace.GetComponent<Image>();
            blowImg.color = tiredTint;
        }
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
