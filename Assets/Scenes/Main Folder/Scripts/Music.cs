using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    private AudioSource[] backgroundMusic;


    public void Start()
    {
        backgroundMusic = GetComponentsInChildren<AudioSource>();
        int index = Random.Range(0, backgroundMusic.Length);
        backgroundMusic[index].Play();
    }


}
