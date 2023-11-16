/*

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GrinderStack : MonoBehaviour
{
    public Timer timerScript;
    public int grindTime = 2;
    [SerializeField] SpriteRenderer msgSR;
    Sprite msg;
    bool msgGrindedUp = false;
    bool grinding = false;

    // stacked msg and foodies
    public int msgCount = 0;
    public int grindingCount = 0;
    public TextMeshProUGUI grindingNumText;
    public TextMeshProUGUI msgNumText;

    private void Start()
    {
        msg = msgSR.sprite;
        msgSR.enabled = false;
        grindingNumText.text = "0";
        msgNumText.text = "0";
    }

    private void Update()
    {
        if (timerScript.timeLeft <= 0 && grinding)
        {
            FinishGrinding();
            
            
        }

        if (grindingCount > 0 && !grinding)
        {
            
            StartGrinding();
        }
    }
    public void StartGrinding()
    {
        grindingCount--;
        grindingNumText.text = grindingCount.ToString();
        grinding = true;
        timerScript.SetMaxTime(grindTime);
    }

    public void FinishGrinding()
    {
        msgSR.enabled = true;
        grinding = false;
        msgGrindedUp = true;
        msgCount++;
        msgNumText.text = msgCount.ToString();
    }

    public void TakeMSG()
    {
        if (!PickupSystem.inst.isHoldingSomething())
        {
            PickupSystem.inst.PickUpItem(msg);
            msgGrindedUp = false;
            msgCount--;
            msgNumText.text = msgCount.ToString();
            if (msgCount == 0)
                msgSR.enabled = false;
        }
        
    }

    public bool IsGrindingDone()
    {
        return msgGrindedUp;
    }
}

*/