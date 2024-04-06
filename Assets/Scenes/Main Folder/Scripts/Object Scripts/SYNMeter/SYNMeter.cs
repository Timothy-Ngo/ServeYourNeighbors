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

    
    [Header("-----SYN SCENE-----")]
    [SerializeField] private GameObject layout1Floors;
    [SerializeField] private GameObject layout2Floors;
    [SerializeField] private GameObject layout3Floors;
    [SerializeField] private Sprite floors;
    [SerializeField] private List<Sprite> floors30;
    [SerializeField] private List<Sprite> floors70;
    [SerializeField] private GameObject shackWalls;
    [SerializeField] private Sprite shack;
    [SerializeField] private Sprite shack30;
    [SerializeField] private Sprite shack70;
    [SerializeField] private GameObject tavernWalls;
    [SerializeField] private Sprite tavern;
    [SerializeField] private Sprite tavern30;
    [SerializeField] private Sprite tavern70;
    [SerializeField] private GameObject restaurantWalls;
    [SerializeField] private Sprite restaurant;
    [SerializeField] private Sprite restaurant30;
    [SerializeField] private Sprite restaurant70;
    [SerializeField] private Upgrades upgradesSystem;
    [SerializeField] private GameObject synOverlay30;
    [SerializeField] private GameObject synOverlay70;
    [SerializeField] private int synState = 0;

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
        if(synState == 2 && fillAmount + val >= 0.7f)
        {
            synState = 3;

            synOverlay30.SetActive(false);
            synOverlay70.SetActive(true);
            foreach (Transform child in layout1Floors.transform)
            {
                child.GetComponent<SpriteRenderer>().sprite = floors70[Random.Range(0, 2)];
            }
            foreach (Transform child in layout2Floors.transform)
            {
                child.GetComponent<SpriteRenderer>().sprite = floors70[Random.Range(0, 2)];
            }
            foreach (Transform child in layout3Floors.transform)
            {
                child.GetComponent<SpriteRenderer>().sprite = floors70[Random.Range(0, 2)];
            }

            switch (upgradesSystem.currentLayout)
            {
                case Upgrades.LayoutLevel.Shack:
                    shackWalls.GetComponent<SpriteRenderer>().sprite = shack70;
                    break;
                case Upgrades.LayoutLevel.Tavern:
                    Debug.Log("changing tavern to 70");
                    tavernWalls.GetComponent<SpriteRenderer>().sprite = tavern70;
                    break;
                case Upgrades.LayoutLevel.Restaurant:
                    Debug.Log("changing restaurant to 70");
                    restaurantWalls.GetComponent<SpriteRenderer>().sprite = restaurant70;
                    break;
            }
        }
        else if ((synState == 3 && fillAmount + val < 0.7f && fillAmount + val >= 0.3f) || (synState == 1 && fillAmount + val >= 0.3f))
        {
            synState = 2;

            synOverlay30.SetActive(true);
            synOverlay70.SetActive(false);
            foreach (Transform child in layout1Floors.transform)
            {
                child.GetComponent<SpriteRenderer>().sprite = floors30[Random.Range(0, 2)];
            }
            foreach (Transform child in layout2Floors.transform)
            {
                child.GetComponent<SpriteRenderer>().sprite = floors30[Random.Range(0, 2)];
            }
            foreach (Transform child in layout3Floors.transform)
            {
                child.GetComponent<SpriteRenderer>().sprite = floors30[Random.Range(0, 2)];
            }

            switch (upgradesSystem.currentLayout)
            {
                case Upgrades.LayoutLevel.Shack:
                    shackWalls.GetComponent<SpriteRenderer>().sprite = shack30;
                    break;
                case Upgrades.LayoutLevel.Tavern:
                    Debug.Log("changing tavern to 30");
                    tavernWalls.GetComponent<SpriteRenderer>().sprite = tavern30;
                    break;
                case Upgrades.LayoutLevel.Restaurant:
                    Debug.Log("restaurant tavern to 30");
                    restaurantWalls.GetComponent<SpriteRenderer>().sprite = restaurant30;
                    break;
            }
        }
        else if (fillAmount + val < 0.3f)
        {
            synState = 1;

            synOverlay30.SetActive(false);
            synOverlay70.SetActive(false);

            foreach (Transform child in layout1Floors.transform)
            {
                child.GetComponent<SpriteRenderer>().sprite = floors;
            }
            foreach (Transform child in layout2Floors.transform)
            {
                child.GetComponent<SpriteRenderer>().sprite = floors;
            }
            foreach (Transform child in layout3Floors.transform)
            {
                child.GetComponent<SpriteRenderer>().sprite = floors;
            }

            switch (upgradesSystem.currentLayout)
            {
                case Upgrades.LayoutLevel.Shack:
                    shackWalls.GetComponent<SpriteRenderer>().sprite = shack;
                    break;
                case Upgrades.LayoutLevel.Tavern:
                    tavernWalls.GetComponent<SpriteRenderer>().sprite = tavern;
                    break;
                case Upgrades.LayoutLevel.Restaurant:
                    restaurantWalls.GetComponent<SpriteRenderer>().sprite = restaurant;
                    break;
            }
        }

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
