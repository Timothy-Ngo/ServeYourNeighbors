using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DistractionCharges : MonoBehaviour
{
    public Collider2D distractionTrigger; // distraction's collider --> isTrigger is on
    SpriteRenderer distractionSR;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI chargesText;

    public int maxCharges = 2;
    int currentCharges;
    private void Start()
    {
        distractionTrigger.enabled = false;
        
        distractionSR = distractionTrigger.GetComponent<SpriteRenderer>();
        distractionSR.enabled = false;

        statusText.enabled = false;
        currentCharges = maxCharges;
        UpdateChargesText();
    }

    public void UpdateChargesText()
    {
        chargesText.text = currentCharges.ToString();
    }

    public bool ChargesAvailable()
    {
        if (currentCharges > 0)
        {
            return true;
        }
        return false;
    }

    public void DecrementCharges()
    {
        currentCharges--;
        UpdateChargesText();
    }

    public void ResetCharges()
    {
        currentCharges = maxCharges;
    }

    private void Update()
    {
        // ON status always shows even if not hovering over
        if (statusText.text == "ON")
        {
            statusText.enabled = true;
        }
    }

    // hover over distraction to see affect range + status (on or off)
    private void OnMouseOver()
    {
        distractionSR.enabled = true;
        statusText.enabled = true;
    }

    private void OnMouseExit()
    {
        if (statusText.text != "ON")
            statusText.enabled = false;
        distractionSR.enabled = false;
    }
}
