using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CustomerPaymentTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void testTimeBasedPayment()
    {
        // Assign, Act
        float tipPercentage = 0.1f;
        var currency = new Currency();
        int gold = currency.startingGold;
        for (int i = 0; i <= 10; i++ )
        {
            CustomerPayments.inst.TimeBasedPayment(tipPercentage);
            float timeBasedTip = (float)(CustomerPayments.inst.standardPayment * 0.5) * tipPercentage;
            float payment = (float)(CustomerPayments.inst.standardPayment + timeBasedTip);
            // Assert
            gold += Mathf.RoundToInt(payment);
            Assert.AreEqual(currency.gold, gold);
            tipPercentage += 0.1f;   
        }
        
    }
}
