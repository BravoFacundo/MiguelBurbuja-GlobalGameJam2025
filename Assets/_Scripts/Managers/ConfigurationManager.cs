using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfigurationManager : MonoBehaviour
{
    GameManager gameManager;

    //[Header("Language")]

    //[Header("Sound")]

    [Header("Microphone Debug")]
    [SerializeField] TMP_Dropdown debugDropdown;
    [SerializeField] Slider debugSlider;
    [SerializeField] Button debugButton;

    [Header("Microphone")]
    [SerializeField] string selectedMicrophone;
    public bool hasMics = true;
    public float gainMultiplier = 1.0f;
    public float threshold = 0.01f;
    private bool wasBelowThreshold = true;

    public event Action<bool> OnMicrophoneStateChanged;

    string[] microphones;
    private AudioClip micClip;
    private bool micActive = false;
    private bool isCalibrating = false;
    int sampleRate = 1024;

    [Header("Microphone References")]
    [SerializeField] Toggle micToggle;
    [SerializeField] TMP_Dropdown micDropdown;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider gainSlider;
    [SerializeField] Button calibrateButton;

    void Start()
    {
        gameManager = transform.parent.GetComponentInChildren<GameManager>();
        
        StartMicrophoneConfiguration();
    }

    private void Update()
    {
        Debug_MicConfiguration();
    }

    //---------- LANGUAGE ------------------------------------------------------------------------------------------------------------------

    //---------- SOUND ------------------------------------------------------------------------------------------------------------------

    //---------- MICROPHONE ------------------------------------------------------------------------------------------------------------------

    void StartMicrophoneConfiguration()
    {
        DetectMicrophones();
        Debug_Start();

        micToggle.onValueChanged.AddListener(ToggleMicrophone);
        gainSlider.onValueChanged.AddListener(ChangeMicrophoneGain);
        calibrateButton.onClick.AddListener(ToggleCalibration);

        gainMultiplier = PlayerPrefs.GetFloat("GainMultiplier", gainMultiplier);
        threshold = PlayerPrefs.GetFloat("Threshold", threshold);
        gainSlider.value = gainMultiplier;
        volumeSlider.value = threshold;
    }

    void DetectMicrophones()
    {
        microphones = Microphone.devices;        
        if (microphones.Length == 0)
        {
            Debug.LogError("No se encontraron micrófonos en el sistema.");
            hasMics = false;
            return;
        }
        selectedMicrophone = PlayerPrefs.GetString("SelectedMicrophone", microphones[0]);
        StartMicrophone();
    }
    void Debug_Start()
    {
        debugDropdown.ClearOptions();
        debugDropdown.AddOptions(new System.Collections.Generic.List<string>(microphones));
        debugDropdown.value = System.Array.IndexOf(microphones, selectedMicrophone);
        debugDropdown.onValueChanged.AddListener(ChangeMicrophone);
        debugButton.image.color = Color.red;
    }

    void Debug_MicConfiguration()
    {
        if (!micActive) return;

        float volume = GetMicVolume() * gainMultiplier;
        debugSlider.value = volume;
        volumeSlider.value = debugSlider.value;

        bool isAboveThreshold = volume > threshold;
        debugButton.image.color = isAboveThreshold ? Color.green : Color.red;

        if (isAboveThreshold != wasBelowThreshold)
        {
            OnMicrophoneStateChanged?.Invoke(isAboveThreshold);
            wasBelowThreshold = isAboveThreshold;
        }

    }

    public void StartMicrophone()
    {
        if (micClip == null || !micActive)
        {
            micClip = Microphone.Start(selectedMicrophone, true, 1, sampleRate);
            micActive = true;
        }
    }
    public void StopMicrophone()
    {
        if (micActive)
        {
            Microphone.End(selectedMicrophone);
            micActive = false;
        }
    }
    public void ChangeMicrophone(int index)
    {
        StopMicrophone();
        selectedMicrophone = Microphone.devices[index];
        StartMicrophone();
    }
    public void ToggleMicrophone(bool isOn)
    {
        if (isOn) StartMicrophone(); else StopMicrophone();
    }
    public void SetMicrophoneThreshold(float value)
    {
        threshold = value;
        PlayerPrefs.SetFloat("Threshold", threshold);
    }
    public void ChangeMicrophoneGain(float value)
    {
        gainMultiplier = value;
        PlayerPrefs.SetFloat("GainMultiplier", gainMultiplier);
    }

    public void ToggleCalibration()
    {
        isCalibrating = !isCalibrating;
        volumeSlider.interactable = isCalibrating;
        if (isCalibrating)
        {
            volumeSlider.handleRect.gameObject.SetActive(true);
            volumeSlider.onValueChanged.AddListener(SetMicrophoneThreshold);
        }
        else
        {
            volumeSlider.handleRect.gameObject.SetActive(false);
            volumeSlider.onValueChanged.RemoveListener(SetMicrophoneThreshold);
        }
    }

    float GetMicVolume()
    {
        if (micClip == null) return 0f;

        float[] samples = new float[sampleRate];
        micClip.GetData(samples, 0);

        float sum = 0f;
        foreach (float sample in samples)
        {
            sum += sample * sample;
        }

        return Mathf.Sqrt(sum / samples.Length);
    }

}
