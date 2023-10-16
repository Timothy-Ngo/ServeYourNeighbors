using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Distraction : MonoBehaviour
{
    public Collider2D distractionTrigger; // distraction's collider --> isTrigger is on
    SpriteRenderer distractionSR;
    public TextMeshProUGUI statusText;

    private void Start()
    {
        distractionTrigger.enabled = false;
        
        distractionSR = distractionTrigger.GetComponent<SpriteRenderer>();
        distractionSR.enabled = false;

        statusText.enabled = false;
    }

    private void Update()
    {
        if (statusText.text == "ON")
        {
            statusText.enabled = true;
        }
    }

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
