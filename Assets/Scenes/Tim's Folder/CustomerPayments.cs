using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerPayments : MonoBehaviour
{

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

    public void StandardPayment()
    {
        CollectPayment(15);
    }
}

