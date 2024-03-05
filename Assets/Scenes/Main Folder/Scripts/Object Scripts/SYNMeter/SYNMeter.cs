// Kirin Hardinger
// October 2023

// Modified by Helen Truong 
// February 2024

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SYNMeter : MonoBehaviour, IDataPersistence
{
    public bool full = false;
    [SerializeField] Image bar; // drag and drop image in inspector
    float decayValue = 0.05f;
    public float kidnappingSYNValue = .1f;
    public float fillAmount = 0f;

    public void LoadData(GameData data)
    {
        fillAmount = data.fillAmount;
        bar.fillAmount = fillAmount;
    }

    public void SaveData(GameData data)
    {
        fillAmount = bar.fillAmount;
        data.fillAmount = fillAmount;
    }

    void Start()
    {
        //bar.fillAmount = 0; // start with 0 SYN
    }

    // for setting a specific, constant value
    public void SetSYN(float val)
    {
        if(val > 1 || val < 0)
        {
            fillAmount = 1;
            Debug.Log("Value is higher than max SYN -- value auto set to max SYN");
        }
        else if (val < 0)
        {
            fillAmount = 0;
            Debug.Log("Value is lower than min SYN -- value auto set to min SYN");
        }
        else
        {
            fillAmount = val;
        }

        bar.fillAmount = fillAmount;
    }

    // for adjusting + or - by a set amount -- input a float between -1 and 1 -- think of input as a percentage of the bar
    public void AdjustSYN(float val)
    {

        if(fillAmount + val >= 1)
        {
            fillAmount = 1;
            full = true;
            GameLoop.inst.GameOver();
        } 
        else if (fillAmount + val < 0)
        {
            fillAmount = 0;
            full = false;
        }
        else
        {
            fillAmount += val;
            full = false;
        }

        bar.fillAmount = fillAmount;
    }


    // decays SYN value when day ends
    public void DecaySYN()
    {
        if (fillAmount - decayValue < 0)
        {
            fillAmount = 0;
        }
        else
        {
            fillAmount -= decayValue;
        }

        bar.fillAmount = fillAmount;
    }

    // for returning SYN value
    public float GetSYN()
    {
        Debug.Log("SYN is at " + fillAmount.ToString());
        return fillAmount;
    }

    public bool IsFull()
    {
        return full;
    }
}
