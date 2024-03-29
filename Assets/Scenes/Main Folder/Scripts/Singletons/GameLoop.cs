// Author: Timothy Ngo 
        
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameLoop : MonoBehaviour, IDataPersistence
{
    public static GameLoop inst;

    private void Awake()
    {
        inst = this;
    }

    [Header("-----TUTORIAL-----")]
    [SerializeField] public bool isTutorial = false;
    
    
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

    [Header("-----YELP REVIEWS-----")]
    [SerializeField] private GameObject reviewScreenObj;
    [SerializeField] private Reviews reviewsScript;
    [SerializeField] private Button continueReviewButton;

    [Header("-----UPGRADES SCREEN-----")]
    [SerializeField] private Button tablesUpgradeButton;

    [Header("-----SYN METER-----")]
    [SerializeField] SYNMeter synMeter;

    [Header("-----END GAME-----")] 
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject endGameScreen;

    [Header("-----PAUSE GAME-----")]
    [SerializeField] private GameObject pauseGameScreen;
    [SerializeField] private Button resumeButton;
    bool pauseScreenOpened = false;

    [Header("-----SETTINGS-----")]
    [SerializeField] private GameObject settingsScreen;

    [Header("-----LAYOUT SELECTION-----")]
    [SerializeField] private GameObject layoutScreen;
    private bool layoutSelected = false;

    public void LoadData(GameData data)
    {
        day = data.day;
        dailyOperationCost = data.dailyOperationCost;

        wavesPerDay = data.wavesPerDay;
        numFoodiesPerWave = data.numFoodiesPerWave;

        layoutSelected = data.layoutSelected;
    }

    public void SaveData(GameData data)
    {
        data.day = day;
        data.dailyOperationCost = dailyOperationCost;

        data.wavesPerDay = wavesPerDay;
        data.numFoodiesPerWave = numFoodiesPerWave;

        data.layoutSelected = layoutSelected;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!isTutorial)
        {
            // Initialize UI
            upgradeScreenObj.SetActive(false);
            endGameScreen.SetActive(false);
            pauseGameScreen.SetActive(false);
            playerUI.SetActive(true);
            settingsScreen.SetActive(false);
            layoutScreen.SetActive(false);

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
        if (upgradeScreenObj.activeInHierarchy)
        {
            if (!(EventSystem.current.currentSelectedGameObject is null))
            {
                Debug.Log(EventSystem.current.currentSelectedGameObject.ToString());
            }
        }
        if (!isTutorial)
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
                        StartCoroutine(DelayedReviewMenu(endDayDelay));
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

            /*
            // if a layout hasn't been selected, show the layout screen and pause time
            if (!layoutSelected)
            {
                Time.timeScale = 0;
                layoutScreen.SetActive(true);

            }
            */
        }

        if (Input.GetKeyDown(InputSystem.inst.pauseKey) && !upgradeScreenObj.activeInHierarchy && !reviewScreenObj.activeInHierarchy)
        {
            if (pauseScreenOpened)
            {
                DisablePauseScreen();
            }
            else
            {
                EnablePauseScreen();
            }

        }
    }
    /*
    // set bool flag to true and unpause time
    public void SetLayoutSelectedTrue()
    {
        layoutSelected = true;
        Time.timeScale = 1;
        layoutScreen.SetActive(false);
    }
    */

    public void EnablePauseScreen()
    {
        pauseGameScreen.SetActive(true);
        pauseScreenOpened = true;
        Time.timeScale = 0; // freezes gameplay
        resumeButton.Select();
    }

    public void DisablePauseScreen()
    {
        pauseGameScreen.SetActive(false);
        pauseScreenOpened = false;
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        if(!isTutorial)
        {
            endGameScreen.SetActive(true);
            playerUI.SetActive(false);
            // TODO: Add sfx
            // TODO: Stop player movement
        }
    }
    public void StartNewDay() // Starts a new day after player is done with their upgrades menu 
    {
        day++;
        dayText.text = day.ToString();
        if (day % 3 == 0) // Every 3rd day, increase amount of waves and operations cost
        {
            wavesPerDay++;
            dailyOperationCost += 15;
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

        // decay SYN Meter
        synMeter.DecaySYN();

        UpdateObserver();
        Player.inst.Activate();

        // Reapply obstacles
        FoodieSystem.inst.pathfinding.UpdateObstacles();

        // Save game data
        SaveSystem.inst.SaveGame();
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
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        SceneManager.LoadScene("Main Menu");
    }

    public void PayOperationsCost()
    {
        Debug.Assert(Currency.inst.AbleToWithdraw(dailyOperationCost));
        Currency.inst.Withdraw(dailyOperationCost);
        //Upgrades.inst.EnableUpgradeButtons();
    }

    public void UpgradeMenu()
    {
        reviewScreenObj.SetActive(false);
        upgradeScreenObj.SetActive(true);
        EventSystem.current.firstSelectedGameObject = tablesUpgradeButton.gameObject;
        EventSystem.current.SetSelectedGameObject(tablesUpgradeButton.gameObject);
        Player.inst.Deactivate();
    }

    IEnumerator DelayedReviewMenu(float delay) 
    {
        yield return new WaitForSeconds(delay);
        reviewScreenObj.SetActive(true);
        reviewsScript.Launch();
        EventSystem.current.firstSelectedGameObject = continueReviewButton.gameObject;
        EventSystem.current.SetSelectedGameObject(continueReviewButton.gameObject);
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

    public void ActivateTutorial()
    {
        isTutorial = true;
    }

    public void DeactivateTutorial()
    {
        isTutorial = false;
    }

    public bool IsEndScreenActive()
    {
        return endGameScreen.activeSelf;
    }
}
