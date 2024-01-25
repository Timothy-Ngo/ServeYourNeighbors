// Author: Timothy Ngo
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Player : MonoBehaviour
{
    public static Player inst;

    private void Awake()
    {
        inst = this;
    }

    [Tooltip("Needs to be a game object with the obstacle script on it placed on a point for the player to spawn")]
    public GameObject playerSpawnObj;
    public Vector2 playerSpawn;

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
