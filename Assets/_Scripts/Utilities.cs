using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    public static void PlaySoundAndDestroy(AudioClip clip)
    {
        GameObject soundObject = new("TemporarySound");

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.playOnAwake = false;
        audioSource.Play();

        Destroy(soundObject, clip.length);
    }

    public static void DeleteAllChildrens(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Object.Destroy(child.gameObject);
        }
    }
}
