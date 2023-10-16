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
    

    void Start()
    {
        
    }

    void Update()
    {
        // turns on distraction
        if (Input.GetKeyDown(KeyCode.U))
        {
            animatronicDistraction.distractionTrigger.enabled = true;

            // ON text
            animatronicDistraction.statusText.enabled = true;
            animatronicDistraction.statusText.text = "ON";
        }
    }

    

}
