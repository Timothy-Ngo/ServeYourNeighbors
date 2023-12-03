using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ScriptableObjects;
using UnityEngine.UIElements;

/// <summary>
/// When enabling and disabling this system utilize the isEnabled variable
/// </summary>
public class PlacementSystem : MonoBehaviour
{
    
    [Tooltip("Prefabs of objects that can be dynamically placed in the scene")]
    [SerializeField] List<GameObject> prefabs;
    public GameEvent dragEvent;
    public GameObject tablesParent;
    public GameObject cookStationsParent;
    public GameObject distractionParent;
    GameObject selectedItem;
    [SerializeField] private Color originalFloorColor;
    [SerializeField] private Color placementFloorColor;

    public GameObject floorsParent;
    List<SpriteRenderer> floors;
    
    private Vector3 newPosition;
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


    // Start is called before the first frame update
    void Start()
    {
        floors = floorsParent.GetComponentsInChildren<SpriteRenderer>().ToList();
        newPosition = new Vector3();
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
                
                /*
                selectedItem.transform.position = Vector3.Lerp(
                    selectedItem.transform.position,
                    worldMousePosition + new Vector3(0, 0, 10),
                    interpolationFramesCount
                );
                */
                newPosition = new Vector3(Mathf.RoundToInt((worldMousePosition + new Vector3(0, 0, 10)).x) - 0.5f,
                    Mathf.RoundToInt((worldMousePosition + new Vector3(0, 0, 10)).y) - 0.5f, 0f );
                if (FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
                {
                    selectedItem.transform.position = newPosition;
                }
            }
            if (Input.GetKey(KeyCode.Space))
            {
                isEnabled = false;
                if (!Upgrades.inst.upgradesScreen.activeSelf)
                {
                    ChangeFloorColorTo(originalFloorColor);
                    Upgrades.inst.upgradesScreen.SetActive(true);
                }
                Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                selectedItem.transform.position = newPosition;
                if (Upgrades.inst.tablePlacementMode)
                {
                    Upgrades.inst.tablePlacementMode = false;
                    selectedItem.GetComponent<Table>().obstacleScript.PlaceObstacle();
                }

                if (Upgrades.inst.cookStationPlacementMode)
                {
                    Upgrades.inst.cookStationPlacementMode = false;
                    selectedItem.GetComponent<Obstacle>().PlaceObstacle();
                }

                if (Upgrades.inst.animatronicPlacementMode)
                {
                    Upgrades.inst.animatronicPlacementMode = false;
                    selectedItem.GetComponent<Obstacle>().PlaceObstacle();
                }
            }
        }
        
    }
    

    private void Enabled(bool enable)
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
        if (Upgrades.inst.upgradesScreen.activeSelf)
        {
            Upgrades.inst.upgradesScreen.SetActive(false);
            
        }
        if (enable)
        {
            ChangeFloorColorTo(placementFloorColor);
            if (Upgrades.inst.tablePlacementMode)
            {
                selectedItem = Instantiate(prefabs[0], worldMousePosition, Quaternion.identity);
                selectedItem.transform.parent = tablesParent.transform;
            }
            else if (Upgrades.inst.cookStationPlacementMode)
            {
                selectedItem = Instantiate(prefabs[1], worldMousePosition, Quaternion.identity);
                selectedItem.transform.parent = cookStationsParent.transform;
            }
            else if (Upgrades.inst.animatronicPlacementMode)
            {
                selectedItem = Instantiate(prefabs[2], worldMousePosition, Quaternion.identity);
                selectedItem.transform.parent = distractionParent.transform;
            }
            
        }
        else
        {
            //Destroy(selectedItem);            
        }


    }


    void ChangeFloorColorTo(Color color)
    {
        foreach (SpriteRenderer rend in floors)
        {
            rend.color = color;
        }
    }
    
    
    
    
}
