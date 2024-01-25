// Author: Helen Truong
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DistractionSystem : MonoBehaviour
{
    public static DistractionSystem inst;

    private void Awake()
    {
        inst = this;
    }


    public Distraction animatronicDistraction;
    public bool showDebug = false;
    public int distractedTime = 2;
    

    void Start()
    {
        
    }

    void Update()
    {
        if (showDebug)
        {
            // turns on distraction
            if (Input.GetKeyDown(KeyCode.T))
            {
                StartDistraction();
            }
        }

        if (animatronicDistraction != null && animatronicDistraction.timerScript.timeLeft <= 0 )
        {
            ResetDistraction();
        }
    }

    public void StartDistraction()
    {
        animatronicDistraction.distractionTrigger.enabled = true;

        // ON text
        animatronicDistraction.statusText.enabled = true;
        animatronicDistraction.statusText.text = "ON";

        animatronicDistraction.timerScript.SetMaxTime(distractedTime);
    }

    public void ResetDistraction()
    {
        animatronicDistraction.distractionTrigger.enabled = false;
        animatronicDistraction.statusText.text = "OFF";
        animatronicDistraction.statusText.enabled = false;
    }
}
