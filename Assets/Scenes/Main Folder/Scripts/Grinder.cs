using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grinder : MonoBehaviour
{
    public Timer timerScript;
    public int grindTime = 2;
    [SerializeField] SpriteRenderer msgSR;
    Sprite msg;
    bool msgGrindedUp = false;
    bool grinding = false;

    private void Start()
    {
        msg = msgSR.sprite;
        msgSR.enabled = false;
    }

    private void Update()
    {
        if (timerScript.timeLeft <= 0 && grinding)
        {
            FinishGrinding();
        }
    }
    public void StartGrinding()
    {
        grinding = true;
        timerScript.SetMaxTime(grindTime);
    }

    public void FinishGrinding()
    {
        msgSR.enabled = true;
        grinding = false;
        msgGrindedUp = true;
    }

    public void TakeMSG()
    {
        if (!PickupSystem.inst.isHoldingSomething())
        {
            msgSR.enabled = false;
            PickupSystem.inst.PickUpItem(msg);
            msgGrindedUp = false;
        }
        
    }

    public bool IsGrindingDone()
    {
        return msgGrindedUp;
    }
}
