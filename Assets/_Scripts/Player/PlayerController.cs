using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] float blowForce = 0.1f;
    [SerializeField] float blowStamina = 250;
    [SerializeField] float blowCost = 100;
    [SerializeField] float staminaRecoveryRate = 1;
    float maxStamina;

    [Header("Random Force")]
    float minInterval = 0.3f;
    float maxInterval = 1.0f;
    private float nextForceTime;

    [Header("Music")]
    [SerializeField] float detectionRadius = 5f;
    [SerializeField] LayerMask colliderLayer;

    [Header("Sounds")]
    [SerializeField] AudioClip popSFX;
    [SerializeField] AudioClip inhalerSFX;

    [Header("References")]
    [SerializeField] private GameController gameController;
    [SerializeField] private UIManager uiManager;

    [Header("Local References")]
    private Rigidbody2D rb;

    private void Awake()
    {
        uiManager = GameObject.FindGameObjectWithTag("Manager").GetComponentInChildren<UIManager>();
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        rb = GetComponent<Rigidbody2D>();

    }
    private void Start()
    {
        maxStamina = blowStamina;
        uiManager.SetBlowBarDefaultValue(maxStamina);
    }

    private void Update()
    {
        Handle_Inputs();

        Handle_RandomForce();
        Handle_BlowStamina();

        if (IsNearCollider())
        {
            Debug.Log("¡Hay un collider cercano!");
        }
        else
        {
            Debug.Log("No hay colliders cercanos.");
        }
    }

    private void Handle_Inputs()
    {
        if (blowStamina >= blowCost)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                Blow(Vector2.left);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                Blow(Vector2.right);
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                Blow(Vector2.up);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                Blow(Vector2.down);
        }
    }
    void Blow(Vector2 dir)
    {
        rb.AddForce(dir * blowForce, ForceMode2D.Impulse);
        uiManager.SetBlowFacesBlue();
        blowStamina -= blowCost;

        uiManager.playerPos = transform;
        uiManager.StartCoroutine("SetBlowFace", dir);
        if (blowStamina <= blowCost) uiManager.SetBlowFacesRed(.35f);
    }

    private void Handle_RandomForce()
    {
        nextForceTime -= Time.deltaTime;
        if (nextForceTime <= 0f)
        {
            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            rb.AddForce(.1f * blowForce * randomDirection, ForceMode2D.Impulse);

            nextForceTime = Random.Range(minInterval, maxInterval);
        }
    }

    private void Handle_BlowStamina()
    {
        blowStamina += staminaRecoveryRate;
        Mathf.Clamp(blowStamina, 0, maxStamina);
        uiManager.SetBlowBarValue(blowStamina);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Colliders"))
        {
            StartCoroutine(nameof(DelayedDeath));
        }
    }
    private IEnumerator DelayedDeath()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        yield return new WaitForSeconds(.15f);
        Destroy(gameObject);
        Utilities.PlaySoundAndDestroy(popSFX);
        yield return new WaitForSeconds(1f);
        gameController.PlayerDie();
    }

    public bool IsNearCollider()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, detectionRadius, colliderLayer);
        return nearbyColliders.Length > 0; // Si hay al menos uno, estás cerca
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal"))
        {
            StartCoroutine(nameof(DelayedGoal));
        }
        else if (collision.CompareTag("PowerUp"))
        {
            StartCoroutine(nameof(DelayedPowerUp), collision.transform.parent.gameObject);
        }
    }
    private IEnumerator DelayedGoal()
    {
        yield return new WaitForSeconds(.25f);
        Utilities.PlaySoundAndDestroy(popSFX);
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        gameController.ReachedGoal();
    }
    private IEnumerator DelayedPowerUp(GameObject inhaler)
    {
        yield return new WaitForSeconds(.25f);
        blowStamina = maxStamina;
        Utilities.PlaySoundAndDestroy(popSFX);
        Destroy(inhaler);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Wind"))
        {
            Vector2 windDir = collision.GetComponentInParent<Wind>().windDirection;
            rb.AddForce(.01f * blowForce * windDir, ForceMode2D.Impulse);
        }
    }

}
