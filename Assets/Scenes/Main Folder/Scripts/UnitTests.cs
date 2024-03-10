using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTests : MonoBehaviour
{
    [SerializeField] bool allowUnitTests = false;
    [SerializeField] SYNMeter synMeter;
    [SerializeField] Cooking cooking;
    [SerializeField] QTEvent qtEvent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (allowUnitTests)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                testTimeBasedPayment();
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                testFullSYNMeter();
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(testCookingKeysmash());
            }
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

    public IEnumerator testCookingKeysmash()
    {
        // keysmash bar not filled = cooking should not begin
        // once the keysmash bar is filled, cooking should begin

        // simulate user beginning prep process (which activates keysmash event)
        cooking.StartPrep();

        // simulate user increasing fill amount of keysmash event incrementally
        for (int i = 0; !qtEvent.isComplete(); i++)
        {
            if (!cooking.IsCooking())
            {
                Debug.Log("Keysmash Event: Test #" + (i + 1) + " passed");
            }
            else
            {
                Debug.Log("Keysmash Event: Test #" + (i + 1) + " failed");
            }

            // fill keysmash event bar by 10%
            qtEvent.AdjustFillAmount(.1f);
            yield return new WaitForFixedUpdate();  // wait for just a frame so that attributes can be updated before the check
        }

        // set keysmash event bar to full
        qtEvent.resetEvent();
        qtEvent.AdjustFillAmount(1);
        yield return new WaitForFixedUpdate(); // wait for just a frame so that attributes can be updated before the check
        if (cooking.IsCooking())
        {
            Debug.Log("Full Keysmash Event: Test #11 passed");
        }
        else
        {
            Debug.Log("Full Keysmash Event: Test #11 failed");
        }
    }
}
