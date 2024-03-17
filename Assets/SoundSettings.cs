
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

//https://www.youtube.com/watch?v=DU7cgVsU2rM&list=PLozZlrFOnyHKhwZWf9YozS3xowq494s2W&index=5
public class SoundSettings : MonoBehaviour, ISettingsDataPersistence
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioClip testSoundFX;

    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundFXSlider;

    public void Start()
    {
        LoadData(SaveSystem.inst.settingsData); // This does not work if called in any function that is called before Start() upon starting up game
    }

    public void LoadData(SettingsData data)
    {
        Debug.Log("loading audio data");
        masterSlider.value = data.masterValue;
        musicSlider.value = data.musicValue;
        soundFXSlider.value = data.audioFXValue;
        
        SetMaster(data.masterValue);
        SetMusic(data.musicValue);
        SetSoundFX(data.audioFXValue);
        

        Debug.Log(masterSlider.value);
        Debug.Log(musicSlider.value);
        Debug.Log(soundFXSlider.value);
    }

    public void SaveData(SettingsData data)
    {
        Debug.Log("saving audio data");
        Debug.Log(masterSlider.value);
        Debug.Log(musicSlider.value);
        Debug.Log(soundFXSlider.value);
        data.masterValue = masterSlider.value;
        data.musicValue = musicSlider.value;
        data.audioFXValue = soundFXSlider.value;
    }

    public void Update()
    {
    }
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
