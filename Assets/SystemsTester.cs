using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemsTester : MonoBehaviour
{

    [Header("Upgrade System")]
    public GameObject upgradeScreen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) // Pulls up the menu for Upgrade System Testing
        {
            upgradeScreen.SetActive(!upgradeScreen.activeSelf);
        }       
    }
}
