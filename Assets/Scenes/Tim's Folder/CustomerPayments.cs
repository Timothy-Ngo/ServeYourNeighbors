using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerPayments : MonoBehaviour
{
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
    }

    public void StandardPayment()
    {
        CollectPayment(15);
    }
}

