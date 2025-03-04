using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private Transform parentTransform;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static void PlaySoundAndDestroy(AudioClip clip)
    {
        if (Instance == null) return;

        GameObject soundObject = new("TemporarySound");
        if (Instance.parentTransform != null) soundObject.transform.SetParent(Instance.parentTransform);

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.playOnAwake = false;
        audioSource.outputAudioMixerGroup = Instance.sfxGroup;
        audioSource.Play();

        Destroy(soundObject, clip.length);
    }

}
