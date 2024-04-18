// Author: Timothy Ngo
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Assertions;
using UnityEngine.UI;
public class Upgrades : MonoBehaviour, IDataPersistence
{
    public static Upgrades inst;

    private void Awake()
    {
        inst = this;


        // Initialize table objects
        foreach (Transform transform in tablesParent.transform)
        {
            tables.Add(transform.gameObject);
        }
        tables[0].SetActive(true);

        for (int i = 1; i < tables.Count; i++)
        {
            tables[i].SetActive(false);
        }

        // Initialize cook station objects
        foreach (Transform transform in cookStationsParent.transform)
        {
            cookStations.Add(transform.gameObject);
        }
        cookStations[0].SetActive(true);
        for (int i = 1; i < cookStations.Count; i++)
        {
            cookStations[i].SetActive(false);
        }



    }

    // used for loading upgrades from save data
    [Header("-----PREFABS-----")]
    public GameObject tablePrefab;
    public GameObject cookStationPrefab;
    public GameObject counterPrefab;
    public GameObject animatronicPrefab;


    [Header("-----UPGRADES UI-----")]
    public GameObject upgradesScreen;

    public GameObject upgradeButtons;
    [SerializeField] private Button[] buttons;
    [SerializeField] private Color greyedOutColor;
    [SerializeField] private Color normalColor;

    public PlacementSystem placementSystem;

    [Header("-----TABLES UPGRADE-----")]
    public GameObject tablesParent;
    public List<GameObject> tables;
    public int numTables { get { return tables.Count; } }
    [SerializeField] int maxTables = 4;
    public int tablesUpgradeCost = 50;
    public TextMeshProUGUI tablesDescription;
    public bool tablePlacementMode = false;

    [Header("-----COOK STATIONS UPGRADE-----")]
    public GameObject cookStationsParent;
    [SerializeField] public List<GameObject> cookStations;
    public int numCookStations { get { return cookStations.Count; } }
    [SerializeField] int maxCookStations = 4;

    public int cookStationsUpgradeCost = 50;
    public TextMeshProUGUI cookStationsDescription;
    public bool cookStationPlacementMode = false;


    [Header("-----COUNTERS UPGRADE-----")]
    public GameObject counterParent;
    public List<GameObject> counterObjs;
    public int numCounters { get { return counterObjs.Count; } }
    [SerializeField] int maxCounters = 5;
    public int countersUpgradeCost = 50;
    public TextMeshProUGUI countersDescription;
    public bool counterPlacementMode = false;

    [Header("-----SPEED BOOST UPGRADE-----")]
    [SerializeField] private PlayerMovement playerMovement;
    public bool isGMO = false;
    public int speedBoostUpgradeCost = 100;
    public TextMeshProUGUI speedBoostDescription;

    [Header("-----DISTRACTION UPGRADES-----")]
    public Transform distractionParent;
    public bool hasAnimatronic = false;

    public bool hasHibachiChef = false;
    public int animatronicUpgradeCost = 100;
    [SerializeField] TextMeshProUGUI animatronicDescription;
    public bool animatronicPlacementMode = false;
    public Vector3 animatronicPosition;

    [Header("-----VISUAL UPGRADES-----")]
    public bool full = false;
    [SerializeField] Image visualBar; // drag and drop image in inspector
    public TextMeshProUGUI barDescription;
    public float fillAmount = 0f;
    public float tavernThreshold = 4; // number of upgrades needed to get tavern design
    public float restaurantThreshold = 8; // number of upgrades needed to get restaurant design
    private float upgradesCount = 0;
    public GameObject shackDesign;
    public GameObject tavernDesign;
    public GameObject restaurantDesign;
    private bool tavernAchieved = false;
    private bool restaurantAchieved = false;
    public SkillTree skillTreeScript;

    public enum LayoutLevel
    {
        Shack = 0,
        Tavern = 1,
        Restaurant = 2
    }
    [Header("-----MAIN LAYOUT UPGRADES-----")]
    public List<GameObject> starterItems;
    public List<GameObject> selectableItems; // Need to add the ingredient boxes, trashcan and grinder in through the inspector
    public int changeLayoutCost = 50;
    public bool changeLayoutMode = false;
    [SerializeField] TextMeshProUGUI changeLayoutDescription;
    public LayoutLevel currentLayout = LayoutLevel.Shack;

