// Kirin Hardinger
// October 2023

// Modified by Helen Truong 
// February 2024

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SYNMeter : MonoBehaviour {
    public bool full = false;
    [SerializeField] Image bar; // drag and drop image in inspector
    float decayValue = 0.05f;
    public float kidnappingSYNValue = .1f;

    void Start()
    {
        bar.fillAmount = 0; // start with 0 SYN
    }

    // for setting a specific, constant value
    public void SetSYN(float val)
    {
        if(val > 1 || val < 0)
        {
            bar.fillAmount = 1;
            Debug.Log("Value is higher than max SYN -- value auto set to max SYN");
        }
        else if (val < 0)
        {
            bar.fillAmount = 0;
            Debug.Log("Value is lower than min SYN -- value auto set to min SYN");
        }
        else
        {
            bar.fillAmount = val;
        }
    }

    // for adjusting + or - by a set amount -- input a float between 0 and 1 -- think of input as a percentage of the bar
    public void AdjustSYN(float val)
    {

        if(bar.fillAmount + val > 1)
        {
            bar.fillAmount = 1;
            full = true;
        } 
        else if (bar.fillAmount + val < 0)
        {
            bar.fillAmount = 0;
            full = false;
        }
        else
        {
            bar.fillAmount += val;
            full = false;
        }
    }


    // decays SYN value when day ends
    public void DecaySYN()
    {
        if (bar.fillAmount - decayValue < 0)
        {
            bar.fillAmount = 0;
        }
        else
        {
            bar.fillAmount -= decayValue;
        }
    }

    // for returning SYN value
    public float GetSYN()
    {
        Debug.Log("SYN is at " + bar.fillAmount.ToString());
        return bar.fillAmount;
    }

    public bool IsFull()
    {
        return full;
    }
}
