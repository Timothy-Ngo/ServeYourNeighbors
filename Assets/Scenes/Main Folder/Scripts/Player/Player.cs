// Author: Timothy Ngo
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Player : MonoBehaviour, IDataPersistence
{
    public static Player inst;

    public List<Sprite> outfits;
    public int outfit = 0;
    public SpriteRenderer outfitSR;
    public string outfitName;
    public GameObject outfitObject;

    private void Awake()
    {
        inst = this;
    }

    [Tooltip("Needs to be a game object with the obstacle script on it placed on a point for the player to spawn")]
    public GameObject playerSpawnObj;
    public Vector2 playerSpawn;


    public void LoadData(GameData data)
    {
        outfit = data.outfit;
        outfitSR.sprite = outfits[outfit];

        // animate outfit
        outfitName = "outfit" + outfit.ToString();
        outfitObject.GetComponent<Animator>().Play(outfitName);
        gameObject.GetComponent<Animator>().Play("Idle");
    }

    public void SaveData(GameData data)
    {
        data.outfit = outfit;
    }

    void Start()
    {
        playerSpawn = playerSpawnObj.transform.position;
        // Initialize player spawn
        Activate();
    }

    public void MoveToSpawnPoint()
    {
        transform.position = playerSpawn;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        MoveToSpawnPoint();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }


    

}