    [Header("-----CHANGE LAYOUT ORIGINAL POSITIONS-----")]
    Vector3 tomatoBoxPos;
    Vector3 lettuceBoxPos;
    Vector3 flourBoxPos;
    Vector3 grinderPos;
    List<Vector3> cookStationPos;
    List<Vector3> tablePos;
    Vector3 animatronicPos;
    Vector3 trashCanPos;

    public void SaveOldItemPositions()
    {
        cookStationPos = new List<Vector3>();
        tablePos = new List<Vector3>();
        foreach(GameObject obj in selectableItems)
        {
            if (obj.CompareTag("IngredientBox"))
            {
                if (obj.name.Contains("tomato"))
                {
                    tomatoBoxPos = obj.transform.position;
                }
                else if (obj.name.Contains("lettuce"))
                {
                    lettuceBoxPos = obj.transform.position;
                }
                else if (obj.name.Contains("flour"))
                {
                    flourBoxPos = obj.transform.position;
                }
            }
            else if (obj.CompareTag("Grinder"))
            {
                grinderPos = obj.transform.position;
            }
            else if (obj.CompareTag("Cooktop"))
            {
                cookStationPos.Add(obj.transform.position);
            }
            else if (obj.CompareTag("Table"))
            {
                tablePos.Add(obj.transform.position);
            }
            else if (obj.CompareTag("Distraction"))
            {
                animatronicPos = obj.transform.position;
            }
            else if (obj.CompareTag("TrashCan"))
            {
                trashCanPos = obj.transform.position;
            }
        }
    }

    public void RestoreItemPositions() // SaveOldItemPositions() must be called at when the positions you want them to be returned to 
    {
        int cookStationIndex = 0;
        int tableIndex = 0;
        foreach(GameObject obj in selectableItems)
        {
            if (obj.CompareTag("IngredientBox"))
            {
                if (obj.name.Contains("tomato"))
                {
                    obj.transform.position = tomatoBoxPos; 
                }
                else if (obj.name.Contains("lettuce"))
                {
                    obj.transform.position = lettuceBoxPos; 
                }
                else if (obj.name.Contains("flour"))
                {
                    obj.transform.position = flourBoxPos; 
                }
            }
            else if (obj.CompareTag("Grinder"))
            {
                obj.transform.position = grinderPos; 
            }
            else if (obj.CompareTag("Cooktop"))
            {
                obj.transform.position = cookStationPos[cookStationIndex++];
            }
            else if (obj.CompareTag("Table"))
            {
                obj.transform.position = tablePos[tableIndex++];
            }
            else if (obj.CompareTag("Distraction"))
            {
                obj.transform.position = animatronicPos; 
            }
            else if (obj.CompareTag("TrashCan"))
            {
                obj.transform.position = trashCanPos;
            }
        }
    }

    public GameObject grinder;

    // using saved positions, object upgrades are instantiated and placed in the world
    public void LoadData(GameData data)
    {
        // tables
        for (int i = 1; i < data.tablePositions.Count; i++)
        {
            GameObject table = Instantiate(tablePrefab, data.tablePositions[i], Quaternion.identity, tablesParent.transform);
            tables.Add(table);
        }

        // cook stations
        for (int i = 1; i < data.cookStationPositions.Count; i++)
        {
            GameObject cookStation = Instantiate(cookStationPrefab, data.cookStationPositions[i], Quaternion.identity, cookStationsParent.transform);
            cookStations.Add(cookStation);
        }

        // distraction

        // if there is a saved animatronic -- instantiate and place in world
        hasAnimatronic = data.hasAnimatronic;
        if (hasAnimatronic)
        {
            animatronicPosition = data.animatronicPosition;
            GameObject animatronic = Instantiate(animatronicPrefab, animatronicPosition, Quaternion.identity, distractionParent);
            DistractionSystem.inst.animatronicDistraction = animatronic.GetComponent<Distraction>();
        }


        // visual upgrades
        // number of upgrades is calculated when upgrade screen is open, so we don't need to save it

        tavernAchieved = data.tavernAchieved;
        restaurantAchieved = data.restaurantAchieved;

        if (tavernAchieved)
        {
            shackDesign.SetActive(false);
            tavernDesign.SetActive(true);
            restaurantDesign.SetActive(false);
            currentLayout = LayoutLevel.Tavern;
        }
        else if (restaurantAchieved)
        {
            shackDesign.SetActive(false);
            tavernDesign.SetActive(false);
            restaurantDesign.SetActive(true);
            currentLayout = LayoutLevel.Restaurant;
        }
        else
        {
            currentLayout = LayoutLevel.Shack;
        }
    }

