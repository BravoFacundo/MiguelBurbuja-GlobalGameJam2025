using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] int currentTrackIndex = 0;

    [Header("Configuration")]
    [SerializeField] float startVolume = .5f;
    [SerializeField] float transitionDuration = 0.25f;
    [SerializeField] AudioMixerGroup musicGroup;

    [Header("Music Tracks")]
    [SerializeField] List<AudioClip> musicTracks;
    [SerializeField] List<AudioSource> audioSources;

    private void Awake() => SetAudioSources();
    private void SetAudioSources()
    {
        audioSources = new List<AudioSource>(GetComponents<AudioSource>());
        for (int i = 0; i < audioSources.Count; i++)
        {
            audioSources[i].clip = musicTracks[i];
            audioSources[i].loop = true;
            audioSources[i].playOnAwake = false;
            audioSources[i].outputAudioMixerGroup = musicGroup;
            audioSources[i].volume = (i == currentTrackIndex) ? startVolume : 0f;
            audioSources[i].Play();
        }
    }

    public void SetMusicTrack(string track)
    {
        if (track == "Menu") StartCoroutine(TransitionRoutine(0));
        else if (track == "Game") StartCoroutine(TransitionRoutine(1));
    }
    private IEnumerator TransitionRoutine(int newTrackIndex)
    {
        float timer = 0f;
        AudioSource activeSource = audioSources[currentTrackIndex];
        AudioSource targetSource = audioSources[newTrackIndex];

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / transitionDuration;

            activeSource.volume = Mathf.Lerp(startVolume, 0f, progress);
            targetSource.volume = Mathf.Lerp(0f, startVolume, progress);

            yield return null;
        }
        activeSource.volume = 0f;
        targetSource.volume = startVolume;

        currentTrackIndex = newTrackIndex;
    }

    public void EnableTenseTrack() => StartCoroutine(FadeTenseTrack(audioSources[2], transitionDuration));
    public void DisableTenseTrack() => StartCoroutine(FadeTenseTrack(audioSources[2], 0f));
    private IEnumerator FadeTenseTrack(AudioSource track, float targetVolume)
    {
        float startVol = track.volume;
        float timer = 0f;

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            track.volume = Mathf.Lerp(startVol, targetVolume, timer / transitionDuration);
            yield return null;
        }
        track.volume = targetVolume;
    }
}
