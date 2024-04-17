// Author: Timothy Ngo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    public static Music inst;
    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("inst == null");
        }
        else if (inst != this || SceneManager.GetActiveScene().name == "Main Scene" || SceneManager.GetActiveScene().name == "Tutorial")
        {
            Destroy(inst.gameObject);
            Debug.Log("inst != this || ScemeNager.GetActiveScene().name == mainscene ");
        }

        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            PlayMainMenuBackgroundMusic();
        }
        else if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            PlayTutorialBackgroundMusic();
        }
        else
        {
            PlayRandomMusic();
        }
    }

    [SerializeField] AudioSource[] backgroundMusic;
    [SerializeField] AudioSource mainMenuBackgroundMusic;
    [SerializeField] AudioSource tutorialBackgroundMusic;



    public void Start()
    {

    }

    public void PlayRandomMusic()
    {
        backgroundMusic = GetComponentsInChildren<AudioSource>();
        int index = Random.Range(0, backgroundMusic.Length);
        backgroundMusic[index].Play();
    }

    public void PlayMainMenuBackgroundMusic()
    {
        mainMenuBackgroundMusic.Play();
    }

    public void PlayTutorialBackgroundMusic()
    {
        tutorialBackgroundMusic.Play();
    }



}