    // the positions of the objects are saved to the file
    public void SaveData(GameData data)
    {
        // tables
        data.tablePositions.Clear();
        for (int i = 0; i < tables.Count; i++)
        {
            data.tablePositions.Add(tables[i].transform.position);
        }

        // cook stations
        data.cookStationPositions.Clear();
        for (int i = 0; i < cookStations.Count; i++)
        {
            data.cookStationPositions.Add(cookStations[i].transform.position);
        }


        // distraction
        // if there is an animatronic -- save the bool and its position
        if (hasAnimatronic)
        {
            data.hasAnimatronic = hasAnimatronic;
            data.animatronicPosition = animatronicPosition;
        }

        // visual upgrades
        data.tavernAchieved = tavernAchieved;
        data.restaurantAchieved = restaurantAchieved;
    }


    // Start is called before the first frame update
    void Start()
    {
        /*
        tablesDescription.text += $" ({tablesUpgradeCost}g)";
        cookStationsDescription.text += $" ({cookStationsUpgradeCost}g)";
        countersDescription.text += $"({countersUpgradeCost}g)";
        animatronicDescription.text += $"({animatronicUpgradeCost}g)";
        changeLayoutDescription.text += $"({changeLayoutCost}g)";
        speedBoostDescription.text += $" ({speedBoostUpgradeCost}g)"; // Deprecated

        buttons = upgradeButtons.GetComponentsInChildren<Button>();
        */
    }

    private void Update()
    {
        if (upgradesScreen.activeSelf)
        {
            upgradesCount = CalculateUpgradesNum();

            if (shackDesign.activeSelf)
            {
                barDescription.text = "Tavern Upgrade Progress: " + upgradesCount.ToString() + "/" + tavernThreshold.ToString();

                fillAmount = upgradesCount / tavernThreshold;

                if (fillAmount >= 1)
                {
                    shackDesign.SetActive(false);
                    tavernDesign.SetActive(true);
                    tavernAchieved = true;
                    currentLayout = LayoutLevel.Tavern;
                }
                else
                {
                    visualBar.fillAmount = fillAmount;
                }

            }
            else if (tavernDesign.activeSelf)
            {
                barDescription.text = "Restaurant Upgrade Progress: " + upgradesCount.ToString() + "/" + restaurantThreshold.ToString();

                fillAmount = upgradesCount / restaurantThreshold;

                if (fillAmount >= 1)
                {
                    tavernDesign.SetActive(false);
                    restaurantDesign.SetActive(true);
                    visualBar.fillAmount = 1;
                    restaurantAchieved = true;
                    currentLayout = LayoutLevel.Restaurant;
                }
                else
                {
                    visualBar.fillAmount = fillAmount;
                }
            }
            else
            {
                barDescription.text = "Restaurant Design Achieved!";
            }


        }
    }

    // -----HELPER METHODS-----
    public int GetNumOfActiveTables()
    {
        int numOfActiveTables = 0;
        foreach (GameObject gameObject in tables)
        {
            if (gameObject.activeSelf)
            {
                numOfActiveTables++;
            }
        }

        return numOfActiveTables;
    }

    public int GetNumOfActiveCookStations()
    {
        int numOfActiveCookStations = 0;
        foreach (GameObject gameObject in cookStations)
        {
            if (gameObject.activeSelf)
            {
                numOfActiveCookStations++;
            }
        }

        return numOfActiveCookStations;
    }


    public int GetNumOfActiveCounters()
    {
        int numOfActiveCounters = 0;
        foreach (GameObject gameObject in counterObjs)
        {
            if (gameObject.activeSelf)
            {
                numOfActiveCounters++;
            }
        }

        return numOfActiveCounters;
    }
    public void UpdateTablesList()
    {
        tables.Clear();
        Debug.Log("cleared tables");
        foreach (Table table in tablesParent.GetComponentsInChildren<Table>())
        {
            tables.Add(table.gameObject);
        }
    }

    public void UpdateCookStationsList()
    {
        cookStations.Clear();
        foreach (Cooking cooking in cookStationsParent.GetComponentsInChildren<Cooking>())
        {
            cookStations.Add(cooking.gameObject);
        }
    }

    public float CalculateUpgradesNum()
    {
        float upgradesNum = 0;

        upgradesNum += GetNumOfActiveTables() - 1;
        upgradesNum += GetNumOfActiveCookStations() - 1;

        if (hasAnimatronic)
        {
            upgradesNum++;
        }

        if (isGMO)
        {
            upgradesNum++;
        }

        int skillTreeUpgrades = skillTreeScript.GetNumActive();

        upgradesNum += skillTreeUpgrades;

        return upgradesNum;
    }

