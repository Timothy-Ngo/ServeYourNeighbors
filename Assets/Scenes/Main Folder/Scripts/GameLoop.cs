using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    [SerializeField] private GameObject upgradeScreenObj;
    [SerializeField] private GameObject foodiesParentObj;

    [Header("-----DAY CYCLE-----")] [SerializeField]
    private int day = 1;

    public TextMeshProUGUI startNewDayDescription;
    [SerializeField] int dailyOperationCost = 50;
    
    
    [Header("-----FOODIE WAVE SETTINGS-----")]
    [SerializeField] private FoodieSpawner foodieSpawner;
    [SerializeField] private int wavesPerDay = 2;
    private int currentWaveCount;
    [SerializeField] private int numFoodiesAtStart = 3;
    private int numFoodiesPerWave;
    [Tooltip("Seconds between each wave of foodies")] public float waveInterval = 10;
    private float waveTimer;
    private bool finishedWaves = false;
    // Start is called before the first frame update
    void Start()
    {
        // Display price of operations cost to text
        startNewDayDescription.text += $" ({dailyOperationCost}g)";
        
        // Set up the day game loop
        upgradeScreenObj.SetActive(false);
        currentWaveCount = wavesPerDay;
        numFoodiesPerWave = numFoodiesAtStart;
        waveTimer = 0;
        finishedWaves = false;
    }

    // Update is called once per frame
    void Update()
    {
        // if all waves are finished
        if (currentWaveCount == 0)
        {
            if (!finishedWaves && foodiesParentObj.transform.childCount <= 1)
            {
                upgradeScreenObj.SetActive(true);
                finishedWaves = true;
            }
        }
        else
        {
            Debug.Assert(currentWaveCount > 0, $"Current wave is : {currentWaveCount}!");
            
            // if waveTimer is still ongoing, decrease timer
            if (waveTimer > 0)
            {
                waveTimer -= Time.deltaTime;
            }
            // else spawn new wave, reset waveTimer, and decrease currentWaveCount
            else 
            {
                foodieSpawner.SpawnWaveOf(numFoodiesPerWave);
                waveTimer = waveInterval;
                currentWaveCount--;
            }
        }
        

    }
    
    public void StartNewDay() // Starts a new day after player is done with their upgrades menu 
    {
        if (Currency.inst.AbleToWithdraw(dailyOperationCost))
        {
            Currency.inst.Withdraw(dailyOperationCost);
            day++;
            if (day % 2 == 0)
            {
                wavesPerDay++;
            }
            else if (day % 2 == 1)
            {
                numFoodiesAtStart++;
            }
            upgradeScreenObj.SetActive(false);
            currentWaveCount = wavesPerDay;
            waveTimer = waveInterval;
            finishedWaves = false;
            
        }
    }
}
