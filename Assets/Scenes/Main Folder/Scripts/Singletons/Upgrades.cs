using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Upgrades : MonoBehaviour
{
    public static Upgrades inst;

    private void Awake()
    {
        inst = this;

        foreach (Transform transform in tablesParent.transform)
        {
            tables.Add(transform.gameObject);
        }
        tables[0].SetActive(true);
        for (int i = 1; i < tables.Count; i++)
        {
            tables[i].SetActive(false);
        }
        
        
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
    [Header("-----UPGRADES UI-----")]
    public GameObject upgradesScreen;

    public GameObject upgradeButtons;
    [SerializeField] private Button[] buttons;
    [SerializeField] private Color greyedOutColor;
    [SerializeField] private Color normalColor;
    
    public PlacementSystem placementSystem;

    [Header("-----TABLES UPGRADE-----")]
    public GameObject tablesParent;
    [SerializeField] private List<GameObject> tables;
    public int tablesUpgradeCost = 50;
    public TextMeshProUGUI tablesDescription;
    public bool tablePlacementMode = false;
    
    [Header("-----COOK STATIONS UPGRADE-----")]
    public GameObject cookStationsParent;
    [SerializeField] private List<GameObject> cookStations;
    public int cookStationsUpgradeCost = 50;
    public TextMeshProUGUI cookStationsDescription;
    public bool cookStationPlacementMode = false;
    
    [Header("-----SPEED BOOST UPGRADE-----")] 
    [SerializeField] private PlayerMovement playerMovement;
    public bool isGMO = false;
    public int speedBoostUpgradeCost = 100;
    public TextMeshProUGUI speedBoostDescription;
    
    [Header("-----DISTRACTION UPGRADES-----")]
    public bool hasAnimatronic = false;
    public bool hasHibachiChef = false;
    public int animatronicUpgradeCost = 100;
    public bool animatronicPlacementMode = false;

    public enum LayoutLevel
    {
        Shack = 0,
        Tavern = 1,
        Restaurant = 2
    }
    [Header("-----MAIN LAYOUT UPGRADES-----")]
    public LayoutLevel currentLayout = LayoutLevel.Shack;

    // Start is called before the first frame update
    void Start()
    {
        tablesDescription.text += $" ({tablesUpgradeCost}g)";
        speedBoostDescription.text += $" ({speedBoostUpgradeCost}g)";
        cookStationsDescription.text += $" ({cookStationsUpgradeCost}g)";

        buttons = upgradeButtons.GetComponentsInChildren<Button>();
    }

    private void Update()
    {
        
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

    public void UpdateTablesList()
    {
        tables.Clear();
        Debug.Log("cleared tables");
        foreach (Table table in tablesParent.GetComponentsInChildren<Table>())
        {
            tables.Add(table.gameObject);
        }
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
    public void Tables() // Increase the amount of tables
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

    public void TablesPlacementMode()
    {
        if (Currency.inst.AbleToWithdraw(tablesUpgradeCost))
        {
            Currency.inst.Withdraw(tablesUpgradeCost);
            tablePlacementMode = true; // Must be set true before enabling placement system
            placementSystem.isEnabled = true;
            UpdateTablesList();
            //NotifyObservers();
            CustomerPayments.inst.standardPayment += 10;
            Debug.Log("Bought a table upgrade");
        }
        else
        {
            Debug.Log("Insufficient funds for table upgrade");
        }
    }
    
    public void CookStations() // Increase the amount of tables
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
            cookStations[numOfActiveCookStations].SetActive(true);
            //NotifyObservers();
            Debug.Log("Bought a cook stations upgrade");
        }
        else
        {
            Debug.Log("Insufficient funds for cook stations upgrade");
        }
    }
    
    public void CookStationsPlacementMode() // Increase the amount of tables
    {

        if (Currency.inst.AbleToWithdraw(cookStationsUpgradeCost))
        {
            Currency.inst.Withdraw(cookStationsUpgradeCost);
            cookStationPlacementMode = true;
            placementSystem.isEnabled = true;
            //NotifyObservers();
            Debug.Log("Bought a cook stations upgrade");
        }
        else
        {
            Debug.Log("Insufficient funds for cook stations upgrade");
        }
    }

    public void BecomeGMO() // Player Speed boost
    {
        if (isGMO)
        {
            Debug.Log("Say NO to drugs!");
            return;
        }

        if (Currency.inst.AbleToWithdraw(speedBoostUpgradeCost))
        {
            Currency.inst.Withdraw(speedBoostUpgradeCost);
            playerMovement.speed += playerMovement.speed / 2; 
            isGMO = true;
            Debug.Log("dRUgS ArE goOd, iTs nOT LiKE itS gOinG to KiLl yA");
        }
        else
        {
            Debug.Log("Insufficient funds for table upgrade");
        }
    }

    public void AnimatronicStand(int cost) // Animatronic Distraction
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
             Currency.inst.Withdraw(animatronicUpgradeCost);
             animatronicPlacementMode = true; 
             placementSystem.isEnabled = true;
             //NotifyObservers(); // Make sure distraction system has reference to animatronic
        }
        else
        {
            Debug.Log("Insufficient funds for animatronic upgrade");
        }
    }

    public void ImproveLayout(int cost) // Uprades main layout 
    {
        int scaledCost = cost + (100 * ((int)currentLayout));
        switch(currentLayout)
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
