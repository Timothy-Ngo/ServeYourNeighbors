// Author: Timothy Ngo 
        
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
    [SerializeField] private TextMeshProUGUI dayText;

    public TextMeshProUGUI payDescription;
    [SerializeField] int dailyOperationCost = 50;
    public float endDayDelay = 1.5f;
    
    [Header("-----FOODIE WAVE SETTINGS-----")]
    [SerializeField] private GameObject foodiesParentObj;
    [SerializeField] private FoodieSpawner foodieSpawner;
    [SerializeField] private int wavesPerDay = 2;
    private int currentWaveCount;
    [SerializeField] private int numFoodiesPerWave = 3;
    private int currentNumFoodiesPerWave;
    [Tooltip("Seconds between each wave of foodies")] public float waveInterval = 10;
    private float waveTimer;
    private bool finishedWaves = false;
    [SerializeField] private TextMeshProUGUI foodieCountText;
    private int numFoodiesCount;

    [Header("-----CURRENCY UI-----")]
    [SerializeField] private GameObject upgradeScreenObj;
    [SerializeField] private TextMeshProUGUI operationCostText;
    [SerializeField] private Color opCostsAchievedColor;
    [SerializeField] private Color opCostsNotAchievedColor;


    [Header("-----END GAME-----")] 
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject endGameScreen;

    [Header("-----PAUSE GAME-----")]
    [SerializeField] private GameObject pauseGameScreen;
    bool pauseScreenOpened = false;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize UI
        upgradeScreenObj.SetActive(false);
        endGameScreen.SetActive(false);
        pauseGameScreen.SetActive(false);
        playerUI.SetActive(true);
        
        // Display current day's operation cost goal for the player to reach
        operationCostText.text = dailyOperationCost.ToString();
        operationCostText.color = opCostsNotAchievedColor;
        // Display price of operations cost to text
        payDescription.text = $"You have paid ({dailyOperationCost}g) towards your operations cost.";
        
        // Set up the day game loop
        dayText.text = day.ToString();
        currentWaveCount = wavesPerDay;
        currentNumFoodiesPerWave = numFoodiesPerWave;
        waveTimer = 0;
        finishedWaves = false;

        // display number of foodies for the day
        numFoodiesCount = currentWaveCount * currentNumFoodiesPerWave;
        foodieCountText.text = numFoodiesCount.ToString();

    }

    // DEBUGGING
    void PrintData()
    {
        Debug.Log($"currentWaveCount: {currentWaveCount}");
        Debug.Log($"currentNumFoodiesPerWave: {currentNumFoodiesPerWave}"); 
    }
    // Update is called once per frame
    void Update()
    {
        // if all waves are finished
        if (currentWaveCount == 0)
        {
            // if day cycle is done
            if (!finishedWaves && foodiesParentObj.transform.childCount <= 1 && Player.inst.gameObject.GetComponentInChildren<Foodie>() == null)
            {
                if (Currency.inst.AbleToWithdraw(dailyOperationCost))
                {
                    PayOperationsCost();
                    finishedWaves = true;  
                    StartCoroutine(DelayedUpgradeMenu(endDayDelay));
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
                foodieSpawner.SpawnWaveOf(currentNumFoodiesPerWave);
                waveTimer = waveInterval;
                currentWaveCount--;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // toggles pause screen open/close
            pauseGameScreen.SetActive(!pauseScreenOpened);
            pauseScreenOpened = !pauseScreenOpened;
            if (pauseScreenOpened)
                Time.timeScale = 0; // freezes gameplay
            else
                Time.timeScale = 1;
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
        dayText.text = day.ToString();
        if (day % 3 == 0) // Every 3rd day, increase amount of waves and operations cost
        {
            wavesPerDay++;
            dailyOperationCost += 10;
        }
        else if (day % 5 == 1) // Every 5th day, increase number of foodies 
        {
            currentNumFoodiesPerWave++;
        }
        // Display current day's operation cost goal for the player to reach
        operationCostText.text = dailyOperationCost.ToString();
        operationCostText.color = opCostsNotAchievedColor;
        
        // Reset items in scene
        ResetItemsInScene();
        // Reset distraction 
        if (DistractionSystem.inst.animatronicDistraction != null)
        {
            DistractionSystem.inst.animatronicDistraction.ResetCharges();
        }
        // Close upgrade screen
        upgradeScreenObj.SetActive(false);
        // Reset wave count
        currentWaveCount = wavesPerDay;
        
        // Reset wave interval
        waveTimer = waveInterval;
        
        // Reset finished Waves
        finishedWaves = false;

        // reset display and number of foodies for the day
        numFoodiesCount = currentWaveCount * currentNumFoodiesPerWave;
        foodieCountText.text = numFoodiesCount.ToString();

        UpdateObserver();
        Player.inst.Activate();

        // Reapply obstacles
        FoodieSystem.inst.pathfinding.UpdateObstacles();
    }
    public void UpdateObserver()
    {
        if (Currency.inst.gold >= dailyOperationCost)
        {
            operationCostText.color = opCostsAchievedColor;
        }
        payDescription.text = $"You have paid ({dailyOperationCost}g) towards your operations cost.";
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
        //Upgrades.inst.EnableUpgradeButtons();
    }

    IEnumerator DelayedUpgradeMenu(float delay)
    {
        yield return new WaitForSeconds(delay);
        upgradeScreenObj.SetActive(true);
        Player.inst.Deactivate();
    }

    void ResetItemsInScene()
    {
        if (PickupSystem.inst.isHoldingItem())
        {
            PickupSystem.inst.DestroyItem();
        }

        foreach (GameObject cookStation in Upgrades.inst.cookStations)
        {
            GameObject dish = cookStation.GetComponent<Cooking>().dish;
            if (dish)
            {
                Destroy(dish);
                cookStation.GetComponent<Cooking>().ResetCooktop();
            }
        }

        foreach (GameObject counter in Upgrades.inst.counterObjs)
        {
            counter.GetComponent<Counter>().ResetCounter();
        }

        GameObject msgItem = Upgrades.inst.grinder;
        if (msgItem)
        {
            Destroy(msgItem.GetComponent<Grinder>().msgObject);
            msgItem.GetComponent<Grinder>().TakeMSG();
        }
    }

    public void DecrementFoodieCountText()
    {
        numFoodiesCount--;
        foodieCountText.text = numFoodiesCount.ToString();
    }
}
