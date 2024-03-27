using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TestSFX : MonoBehaviour
{
    [SerializeField] AudioSource soundFXObject;
    [SerializeField] AudioMixerGroup soundFXAudioMixerGroup;

    [SerializeField] AudioClip testSFX;

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.outputAudioMixerGroup = soundFXAudioMixerGroup;
        audioSource.clip = audioClip;

        Debug.Assert(0 <= volume && volume <= 1, "Out of range volume value!");
        audioSource.volume = volume;

        audioSource.PlayOneShot(audioSource.clip);

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

    }

    public void Test()
    {
        PlaySoundFXClip(testSFX, transform, 1f);
    }
}
