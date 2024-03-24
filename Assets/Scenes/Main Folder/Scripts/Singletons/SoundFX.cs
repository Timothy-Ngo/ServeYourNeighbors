// Author: Timothy Ngo
// https://www.youtube.com/watch?v=DU7cgVsU2rM&list=PLozZlrFOnyHKhwZWf9YozS3xowq494s2W&index=5
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundFX : MonoBehaviour
{

    [SerializeField] AudioSource soundFXObject;
    [SerializeField] AudioMixerGroup soundFXAudioMixerGroup;

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
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.outputAudioMixerGroup = soundFXAudioMixerGroup;
        audioSource.clip = audioClip;

        Debug.Assert(0 <= volume && volume <= 1, "Out of range volume value!");
        audioSource.volume = volume;

        audioSource.PlayOneShot(audioSource.clip);

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

    }
    public void PlayTimedSoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume, float timedLength)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.outputAudioMixerGroup = soundFXAudioMixerGroup;
        audioSource.clip = audioClip;

        Debug.Assert(0 <= volume && volume <= 1, "Out of range volume value!");
        audioSource.volume = volume;

        audioSource.PlayOneShot(audioSource.clip);


        Destroy(audioSource.gameObject, timedLength);

    }

    public AudioClip collectPaymentSFX;

    public void CollectPaymentSFX(float volume)
    {
        PlaySoundFXClip(collectPaymentSFX, transform, volume);
    }

    public AudioClip serveDishSFX;

    public void ServeDishSFX(float volume)
    {
        PlaySoundFXClip(serveDishSFX, transform, volume);
    }

    public AudioClip pickUpDishSFX;
    public void PickUpDishSFX(float volume)
    {
        PlaySoundFXClip(pickUpDishSFX, transform, volume);
    }

    public AudioClip pickUpIngredientSFX;
    public void PickUpIngredientSFX(float volume)
    {
        PlaySoundFXClip(pickUpIngredientSFX, transform, volume);
    }

    public AudioClip openIngredientBoxSFX;
    public void OpenIngredientBoxSFX(float volume)
    {
        PlaySoundFXClip(openIngredientBoxSFX, transform, volume);
    }

    public AudioClip closeIngredientBoxSFX;
    public void CloseIngredientBoxSFX(float volume)
    {
        PlaySoundFXClip(closeIngredientBoxSFX, transform, volume);
    }

    public AudioClip qtEventSFX;
    public void QTEventSFX(float volume)
    {
        PlaySoundFXClip(qtEventSFX, transform, volume);
    }

    public AudioClip tomatoDishPrepSFX;

    public void TomatoDishPrepSFX(float volume, float timedLength)
    {
        PlayTimedSoundFXClip(tomatoDishPrepSFX, transform, volume, timedLength);
    }

    public AudioClip lettuceDishPrepSFX;

    public void LettuceDishPrepSFX(float volume, float timedLength)
    {
        PlayTimedSoundFXClip(lettuceDishPrepSFX, transform, volume, timedLength);
    }


    public AudioClip finishedDishSFX;
    public void FinishedDishSFX(float volume)
    {
        PlaySoundFXClip(finishedDishSFX, transform, volume);        
    }

    public AudioClip throwAwaySFX;

    public void ThrowAwaySFX(float volume)
    {
        PlaySoundFXClip(throwAwaySFX, transform, volume);
    }

    public AudioClip addMSGSFX;

    public void AddMSGSFX(float volume)
    {
        PlaySoundFXClip(addMSGSFX, transform, volume);
    }
}
