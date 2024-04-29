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
    public Vector2 playerSpawn;
    public GameObject layout1Spawn;
    public GameObject layout2Spawn;
    public GameObject layout3Spawn;



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
        playerSpawn = new Vector2();
        int layout = Layout.inst.layout;
        if (layout == 1)
        {
            playerSpawn = layout1Spawn.transform.position;
        }
        else if (layout == 2)
        {
            playerSpawn = layout2Spawn.transform.position;
        }
        else if (layout == 3)
        {
            playerSpawn = layout3Spawn.transform.position;
        }
        else
        {
            Debug.LogError("Could not find player spawn");

        }
        // Initialize player spawn
        Activate();
    }

    public void MoveToSpawnPoint()
    {
        int layout = Layout.inst.layout;
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
