using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DistractionCooldown : MonoBehaviour
{
    public Collider2D distractionTrigger; // distraction's collider --> isTrigger is on
    SpriteRenderer distractionSR;
    public TextMeshProUGUI statusText;


    public Timer cooldownTimer;
    public TextMeshProUGUI cooldownText;
    public int cooldownTime = 2;
    public bool onCooldown = false;

    private void Start()
    {
        distractionTrigger.enabled = false;
        
        distractionSR = distractionTrigger.GetComponent<SpriteRenderer>();
        distractionSR.enabled = false;

        statusText.enabled = false;

        cooldownText.text = "Cooldown";
        cooldownText.enabled = false;
    }

    private void Update()
    {
        // ON status always shows even if not hovering over
        if (statusText.text == "ON")
        {
            statusText.enabled = true;
        }
    }

    public void StartCooldown()
    {
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        cooldownTimer.SetMaxTime(cooldownTime);
        onCooldown = true;
        cooldownText.enabled = true;

        yield return new WaitForSeconds(cooldownTime);

        onCooldown = false;
        cooldownText.enabled = false;

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
