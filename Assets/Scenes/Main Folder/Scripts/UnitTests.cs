using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTests : MonoBehaviour
{
    [SerializeField] SYNMeter synMeter;

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
        if (Input.GetKeyDown(KeyCode.Y))
        {
            testFullSYNMeter();
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
                Debug.Log($"Time Based Payment Test #{i} passed");
            }
            else
            {
                Debug.Log($"Time Based Payment Test #{i} failed");
            }

        }
    }

    public void testFullSYNMeter()
    {
        // SYN Meter is not full -- end game screen should not be active
            // will check SYN Meter when filled in increments of 10%

        // set SYN Meter to empty
        synMeter.AdjustSYN(-1); 
        for (int i = 0; !synMeter.IsFull(); i++)
        {
            if (!GameLoop.inst.IsEndScreenActive())
            {
                Debug.Log("Full SYN Meter: Test #" + (i+1) + " passed");
            }
            else
            {
                Debug.Log("Full SYN Meter: Test #" + (i+1) + " failed");
            }

            // fill SYN Meter by 10%
            synMeter.AdjustSYN(.1f);
        }

        // SYN Meter is full -- end game screen should be active

        // set SYN Meter to full
        synMeter.AdjustSYN(1);
        if (GameLoop.inst.IsEndScreenActive())
        {
            Debug.Log("Full SYN Meter: Test #11 passed");
        }
        else
        {
            Debug.Log("Full SYN Meter: Test #11 failed");
        }

    }
}
