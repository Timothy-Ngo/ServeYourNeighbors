using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When enabling and disabling this system utilize the isEnabled variable
/// </summary>
public class DynamicUpgradePlacement : MonoBehaviour
{
    [Tooltip("Prefabs of objects that can be dynamically placed in the scene")]
    [SerializeField] List<GameObject> prefabs;
    GameObject selectedItem;

    /// <summary>
    /// Number of frames to completely interpolate between item position and mouse position
    /// </summary>
    [SerializeField] private int interpolationFramesCount = 45; 
    
    bool _isEnabled;
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
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedItem.transform.position = Vector3.Lerp(
                selectedItem.transform.position,
                worldMousePosition,
                interpolationFramesCount
            );
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
        //uiGameObject.SetActive(enabled);
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (enabled)
        {
            selectedItem = Instantiate(prefabs[0], worldMousePosition, Quaternion.identity);
        }
        else
        {
            Destroy(selectedItem);            
        }


    }
    
    
    
}
