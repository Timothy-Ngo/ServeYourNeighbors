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
    }

    public void StartDistraction()
    {
        animatronicDistraction.distractionTrigger.enabled = true;

        // ON text
        animatronicDistraction.statusText.enabled = true;
        animatronicDistraction.statusText.text = "ON";
    }

}
