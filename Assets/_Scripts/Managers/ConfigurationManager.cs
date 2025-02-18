using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfigurationManager : MonoBehaviour
{
    //[Header("Language")]

    //[Header("Sound")]
    
    [Header("Microphone")]
    [SerializeField] string selectedMicrophone;
    [SerializeField] TMP_Dropdown microphoneDropdown;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Button calibrateButton;
    private float sensitivityThreshold = 0.1f;
    private AudioClip micClip;
    private bool isCalibrating = false;

    void Start()
    {
        StartMicrophoneConfiguration();
    }

    //---------- LANGUAGE ------------------------------------------------------------------------------------------------------------------

    //---------- SOUND ------------------------------------------------------------------------------------------------------------------

    //---------- MICROPHONE ------------------------------------------------------------------------------------------------------------------

    void StartMicrophoneConfiguration()
    {
        DetectMicrophones();
        LoadSettings();
        if (calibrateButton != null)
            calibrateButton.onClick.AddListener(CalibrateMicrophone);

        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(ChangeSensitivity);
    }

    void DetectMicrophones()
    {
        string[] microphones = Microphone.devices;
        if (microphones.Length == 0)
        {
            Debug.LogError("No se encontraron micrófonos en el sistema.");
            return;
        }

        microphoneDropdown.ClearOptions();
        microphoneDropdown.AddOptions(new System.Collections.Generic.List<string>(microphones));

        selectedMicrophone = PlayerPrefs.GetString("SelectedMicrophone", microphones[0]);
        microphoneDropdown.value = System.Array.IndexOf(microphones, selectedMicrophone);
        microphoneDropdown.onValueChanged.AddListener(ChangeMicrophone);
    }

    void ChangeMicrophone(int index)
    {
        string[] microphones = Microphone.devices;
        if (index >= 0 && index < microphones.Length)
        {
            selectedMicrophone = microphones[index];
            PlayerPrefs.SetString("SelectedMicrophone", selectedMicrophone);
            PlayerPrefs.Save();
        }
    }

    void ChangeSensitivity(float value)
    {
        sensitivityThreshold = value;
        PlayerPrefs.SetFloat("SensitivityThreshold", sensitivityThreshold);
        PlayerPrefs.Save();
    }
    void LoadSettings()
    {
        sensitivityThreshold = PlayerPrefs.GetFloat("SensitivityThreshold", 0.1f);
        if (sensitivitySlider != null)
            sensitivitySlider.value = sensitivityThreshold;
    }

    public void CalibrateMicrophone()
    {
        if (isCalibrating) return;

        isCalibrating = true;
        Debug.Log("Calibrando micrófono...");

        StartCoroutine(CalibrationRoutine());
    }
    private IEnumerator CalibrationRoutine()
    {
        if (Microphone.IsRecording(selectedMicrophone))
            Microphone.End(selectedMicrophone);

        micClip = Microphone.Start(selectedMicrophone, true, 2, 44100);
        yield return new WaitForSeconds(2);

        float maxVolume = GetAverageVolume(micClip);
        sensitivityThreshold = Mathf.Clamp(maxVolume * 2, 0.05f, 1f); // Ajusta el umbral según el ruido detectado

        Debug.Log($"Calibración completada. Nuevo umbral: {sensitivityThreshold}");

        if (sensitivitySlider != null)
            sensitivitySlider.value = sensitivityThreshold;

        PlayerPrefs.SetFloat("SensitivityThreshold", sensitivityThreshold);
        PlayerPrefs.Save();

        isCalibrating = false;
    }
    private float GetAverageVolume(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        float sum = 0f;
        foreach (float sample in samples)
        {
            sum += Mathf.Abs(sample);
        }

        return sum / samples.Length;
    }

    public float GetSensitivityThreshold()
    {
        return sensitivityThreshold;
    }
    public string GetSelectedMicrophone()
    {
        return selectedMicrophone;
    }

}
