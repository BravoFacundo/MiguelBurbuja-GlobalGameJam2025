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

    [Header("Sounds")]
    [SerializeField] AudioClip popSFX;

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
        blowStamina -= blowCost;

        uiManager.playerPos = transform;
        uiManager.StartCoroutine("SetBlowFace", dir);
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
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            StartCoroutine(nameof(DelayedCollisionEnter2D));
        }
    }
    private IEnumerator DelayedCollisionEnter2D()
    {
        yield return new WaitForSeconds(.15f);
        Destroy(gameObject);
        Utilities.PlaySoundAndDestroy(popSFX);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal"))
        {
            StartCoroutine(nameof(DelayedGoal));
        }
        else if (collision.CompareTag("PowerUp"))
        {

        }
    }
    private IEnumerator DelayedGoal()
    {
        yield return new WaitForSeconds(.25f);
        Utilities.PlaySoundAndDestroy(popSFX);
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        gameController.ReachedGoal();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Wind"))
        {
            Vector2 windDir = collision.GetComponent<Wind>().windDirection;
            rb.AddForce(windDir * blowForce * .01f, ForceMode2D.Impulse);
        }
    }

}
