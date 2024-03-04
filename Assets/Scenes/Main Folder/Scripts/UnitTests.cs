using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTests : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            testTimeBasedPayment();
        }

    }

    public void testTimeBasedPayment() // Using varying tipPercentages to compare and see if the player gold is correct
    {
        // Assign, Act
        float tipPercentage = 0.1f;
        int gold = Currency.inst.gold;
        for (int i = 0; i <= 10; i++)
        {
            CustomerPayments.inst.TimeBasedPayment(tipPercentage);
            float timeBasedTip = (float)(CustomerPayments.inst.standardPayment * 0.5) * tipPercentage;
            float payment = (float)(CustomerPayments.inst.standardPayment + timeBasedTip);
            // Assert
            gold += Mathf.RoundToInt(payment);
            tipPercentage += 0.1f;
            if (Currency.inst.gold == gold)
            {
                Debug.Log($"Test #{i} passed");
            }
            else
            {
                Debug.Log($"Test #{i} failed");
            }

        }
    }


}
