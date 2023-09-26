using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour
{
    public static Currency inst;

    private void Awake()
    {
        inst = this;
    }
    
    public int gold;

    void Start()
    {
        Deposit(100);
    }


    /// <summary>
    /// Add money to gold
    /// </summary>
    public void Deposit(int amount)
    {
        Debug.Assert(amount > 0);
        gold += amount;
    }

    /// <summary>
    /// Take money from players gold
    /// </summary>
    public void Withdraw(int amount)
    {
        Debug.Assert(amount > 0);
        Debug.Assert(gold - amount >= 0);
        gold -= amount; 
    }

}
