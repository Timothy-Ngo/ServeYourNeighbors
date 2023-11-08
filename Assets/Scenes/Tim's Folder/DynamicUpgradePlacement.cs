using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When enabling and disabling this system utilize the isEnabled variable
/// </summary>
public class DynamicUpgradePlacement : MonoBehaviour
{

    private bool _isEnabled;
    /// <summary>
    /// Will enable and disable the system
    /// </summary>
    public bool isEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            Enabled(_isEnabled);
        }
    }

    private GameObject selectedItem;

    public GameObject uiGameObject;
    // Start is called before the first frame update
    void Start()
    {
        isEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isEnabled = !isEnabled;
        }

        if (isEnabled)
        {
            
        }
        
    }


    public void Enabled(bool enabled)
    {
        /*
         * - remove player from scene
         * - show faded colored background
         * - show grid system, maybe I'll do this later
         *  
         * 
         */
        uiGameObject.SetActive(enabled);
        
        
        
        


    }
}
