// Author: Timothy Ngo
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CustomerPayments : MonoBehaviour
{
    [SerializeField] SYNMeter synMeter;
    public static CustomerPayments inst;

    private void Awake()
    {
        inst = this;
    }

    [Header("Price Categories")]

    public int standardPayment = 15;


    [Header("MSG")]
    public float msgBonusMultiplier = 3f;

    [Header("Sound FX")]
    public AudioClip collectPaymentSfx;

    [SerializeField] bool debugMode = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (debugMode)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CollectPayment(1000);
            }
        }
    }

    private void CollectPayment(int amount)
    {
        Currency.inst.Deposit(amount);
        SoundFX.inst.CollectPaymentSFX(1f);
    }

    public void TimeBasedPayment(float tipPercentage)
    {
        // Tip percentage is used to calculate how much of a 50% tip should be paid by the customer
        float timeBasedTip = (float)(standardPayment * 0.5) * tipPercentage;
        float payment = (float)(standardPayment + timeBasedTip);
        CollectPayment(Mathf.RoundToInt(payment));
    }

    public void MSGPayment()
    {
        float payment = standardPayment * (msgBonusMultiplier * (1 - synMeter.GetSYN()));
        CollectPayment(Mathf.RoundToInt(payment));
    }

    public void SetStandardPayment(int newPayment) {
        standardPayment = newPayment;
    }
}

