// Author: Timothy Ngo
// https://www.youtube.com/watch?v=DU7cgVsU2rM&list=PLozZlrFOnyHKhwZWf9YozS3xowq494s2W&index=5
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX : MonoBehaviour
{

    [SerializeField] private AudioSource soundFXObject;

    public static SoundFX inst;
    // Start is called before the first frame update
    void Start()
    {
        if (inst == null)
        {
            inst = this;
        }
    }



    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position , Quaternion.identity);

        audioSource.clip = audioClip;

        Debug.Assert(0 <= volume && volume <= 1, "Out of range volume value!");
        audioSource.volume = volume;

        audioSource.PlayOneShot(audioSource.clip);

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

    }
}
