using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Currency : MonoBehaviour
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

    void Start()
    {
        Deposit(startingGold);
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
        currentGoldText.text = "Gold:\n" + gold;
    }

    public bool AbleToWithdraw(int amount)
    {
        return (gold - amount) >= 0;
    }


}
