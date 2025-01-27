using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public List<AudioClip> musicClips;
    private List<AudioSource> audioSources;
    private int currentTrackIndex = 0;
    public float transitionDuration = 1f;

    private void Start()
    {
        audioSources = new List<AudioSource>();

        for (int i = 0; i < musicClips.Count; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = musicClips[i];
            source.loop = true;
            source.playOnAwake = false;
            source.volume = .5f;
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

    private System.Collections.IEnumerator TransitionRoutine(int newTrackIndex)
    {
        float timer = 0f;
        AudioSource activeSource = audioSources[currentTrackIndex];
        AudioSource targetSource = audioSources[newTrackIndex];

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / transitionDuration;

            // Reducir el volumen de la pista activa
            activeSource.volume = Mathf.Lerp(1f, 0f, progress);
            // Aumentar el volumen de la pista objetivo
            targetSource.volume = Mathf.Lerp(0f, 1f, progress);

            yield return null; // Esperar un frame
        }

        // Asegurarse de que los vol�menes est�n correctamente ajustados al finalizar
        activeSource.volume = 0f;
        targetSource.volume = 1f;

        currentTrackIndex = newTrackIndex;
    }
}
