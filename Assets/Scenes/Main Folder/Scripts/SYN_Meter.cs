// Kirin Hardinger
// October 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SYN_Meter : MonoBehaviour {
    Image bar;

    void Start() {
        bar = GetComponent<Image>();
        bar.fillAmount = 0; // start with 0 SYN
    }

    // for setting a specific, constant value
    public void setSYN(int val) {
        bar.fillAmount = val;
    }

    // for adjusting + or - by a set amount
    public void adjustSYN(int val) {
        bar.fillAmount += val;
    }

    // for returning SYN value
    public float getSYN() {
        Debug.Log("SYN is at " + bar.fillAmount.ToString());
        return bar.fillAmount;
    }
}
