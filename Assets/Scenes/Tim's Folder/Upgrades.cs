using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    

    public void GetChair()
    {
        Currency.inst.Withdraw(250);
        Debug.Log("Bought a Chair"); // This is to be replaced with making the upgrade appear in the scene 
    }

}
