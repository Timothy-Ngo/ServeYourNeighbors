using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;

/// <summary>
/// When enabling and disabling this system utilize the isEnabled variable
/// </summary>
public class PlacementSystem : MonoBehaviour
{
    [Tooltip("Prefabs of objects that can be dynamically placed in the scene")]
    [SerializeField] List<GameObject> prefabs;
    public GameEvent dragEvent;
    public GameObject tablesParent;
    GameObject selectedItem;
    
    /// <summary>
    /// Number of frames to completely interpolate between item position and mouse position
    /// </summary>
    [SerializeField] private int interpolationFramesCount = 45;

    bool _isDragging;
    public bool isDragging
    {
        get => _isDragging;
        set
        {
            _isDragging = value;
        }
    }
     
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
        isDragging = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isEnabled = !isEnabled;
        }

        if ( isEnabled )
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
            if (isDragging)
            {
                Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                selectedItem.transform.position = Vector3.Lerp(
                    selectedItem.transform.position,
                    worldMousePosition + new Vector3(0, 0, 10),
                    interpolationFramesCount
                );
            }
            if (Input.GetKey(KeyCode.Space))
            {
                isEnabled = false;
            }
        }
        
    }


    private void Enabled(bool enabled)
    {
        /*
         * - remove player from scene
         * - show faded colored background
         * - show grid system, maybe I'll do this later
         *  
         * 
         */
        //uiGameObject.SetActive(enabled);
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0,0,10);
        Upgrades.inst.ToggleUpgradesScreen();
        if (enabled)
        {
            selectedItem = Instantiate(prefabs[0], worldMousePosition, Quaternion.identity);
            selectedItem.transform.parent = tablesParent.transform;
            
        }
        else
        {
            //Destroy(selectedItem);            
        }


    }
    
    
    
    
    
}
