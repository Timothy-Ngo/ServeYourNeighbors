// Timothy Ngo 10/21

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameLoop : MonoBehaviour
{
    public static GameLoop inst;

    private void Awake()
    {
        inst = this;
    }
    
    
    [Header("-----DAY CYCLE-----")] [SerializeField]
    private int day = 1;

    public TextMeshProUGUI payDescription;
    [SerializeField] int dailyOperationCost = 50;
    
    
    [Header("-----FOODIE WAVE SETTINGS-----")]
    [SerializeField] private GameObject foodiesParentObj;
    [SerializeField] private FoodieSpawner foodieSpawner;
    [SerializeField] private int wavesPerDay = 2;
    private int currentWaveCount;
    [SerializeField] private int numFoodiesAtStart = 3;
    private int numFoodiesPerWave;
    [Tooltip("Seconds between each wave of foodies")] public float waveInterval = 10;
    private float waveTimer;
    private bool finishedWaves = false;
    
    [Header("-----CURRENCY UI-----")]
    [SerializeField] private GameObject upgradeScreenObj;
    [SerializeField] private TextMeshProUGUI operationCostText;
    [SerializeField] private Color opCostsAchievedColor;
    [SerializeField] private Color opCostsNotAchievedColor;


    [Header("-----END GAME-----")] 
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject endGameScreen;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize UI
        upgradeScreenObj.SetActive(false);
        endGameScreen.SetActive(false);
        playerUI.SetActive(true);
        
        // Display current day's operation cost goal for the player to reach
        operationCostText.text = $"Goal:\n{dailyOperationCost.ToString()}";
        operationCostText.color = opCostsNotAchievedColor;
        // Display price of operations cost to text
        payDescription.text += $" ({dailyOperationCost}g)";
        
        // Set up the day game loop
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
            // if day cycle is done
            if (!finishedWaves && foodiesParentObj.transform.childCount <= 1)
            {
                if (Currency.inst.AbleToWithdraw(dailyOperationCost))
                {
                    upgradeScreenObj.SetActive(true);
                    Upgrades.inst.DisableUpgradeButtons();
                    finishedWaves = true;
                }
                else
                {
                    GameOver();
                }
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

    public void GameOver()
    {
        endGameScreen.SetActive(true);
        playerUI.SetActive(false);
        // TODO: Add sfx
        // TODO: Stop player movement
    }
    public void StartNewDay() // Starts a new day after player is done with their upgrades menu 
    {
        day++;
        if (day % 2 == 0)
        {
            wavesPerDay++;
            dailyOperationCost += 10;
            UpdateObserver();
        }
        else if (day % 2 == 1)
        {
            numFoodiesAtStart++;
        }
        // Display current day's operation cost goal for the player to reach
        operationCostText.text = $"Reach goal:\n{dailyOperationCost.ToString()}";
        operationCostText.color = opCostsNotAchievedColor;
        
        // Reset distraction 
        DistractionSystem.inst.animatronicDistraction.ResetCharges();
        // Close upgrade screen
        upgradeScreenObj.SetActive(false);
        // Reset wave count
        currentWaveCount = wavesPerDay;
        
        // Reset wave interval
        waveTimer = waveInterval;
        
        // Reset finished Waves
        finishedWaves = false;
        

    }
    public void UpdateObserver()
    {
        if (Currency.inst.gold >= dailyOperationCost)
        {
            operationCostText.color = opCostsAchievedColor;
        }
    }

    public void PlayAgain()
    {
        // TODO: This method should maybe? take the player back to the menu in the future 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void PayOperationsCost()
    {
        Debug.Assert(Currency.inst.AbleToWithdraw(dailyOperationCost));
        Currency.inst.Withdraw(dailyOperationCost);
        Upgrades.inst.EnableUpgradeButtons();
    }
}
