using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    public bool canMove;
    public bool usingMic;
    [SerializeField] float blowForce = 0.1f;
    [SerializeField] float blowCooldown = 0.5f;
    [SerializeField] float blowStamina = 250;
    [SerializeField] float blowCost = 100;
    [SerializeField] float staminaRecoveryRate = 1;
    private bool canBlow = true;
    float maxStamina;

    [Header("Microphone")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] string selectedMicrophone;
    float[] samples;
    int sampleRate = 44100;
    float threshold = 0.02f;
    public Slider volumeSlider;
    public Dropdown microphoneDropdown;

    [Header("Random Force")]
    float minInterval = 0.3f;
    float maxInterval = 1.0f;
    private float nextForceTime;

    [Header("Collision")]
    [SerializeField] LayerMask collisionLayer;
    [SerializeField] float wallDetectionDistance = .75f;
    [SerializeField] bool isCloseToWall;
    [SerializeField] bool lastCloseToWallValue;

    [Header("Sprites")]
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite tenseSprite;
    [SerializeField] Sprite deathSprite;
    private SpriteRenderer spriteRenderer;

    [Header("Sounds")]
    [SerializeField] AudioClip popSFX;

    [Header("Prefabs")]
    [SerializeField] GameObject blowObjectPrefab;

    [Header("Local References")]
    private Rigidbody2D rb;

    [Header("References")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private HUDManager uiManager;
    [SerializeField] private MusicManager musicManager;

    private void Awake()
    {
        GameObject managers = GameObject.FindGameObjectWithTag("Managers");       
        uiManager = managers.GetComponentInChildren<HUDManager>();
        musicManager = managers.GetComponentInChildren<MusicManager>();
        levelManager = GameObject.FindWithTag("Levels").GetComponent<LevelManager>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        DetectMicrophone();
    }

    void DetectMicrophone()
    {
        selectedMicrophone = PlayerPrefs.GetString("SelectedMicrophone", "");
        if (!string.IsNullOrEmpty(selectedMicrophone)) StartMicrophone(selectedMicrophone);
    }
    void StartMicrophone(string micName)
    {
        if (Microphone.IsRecording(micName)) Microphone.End(micName);

        audioSource.clip = Microphone.Start(micName, true, 1, 44100);
        samples = new float[audioSource.clip.samples];
    }

    private void Start()
    {
        lastCloseToWallValue = isCloseToWall;
        maxStamina = blowStamina;
        
        uiManager.SetBlowBarDefaultValue(maxStamina);
    }

    private void Update()
    {
        if (canMove)
        {
            Handle_RandomForce();
            Handle_InputModes();
        }
        Handle_BlowStamina();
        Handle_Collision();
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
    private void Handle_InputModes()
    {
        if (blowStamina >= blowCost && canBlow)
        {
            if (!usingMic) Handle_Inputs();
            else Handle_MicInput();
        }
    }
    private void Handle_Inputs()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            Blow(Vector2.left, BlowDirection.Left);
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            Blow(Vector2.right, BlowDirection.Right);
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            Blow(Vector2.up, BlowDirection.Up);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            Blow(Vector2.down, BlowDirection.Down);
    }
    private void Handle_MicInput()
    {
        if (audioSource.clip == null) return;
        audioSource.clip.GetData(samples, 0);

        float currentVolume = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            currentVolume += Mathf.Abs(samples[i]);
        }
        currentVolume /= samples.Length;

        if (currentVolume > threshold)
        {
            Debug.Log("Volumen actual: " + currentVolume);
            Debug.Log("¡Se ha detectado una entrada de micrófono!");

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                Blow(Vector2.left, BlowDirection.Left);
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                Blow(Vector2.right, BlowDirection.Right);
            else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                Blow(Vector2.up, BlowDirection.Up);
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                Blow(Vector2.down, BlowDirection.Down);            
        }
    }
    private void Handle_Collision()
    {
        bool prevState = isCloseToWall;
        isCloseToWall = Physics2D.OverlapCircle(transform.position, wallDetectionDistance, collisionLayer) != null;
        if (prevState == isCloseToWall) return;

        if (isCloseToWall)
        {
            spriteRenderer.sprite = tenseSprite;
            musicManager.EnableTenseTrack();
        }
        else
        {
            spriteRenderer.sprite = defaultSprite;
            musicManager.DisableTenseTrack();
        }
    }
    private void Handle_BlowStamina()
    {
        if (blowStamina <= maxStamina) blowStamina += staminaRecoveryRate;
        uiManager.SetBlowBarValue(blowStamina);
    }

    void Blow(Vector2 dir, BlowDirection blowDirection)
    {
        StartCoroutine(nameof(BlowCooldown));
        rb.AddForce(dir * blowForce, ForceMode2D.Impulse);
        blowStamina -= blowCost;

        GameObject newBlowObject = Instantiate(blowObjectPrefab, transform.position , Quaternion.identity);
        BlowObject newBlowObjectScript = newBlowObject.GetComponent<BlowObject>();
        newBlowObjectScript.playerPos = transform;
        newBlowObjectScript.blowDirection = blowDirection;
        newBlowObjectScript.timeAlive = 0.75f;
        if (blowStamina <= blowCost) newBlowObjectScript.startTired = true;
    }
    private IEnumerator BlowCooldown()
    {
        canBlow = false;
        yield return new WaitForSeconds(blowCooldown);
        canBlow = true;
    }

    private IEnumerator Player_Die()
    {
        StartCoroutine(levelManager.PlayerDie());
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spriteRenderer.sprite = deathSprite;
        yield return new WaitForSeconds(.4f);
        levelManager.SendPlayerToPool();
        SoundManager.PlaySoundAndDestroy(popSFX);
        yield return new WaitForSeconds(1f);
    }
    private IEnumerator Player_Win()
    {
        yield return new WaitForSeconds(.25f);
        SoundManager.PlaySoundAndDestroy(popSFX);
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        levelManager.PlayerReachedGoal();
    }
    private IEnumerator Player_Recharge(GameObject inhaler)
    {
        yield return new WaitForSeconds(.25f);
        blowStamina = maxStamina;
        SoundManager.PlaySoundAndDestroy(popSFX);
        Destroy(inhaler);
    }

    //---------- COLLISIONS ------------------------------------------------------------------------------------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Colliders"))
            StartCoroutine(nameof(Player_Die));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal")) StartCoroutine(nameof(Player_Win));
        else if (collision.CompareTag("PowerUp"))
            StartCoroutine(nameof(Player_Recharge), collision.transform.parent.gameObject);
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