    // ------------------------


    //-----OBSERVABLE IMPLEMENTATION-----

    public void NotifyObservers() // Different from the standard observable implementation as we are utilizing singletons, so no need for registering and unregistering Observers for the time being
    {
        FoodieSystem.inst.GetCurrentSeats();
    }

    // -----UPGRADE IMPLEMENTATIONS-----

    public void ToggleUpgradesScreen()
    {
        upgradesScreen.SetActive(!upgradesScreen.activeSelf);
    }
    public void Tables() // Deprecated
    {
        if (Currency.inst.AbleToWithdraw(tablesUpgradeCost))
        {
            Currency.inst.Withdraw(tablesUpgradeCost);
            tables[GetNumOfActiveTables()].SetActive(true);
            NotifyObservers();
            CustomerPayments.inst.standardPayment += 10;
            Debug.Log("Bought a table upgrade");
        }
        else
        {
            Debug.Log("Insufficient funds for table upgrade");
        }
    }

    public void TablesPlacementMode() // Initializes placement mode for tables
    {
        if (Currency.inst.AbleToWithdraw(tablesUpgradeCost))
        {
            Debug.Assert(numTables <= maxTables, "# of tables is over the max amount.");
            Currency.inst.Withdraw(tablesUpgradeCost);
            tablePlacementMode = true; // Must be set true before enabling placement system
            placementSystem.isEnabled = true;
            UpdateTablesList();
            //NotifyObservers();
            CustomerPayments.inst.standardPayment += 10;
            if (numTables == maxTables - 1)
            {
                tablesDescription.gameObject.transform.parent.GetComponent<Button>().interactable = false;
                tablesDescription.text = "Max number of tables reached.";
            }
            Debug.Log("In table placement mode");
        }
        else
        {
            Debug.Log("Insufficient funds for table upgrade");
        }
    }

    public void CookStations() // Deprecated
    {
        int numOfActiveCookStations = GetNumOfActiveCookStations();
        if (numOfActiveCookStations == cookStations.Count)
        {
            Debug.Log("All cook stations have been bought");
            return;
        }

        if (Currency.inst.AbleToWithdraw(cookStationsUpgradeCost))
        {
            Currency.inst.Withdraw(cookStationsUpgradeCost);
            
            //NotifyObservers();
            Debug.Log("Bought a cook stations upgrade");
        }
        else
        {
            Debug.Log("Insufficient funds for cook stations upgrade");
        }
    }


    public void CookStationsPlacementMode()  // Initializes placement mode for cook stations
    {

        if (Currency.inst.AbleToWithdraw(cookStationsUpgradeCost))
        {
            Debug.Assert(numCookStations <= maxCookStations, "# of cook stations is over the max amount.");
            Currency.inst.Withdraw(cookStationsUpgradeCost);
            cookStationPlacementMode = true;
            placementSystem.isEnabled = true;
            UpdateCookStationsList();
            //NotifyObservers();
            if (numCookStations == maxCookStations - 1)
            {
                cookStationsDescription.gameObject.transform.parent.GetComponent<Button>().interactable = false;
                cookStationsDescription.text = "Max number of cook stations reached.";
            }
        }
        else
        {
            Debug.Log("Insufficient funds for cook stations upgrade");
        }
    }


