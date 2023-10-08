using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Upgrades : MonoBehaviour
{

    [Header("Table Upgrade")]
    public GameObject tablesParent;
    [SerializeField] private List<GameObject> tables;

    [Header("Speed Boost Upgrade")]
    public bool isGMO = false;

    [Header("Distraction Upgrades")]
    public bool hasAnimatronic = false;
    public bool hasHibachiChef = false;

    public enum LayoutLevel
    {
        Shack = 0,
        Tavern = 1,
        Restaurant = 2
    }
    [Header("Main Layout Upgrades")]
    public LayoutLevel currentLayout = LayoutLevel.Shack;    

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform transform in tablesParent.transform)
        {
            tables.Add(transform.gameObject);
        }
        tables[0].SetActive(true);
        for (int i = 1; i < tables.Count; i++)
        {
            tables[i].SetActive(false);
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

    // ------------------------

    // -----UPGRADE IMPLEMENTATIONS-----
    public void Tables(int cost) // Increase the amount of tables
    {
        int numOfActiveTables = GetNumOfActiveTables();
        if (numOfActiveTables == tables.Count)
        {
            Debug.Log("All tables have been bought");
            return;
        }

        if (Currency.inst.AbleToWithdraw(cost))
        {
            Currency.inst.Withdraw(cost);
            tables[numOfActiveTables].SetActive(true);
            Debug.Log("Bought a table upgrade");
        }
        else
        {
            Debug.Log("Insufficient funds for table upgrade");
        }
    }

    public void BecomeGMO(int cost) // Player Speed boost
    {
        if (isGMO)
        {
            Debug.Log("Say NO to drugs!");
            return;
        }

        if (Currency.inst.AbleToWithdraw(cost))
        {
            Currency.inst.Withdraw(cost);
            Time.timeScale = 2; // TODO: Temporary code to simulate speed boost of player. Must be replaced in the future.
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
}
