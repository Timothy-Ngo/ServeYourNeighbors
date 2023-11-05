using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerPayments : MonoBehaviour
{
    public static CustomerPayments inst;

    private void Awake()
    {
        inst = this;
    }
    
    [Header("Price Categories")]
    public int cheapPayment = 10;
    public int standardPayment = 20;
    public int expensivePayment = 30;

    [Header("Price Probabilities")]
    public float cheapProbability = 0.10f;
    public float standardProbability = 0.85f;
    public float expensiveProbability = 0.05f;

    [Header("MSG")] public int msgBonusMultiplier = 100;
    
    [Header("Sound FX")]
    public AudioClip collectPaymentSfx;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CollectPayment(int amount)
    {
        Currency.inst.Deposit(amount);
        SoundFX.inst.PlaySoundFXClip(collectPaymentSfx, transform, 1f);
    }

    public void RandomPayment() // This is actually dumb I don't like it, will change in the future
    {
        float probability = Random.value;
        Debug.Log($"Random value: {probability}");
        if (0 <= probability && probability  <= cheapProbability)
        {
            CollectPayment(cheapPayment);
        }
        else if (cheapProbability < probability && probability < (cheapProbability + standardProbability))
        {
            CollectPayment(standardPayment);
        }
        else if ((cheapProbability + standardProbability) <= probability && probability <= (cheapProbability + standardProbability + expensiveProbability))
        {
            CollectPayment(expensivePayment);
        }
    }

    public void TimeBasedPayment(float tipPercentage)
    {
        // Tip percentage is used to calculate how much of a 50% tip should be paid by the customer
        float payment = (float)(standardPayment + ((standardPayment*0.5) * tipPercentage));
        CollectPayment(Mathf.RoundToInt(payment));
        Debug.Log($"payment (before rounding): {payment}");

    }

    public void MSGPayment()
    {
        int payment = standardPayment * msgBonusMultiplier;
        CollectPayment(payment);
    }

}

