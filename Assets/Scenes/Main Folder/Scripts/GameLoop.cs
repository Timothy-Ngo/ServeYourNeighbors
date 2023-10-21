using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    [SerializeField] private GameObject upgradeScreenObj;
    [SerializeField] private GameObject foodiesParentObj;
    
    [Header("-----FOODIE WAVE SETTINGS-----")]
    [SerializeField] private FoodieSpawner foodieSpawner;
    public int numOfWaves = 2;

    private int currentWaveCount;
    [Tooltip("Seconds between each wave of foodies")]
    public float waveInterval = 10;

    private bool finishedWaves = false;

    private float waveTimer;
    // Start is called before the first frame update
    void Start()
    {
        upgradeScreenObj.SetActive(false);
        currentWaveCount = numOfWaves;
        foodieSpawner.SpawnWaveOfFoodies();
        waveTimer = waveInterval;
        currentWaveCount--;
    }

    // Update is called once per frame
    void Update()
    {
        if (!finishedWaves)
        {
            waveTimer -= Time.deltaTime;
        }
        else
        {
            if (foodiesParentObj.transform.childCount <= 1)
            {
                upgradeScreenObj.SetActive(true);
            }
        }
        
        if (currentWaveCount >= 0 && waveTimer <= 0)
        {
            foodieSpawner.SpawnWaveOfFoodies();
            waveTimer = waveInterval;
            currentWaveCount--;
            if (currentWaveCount == 0)
            {
                finishedWaves = true;
                upgradeScreenObj.SetActive(false);
            }
        }
    }
}
