using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net;
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
    public GameObject instructions;
    private Vector3 newPosition;
    
    
    //Boundaries
    public GameObject topLeftCornerObj;
    public GameObject bottomRightCornerObj;
    public Vector2 topLeftCorner;
    public Vector2 bottomRightCorner;
    /// <summary>
    /// Number of frames to completely interpolate between item position and mouse position
    /// </summary>
    //[SerializeField] private int interpolationFramesCount = 45;

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
        instructions.SetActive(false);

        topLeftCorner = topLeftCornerObj.transform.position;
        bottomRightCorner = bottomRightCornerObj.transform.position;
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
            if (Upgrades.inst.moveItemPlacementMode)
            {
                if (Input.GetMouseButton(1)) // Chooses an item to start moving
                {
                    // Utilize point and click to recognize what placement mode to use
                    Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Debug.Log($"worldPosition: {worldPosition}");
                    Collider2D collider = Physics2D.OverlapPoint(worldPosition);
                    Debug.Log($"Collider2D: {collider.gameObject.name}");
                    //  Use tags to recognize which one to use
                    if (collider.gameObject.CompareTag("Table"))
                    {
                        Upgrades.inst.tablePlacementMode = true; 
                        Destroy(collider.transform.parent.gameObject);
                    }
                    else if (collider.gameObject.CompareTag("Counter"))
                    {
                        Upgrades.inst.counterPlacementMode = true;
                        Destroy(collider.transform.gameObject);
                    }
                    else if (collider.gameObject.CompareTag("Cooktop"))
                    {
                        Upgrades.inst.cookStationPlacementMode = true;
                        Destroy(collider.transform.gameObject);
                    }
                    Upgrades.inst.placementSystem.isEnabled = true;
                    // Need to include Grinder, boxes, and distraction
                    Upgrades.inst.moveItemPlacementMode = false;
                }

                return;
            }
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
                if (topLeftCorner.x < worldMousePosition.x && worldMousePosition.x < bottomRightCorner.x && bottomRightCorner.y < worldMousePosition.y && worldMousePosition.y < topLeftCorner.y)
                {
                    newPosition = new Vector3(Mathf.RoundToInt((worldMousePosition + new Vector3(0, 0, 10)).x) - 0.5f,
                        Mathf.RoundToInt((worldMousePosition + new Vector3(0, 0, 10)).y) - 0.5f, 0f );
                    if (FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
                    {
                        selectedItem.transform.position = newPosition;
                    }
                    
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
                //Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //selectedItem.transform.position = newPosition;
                if (Upgrades.inst.tablePlacementMode)
                {
                    FoodieSystem.inst.GetCurrentSeats();
                    Upgrades.inst.tablePlacementMode = false;
                    selectedItem.GetComponent<Table>().obstacleScript.PlaceObstacle();
                }

                if (Upgrades.inst.cookStationPlacementMode)
                {
                    Upgrades.inst.cookStationPlacementMode = false;
                    selectedItem.GetComponent<Obstacle>().PlaceObstacle();
                }

                if (Upgrades.inst.counterPlacementMode)
                {
                    Upgrades.inst.counterPlacementMode = false;
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
            else if (Upgrades.inst.counterPlacementMode)
            {
                selectedItem = Instantiate(prefabs[2], worldMousePosition, Quaternion.identity);
                selectedItem.transform.parent = Upgrades.inst.counterParent.transform;
            }
            else if (Upgrades.inst.animatronicPlacementMode)
            {
                selectedItem = Instantiate(prefabs[3], worldMousePosition, Quaternion.identity);
                selectedItem.transform.parent = distractionParent.transform;
                DistractionSystem.inst.animatronicDistraction = selectedItem.GetComponent<Distraction>();
            }

            instructions.SetActive(true);
        }
        else
        {
            instructions.SetActive(false);
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
