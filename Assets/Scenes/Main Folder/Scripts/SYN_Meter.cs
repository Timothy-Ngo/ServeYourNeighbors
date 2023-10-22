// Kirin Hardinger
// October 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SYN_Meter : MonoBehaviour {
    public bool full = false;
    Image bar;

    void Start() {
        bar = GetComponent<Image>();
        bar.fillAmount = 0; // start with 0 SYN
    }

    private void Update() {
        if(bar.fillAmount == 1) {
            full = true;
        } else {
            full = false;
        }
    }

    // for setting a specific, constant value
    public void setSYN(float val) {
        if(val > 1 || val < 0) {
            Debug.Log("Invalid SYN value!");
        } else {
            bar.fillAmount = val;
        }
    }

    // for adjusting + or - by a set amount
    public void adjustSYN(float val) {
        if(bar.fillAmount + val > 1 || bar.fillAmount + val < 0) {
            Debug.Log("Invalid SYN value!");
        } else {
            bar.fillAmount += val;
        }
    }

    // for returning SYN value
    public float getSYN() {
        Debug.Log("SYN is at " + bar.fillAmount.ToString());
        return bar.fillAmount;
    }

    public bool isFull() {
        return full;
    }
}