    public void Counters() // Deprecated
    {
        if (Currency.inst.AbleToWithdraw(countersUpgradeCost))
        {
            Currency.inst.Withdraw(countersUpgradeCost);
            counterObjs[GetNumOfActiveCounters()].SetActive(true);
        }
        else
        {
            Debug.Log("Insufficient funds for counter upgrade");
        }
    }
    public void CountersPlacementMode() // Initializes placement mode for counters
    {
        if (Currency.inst.AbleToWithdraw(countersUpgradeCost))
        {
            Debug.Assert(numCounters <= maxCounters, "# of counters is over the max amount.");
            Currency.inst.Withdraw(countersUpgradeCost);
            counterPlacementMode = true;
            placementSystem.isEnabled = true;
            if (numCounters == maxCounters)
            {
                countersDescription.gameObject.transform.parent.GetComponent<Button>().interactable = false;
                countersDescription.text = "Max number of counters reached.";
            }
        }
        else
        {
            Debug.Log("Insufficient funds for counters upgrade");
        }
    }
    public void ChangeLayoutMode()
    {
        if (Currency.inst.AbleToWithdraw(changeLayoutCost))
        {
            Debug.Log("Bought Change layout");
            selectableItems.Clear();
            foreach (GameObject obj in starterItems)
            {
                selectableItems.Add(obj);
            }
            Currency.inst.Withdraw(changeLayoutCost);
            changeLayoutMode = true;
            placementSystem.isEnabled = true;
            // Find all selectable items for placement mode keyboard input (i.e. tables, cook stations, animatronics)
            foreach (Transform cookTransform in cookStationsParent.GetComponentInChildren<Transform>()) // Add cook stations as selectable
            {
                selectableItems.Add(cookTransform.gameObject);
            }
            foreach (Transform tableTransform in tablesParent.GetComponentInChildren<Transform>()) // Add tables as selectable
            {
                if (tableTransform.gameObject.GetComponent<Table>())
                {
                    selectableItems.Add(tableTransform.gameObject);
                }
            }
            foreach (Transform distractionTransform in distractionParent.GetComponentInChildren<Transform>()) // Add animatronic as selectable
            {
                selectableItems.Add(distractionTransform.gameObject);
            }

            foreach (GameObject item in selectableItems) // Make sure all obstacles default to not showing their visuals
            {
                Obstacle[] obstacles = item.GetComponentsInChildren<Obstacle>();
                foreach(Obstacle obstacle in obstacles)
                {
                    obstacle.showObstacle = false;
                }
            }
            SaveOldItemPositions();

        }
        else
        {
            Debug.Log("Insufficient funds for move item upgrade");
        }
    }

    public void BecomeGMO() // Deprecated
    {
        if (isGMO)
        {
            Debug.Log("Say NO to drugs!");
            return;
        }

        if (Currency.inst.AbleToWithdraw(speedBoostUpgradeCost))
        {
            Currency.inst.Withdraw(speedBoostUpgradeCost);
            //playerMovement.speed += playerMovement.speed / 2; 
            isGMO = true;
            Debug.Log("dRUgS ArE goOd, iTs nOT LiKE itS gOinG to KiLl yA");
        }
        else
        {
            Debug.Log("Insufficient funds for table upgrade");
        }
    }

    public void AnimatronicStand(int cost) // Deprecated
    {
        if (hasAnimatronic)
        {
            Debug.Log("Bite of '87 ain't happening anytime soon bud");
            return;
        }

        if (Currency.inst.AbleToWithdraw(cost))
        {
            Currency.inst.Withdraw(cost);
            // TODO: Set animatronic gameobject to active
            Debug.Log("Bought an animatronic");
        }
    }

    public void AnimatronicPlacementMode()
    {

        if (Currency.inst.AbleToWithdraw(animatronicUpgradeCost))
        {
            Debug.Assert(!hasAnimatronic, "# of animatronics is over the max amount.");
            Currency.inst.Withdraw(animatronicUpgradeCost);
            animatronicPlacementMode = true;
            placementSystem.isEnabled = true;
            animatronicDescription.gameObject.transform.parent.GetComponent<Button>().interactable = false;
            animatronicDescription.text = "Max number of animatronics reached.";

        }
        else
        {
            Debug.Log("Insufficient funds for animatronic upgrade");
        }
    }

    public void ImproveLayout(int cost) // Uprades main layout 
    {
        int scaledCost = cost + (100 * ((int)currentLayout));
        switch (currentLayout)
        {
            case LayoutLevel.Shack:
                if (Currency.inst.AbleToWithdraw(cost))
                {
                    Currency.inst.Withdraw(cost);
                    // TODO: Set Tavern layout game objects active here
                    currentLayout = LayoutLevel.Tavern;
                    Debug.Log("Layout Upgraded to Tavern");
                }
                else
                {
                    Debug.Log("Insufficient funds for layout upgrade");
                }
                break;
            case LayoutLevel.Tavern:
                if (Currency.inst.AbleToWithdraw(cost))
                {
                    Currency.inst.Withdraw(cost);
                    // TODO: Set Tavern layout game objects active here
                    currentLayout = LayoutLevel.Tavern;
                    Debug.Log("Layout Upgraded to Tavern");
                }
                else
                {
                    Debug.Log("Insufficient funds for layout upgrade");
                }
                break;
            case LayoutLevel.Restaurant: // This can be extendable 
                Debug.Log("Layout upgrades have been maxed out!");
                break;

        }
    }


    public void DisableUpgradeButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
            button.enabled = false;
        }
    }

    public void EnableUpgradeButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
            button.enabled = true;
        }
    }
}
