// Author: Helen Truong
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Distraction : MonoBehaviour
{
    public Collider2D distractionTrigger; // distraction's collider --> isTrigger is on
    public SpriteRenderer distractionSR;
    public TextMeshProUGUI statusText;
    [SerializeField] TextMeshProUGUI chargesText;

    public Timer timerScript;
    public Obstacle obstacleScript; 
    [SerializeField] int maxCharges = 2;
    int currentCharges;

    private void Start()
    {
        distractionTrigger.enabled = false;
        
        distractionSR.enabled = false;

        statusText.enabled = false;

        currentCharges = maxCharges;
        UpdateChargesText();
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

    // --------------- CHARGES --------------- //

    // Updates the text saying how many charges are left
    public void UpdateChargesText()
    {
        chargesText.text = currentCharges.ToString();
    }

    // Checks if there are any charges left to use
    public bool ChargesAvailable()
    {
        if (currentCharges > 0)
        {
            return true;
        }
        return false;
    }

    // Decreases the charges
    public void DecrementCharges()
    {
        currentCharges--;
        UpdateChargesText();
    }

    // Resets distractions charges back to max amount
    public void ResetCharges()
    {
        currentCharges = maxCharges;
        chargesText.text = currentCharges.ToString();
    }

    public void Animate()
    {
        gameObject.GetComponent<Animator>().enabled = true;
    }

    public void StopAnimate()
    {
        gameObject.GetComponent<Animator>().enabled = false;
    }
}
