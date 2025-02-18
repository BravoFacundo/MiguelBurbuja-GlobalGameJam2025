using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlowDirection { Left, Right, Down, Up }
public class BlowObject : MonoBehaviour
{
    [Header("Config")]
    public Transform playerPos;
    public BlowDirection blowDirection;
    public bool startTired;
    public float timeAlive = 0.6f;

    [Header("Visuals")]
    [SerializeField] Color normalColor;
    [SerializeField] Color tiredColor;
    private Color startColor;
    private Color finalColor;
    private Color whiteClear = new( 1, 1, 1, 0);

    [Header("Follow")]
    [SerializeField] float followSpeed = 5f;
    [SerializeField] Vector2 defaultOffset = new Vector2( 5f, 3.5f);
    bool followX = false;
    bool followY = false;

    [Header("Audio")]
    [SerializeField] AudioClip blowSFX;
    [SerializeField] AudioClip tiredSFX;
    private AudioSource audioSource;

    [Header("List")]
    [SerializeField] List<SpriteRenderer> spriteRenderers = new();

    [Header("Resources")]
    [SerializeField] List<Sprite> spriteFaces = new();
    [SerializeField] List<Sprite> spriteFacesW = new();

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        SetAllSpritesTransparent();
    }

    private void Start()
    {
        SetObjectValues();

        if (blowSFX != null)
        {
            if (!startTired) audioSource.PlayOneShot(blowSFX);
            else audioSource.PlayOneShot(tiredSFX);
        }
        StartCoroutine(FadeIn(timeAlive/3));
    }

    private void Update()
    {
        if (playerPos == null || playerPos.position.y > defaultOffset.y) return;

        float newX = followX ? Mathf.Lerp(transform.position.x, playerPos.position.x, Time.deltaTime * followSpeed) : transform.position.x;
        float newY = followY ? Mathf.Lerp(transform.position.y, playerPos.position.y, Time.deltaTime * followSpeed) : transform.position.y;

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
    
    private void SetAllSpritesTransparent()
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr != null) sr.color = Color.clear; 
        }
    }
    private void SetFlipX(bool flipValue)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.flipX = flipValue;
        }
    }
    private void SetFlipY(bool flipValue)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.flipY = flipValue;
        }
    }
    private void SetObjectValues()
    {
        var randomIndex = Random.Range(0, spriteFaces.Count);
        spriteRenderers[0].sprite = spriteFaces[randomIndex];
        spriteRenderers[1].sprite = spriteFacesW[randomIndex];

        switch (blowDirection)
        {
            case BlowDirection.Left:
                transform.position = defaultOffset;
                followY = true;                
                break;
            case BlowDirection.Right:
                transform.position = -defaultOffset;
                followY = true;
                SetFlipX(true);
                break;
            case BlowDirection.Up:
                transform.position = -defaultOffset;
                transform.rotation = Quaternion.Euler(0, 0, 90f);
                followX = true;
                SetFlipX(true);
                break;
            case BlowDirection.Down:
                transform.position = defaultOffset;
                transform.rotation = Quaternion.Euler(0, 0, 90f);
                followX = true;
                SetFlipY(true);
                break;
        }

        spriteRenderers[0].color = normalColor;
        startColor = new Color(normalColor.r, normalColor.g, normalColor.b, 0f);
        if (!startTired) finalColor = normalColor;
        else finalColor = tiredColor;

        SnapToPlayer();
    }

    private IEnumerator FadeIn(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            spriteRenderers[0].color = Color.Lerp(startColor, finalColor, t);
            spriteRenderers[1].color = Color.Lerp(whiteClear, Color.white, t);
            yield return null;
        }

        yield return new WaitForSeconds(timeAlive / 3);
        StartCoroutine(FadeOutAndDestroy(timeAlive/3));
    }
    private IEnumerator FadeOutAndDestroy(float duration)
    {
        float elapsedTime = 0f;
        startColor = spriteRenderers[0].color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            spriteRenderers[0].color = Color.Lerp(startColor, Color.clear, t);
            spriteRenderers[1].color = Color.Lerp(Color.white, whiteClear, t);
            yield return null;
        }

        Destroy(gameObject, timeAlive * .1f);
    }

    private void SnapToPlayer()
    {
        if (playerPos == null) return;

        float newX = followX ? playerPos.position.x : transform.position.x;
        float newY = followY ? playerPos.position.y : transform.position.y;

        transform.position = new Vector3(newX, newY, transform.position.z);
    }


}

