// Author: Timothy Ngo
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Currency : MonoBehaviour, IDataPersistence
{
    /// <summary>
    /// When utilizing currency, use this (singleton) instance.
    /// </summary>
    public static Currency inst;

    public TextMeshProUGUI currentGoldText;
    private void Awake()
    {
        inst = this;
    }

    public int startingGold;
    public int gold;

    public void LoadData(GameData data)
    {
        gold = data.gold;
        UpdateTextUI();
    }

    public void SaveData(GameData data)
    {
        data.gold = gold;
    }


    void Start()
    {
        //Deposit(startingGold);
        UpdateTextUI();
    }


    /// <summary>
    /// Add money to gold
    /// </summary>
    public void Deposit(int amount)
    {
        Debug.Assert(amount > 0, "Amount cannot be negative");
        gold += amount;
        UpdateTextUI();
    }

    /// <summary>
    /// Take money from players gold
    /// </summary>
    public void Withdraw(int amount)
    {
        Debug.Assert(amount > 0, "Amount cannot be negative");
        if (AbleToWithdraw(amount))
        {
            gold -= amount;
            UpdateTextUI();
        }
        else
        {
            // Utilize UI to notify user they don't have enough gold
            Debug.Log("Not enough funds");
        }
    }

    public void UpdateTextUI()
    {
        currentGoldText.text = gold.ToString();
        NotifyObservers();
    }

    public bool AbleToWithdraw(int amount)
    {
        return (gold - amount) >= 0;
    }

    public void NotifyObservers()
    {
        GameLoop.inst.UpdateObserver();
    }

}
