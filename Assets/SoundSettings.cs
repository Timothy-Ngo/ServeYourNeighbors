
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
//https://www.youtube.com/watch?v=DU7cgVsU2rM&list=PLozZlrFOnyHKhwZWf9YozS3xowq494s2W&index=5
public class SoundSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioClip testSoundFX;
    public void SetMaster(float level)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(level) * 20f);
    }
    public void SetSoundFX(float level)
    {
        audioMixer.SetFloat("SoundFX", Mathf.Log10(level) * 20f);
    }

    public void TestSoundFX()
    {
        SoundFX.inst.PlaySoundFXClip(testSoundFX, transform, 1f);
    }

    public void SetMusic(float level)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(level) * 20f);
    }
}
