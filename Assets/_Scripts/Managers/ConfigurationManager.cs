using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System;

public class ConfigurationManager : MonoBehaviour
{
    GameManager gameManager;

    [Header("Microphone Debug")]
    [SerializeField] bool debugMode = false;
    [SerializeField] Canvas debugCanvas;    
    private TMP_Dropdown debugDropdown;
    private Slider debugSlider;
    private Button debugButton;

    [Header("Microphone Settings")]
    public bool micEnabled = false;
    [SerializeField] string selectedMicrophone;
    private bool hasMics = true;
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
    
    [Header("Sound Settings")]
    [SerializeField] AudioMixer audioMixer;

    [Header("Sound References")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;

    void Start()
    {
        gameManager = transform.parent.GetComponentInChildren<GameManager>();

        Start_MicrophoneConfiguration();
        Start_SoundConfiguration();        
        Start_DebugConfiguration();
    }

    private void Update() => Debug_MicConfiguration();

    //---------- MICROPHONE ------------------------------------------------------------------------------------------------------------------

    void Start_MicrophoneConfiguration()
    {
        DetectMicrophones();
        
        micToggle.onValueChanged.AddListener(ToggleMicrophone);
        ToggleMicrophone(PlayerPrefs.GetInt("MicrophoneEnabled", 0) == 1);
        micToggle.isOn = PlayerPrefs.GetInt("MicrophoneEnabled", 0) == 1;
        
        threshold = PlayerPrefs.GetFloat("Threshold", threshold);
        volumeSlider.value = threshold;

        micDropdown.ClearOptions();
        micDropdown.AddOptions(new System.Collections.Generic.List<string>(microphones));
        micDropdown.value = System.Array.IndexOf(microphones, selectedMicrophone);
        micDropdown.onValueChanged.AddListener(ChangeMicrophone);
    }
    void Start_DebugConfiguration()
    {
        if (!debugMode) return;
        
        debugCanvas.gameObject.SetActive(true);

        debugDropdown = debugCanvas.GetComponentInChildren<TMP_Dropdown>();
        debugSlider = debugCanvas.GetComponentInChildren<Slider>();
        debugButton = debugCanvas.GetComponentInChildren<Button>();

        debugDropdown.ClearOptions();
        debugDropdown.AddOptions(new System.Collections.Generic.List<string>(microphones));
        debugDropdown.value = System.Array.IndexOf(microphones, selectedMicrophone);
        debugDropdown.onValueChanged.AddListener(ChangeMicrophone);
        
        debugButton.image.color = Color.red;
    }

    void DetectMicrophones()
    {
        microphones = Microphone.devices;        
        if (microphones.Length == 0)
        {
            Debug.LogError("No se encontraron micrófonos en el sistema.");
            hasMics = false;
            micEnabled = false;
            micToggle.interactable = false;
            return;
        }
        selectedMicrophone = PlayerPrefs.GetString("SelectedMicrophone", microphones[0]);
        StartMicrophone();
    }    

    void Debug_MicConfiguration()
    {
        if (!micActive) return;        

        float volume = GetMicVolume() * gainMultiplier;
        volumeSlider.value = volume;

        bool isAboveThreshold = volume > threshold;

        if (isAboveThreshold != wasBelowThreshold)
        {
            OnMicrophoneStateChanged?.Invoke(isAboveThreshold);
            wasBelowThreshold = isAboveThreshold;
        }

        if (debugMode)
        {
            debugSlider.value = volume;
            debugButton.image.color = isAboveThreshold ? Color.green : Color.red;
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
    public void ToggleMicrophone(bool isEnabled)
    {
        micEnabled = isEnabled;
        micDropdown.interactable = isEnabled;
        PlayerPrefs.SetInt("MicrophoneEnabled", isEnabled ? 1 : 0);
        if (isEnabled) StartMicrophone(); else StopMicrophone();
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

    //---------- SOUND ------------------------------------------------------------------------------------------------------------------

    void Start_SoundConfiguration()
    {
        LoadVolume("MasterVolume", masterSlider);
        LoadVolume("SFXVolume", sfxSlider);
        LoadVolume("MusicVolume", musicSlider);

        masterSlider.onValueChanged.AddListener(value => SetVolume("MasterVolume", value));
        sfxSlider.onValueChanged.AddListener(value => SetVolume("SFXVolume", value));
        musicSlider.onValueChanged.AddListener(value => SetVolume("MusicVolume", value));
    }

    private void LoadVolume(string parameterName, Slider slider)
    {
        float value = PlayerPrefs.GetFloat(parameterName, 1f);
        slider.value = value;
        SetVolume(parameterName, value);
    }
    private void SetVolume(string parameterName, float value)
    {
        float volume = (value > 0.0001f) ? Mathf.Log10(value) * 20 : -80f;
        audioMixer.SetFloat(parameterName, volume);
        PlayerPrefs.SetFloat(parameterName, value);
    }

    //---------- LANGUAGE ------------------------------------------------------------------------------------------------------------------

}
