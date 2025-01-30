using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] int currentTrackIndex = 1;
    
    [Header("Configuration")]
    [SerializeField] float transitionDuration = 0.25f;

    [Header("Music Tracks")]
    public List<AudioClip> musicTracks;
    private List<AudioSource> audioSources;

    private void Awake()
    {
        audioSources = new List<AudioSource>();

        for (int i = 0; i < musicTracks.Count; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = musicTracks[i];
            source.loop = true;
            source.playOnAwake = false;
            source.volume = (i == currentTrackIndex) ? 0.5f : 0f;
            source.Play();
            audioSources.Add(source);
        }
    }

    public void TransitionToTrack(int newTrackIndex)
    {
        if (newTrackIndex < 0 || newTrackIndex >= audioSources.Count || newTrackIndex == currentTrackIndex)
        {
            Debug.LogWarning("Invalid track index or already playing this track.");
            return;
        }
        StartCoroutine(TransitionRoutine(newTrackIndex));
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

            activeSource.volume = Mathf.Lerp(1f, 0f, progress);
            targetSource.volume = Mathf.Lerp(0f, 1f, progress);

            yield return null;
        }
        activeSource.volume = 0f;
        targetSource.volume = 1f;

        currentTrackIndex = newTrackIndex;
    }

    public void SetMusicTrack(int index)
    {
        currentTrackIndex = index;
        TransitionToTrack(index);
        Debug.Log(currentTrackIndex);
    }
    public void AddMusicTrack()
    {
        TransitionToTrack(currentTrackIndex++);
    }
    public void SubstractMusicTrack()
    {
        TransitionToTrack(currentTrackIndex--);
    }
}
