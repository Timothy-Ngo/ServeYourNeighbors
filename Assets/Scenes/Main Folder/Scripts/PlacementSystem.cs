using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;


/// <summary>
/// When enabling and disabling this system utilize the isEnabled variable
/// </summary>
public class PlacementSystem : MonoBehaviour
{

    [Tooltip("Prefabs of objects that can be dynamically placed in the scene")]
    [SerializeField] List<GameObject> prefabs;
    //public GameEvent dragEvent;
    public GameObject tablesParent;
    public GameObject cookStationsParent;
    public GameObject distractionParent;
    GameObject selectedItem;
    [SerializeField] private Color originalFloorColor;
    [SerializeField] private Color placementFloorColor;

    public GameObject floorsParent;
    List<SpriteRenderer> floors;
    public GameObject instructions;
    public TextMeshProUGUI instructionsTextMesh;
    private Vector3 newPosition;

    [SerializeField] GameObject startingPositionObject;
    [SerializeField] GameObject bottomUIObject;
    [SerializeField] GameObject topUIObject;


    //Boundaries
    [Header("Whole Map Boundary")]
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
        if (isEnabled)
        {

            if (Upgrades.inst.changeLayoutMode && false)
            {
                // Detect Item using overlap point
                // make item the selected item
                if (Input.GetMouseButton(1)) // Chooses an item to start moving
                {
                    // Utilize point and click to recognize what placement mode to use
                    Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Collider2D collider = Physics2D.OverlapPoint(worldPosition);
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
                    Upgrades.inst.changeLayoutMode = false;
                }

                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (Upgrades.inst.changeLayoutMode)
                {

                    Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Collider2D collider = Physics2D.OverlapPoint(worldPosition);
                    if (collider.gameObject.CompareTag("Table"))
                    {
                        selectedItem = collider.gameObject.transform.parent.gameObject;
                    }
                    else if (collider.gameObject.CompareTag("Cooktop") ||
                             collider.gameObject.CompareTag("Grinder") ||
                             collider.gameObject.CompareTag("TrashCan") ||
                             collider.gameObject.CompareTag("IngredientBox") ||
                             collider.gameObject.CompareTag("Distraction")
                             )
                    {
                        selectedItem = collider.gameObject.transform.gameObject;
                    }
                }
                isDragging = true;
                if (selectedItem.CompareTag("Table"))
                {
                    selectedItem.GetComponentInChildren<Obstacle>().RemoveObstacle();
                }
                else
                {
                    selectedItem.GetComponent<Obstacle>().RemoveObstacle();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                if (selectedItem.CompareTag("Table"))
                {
                    selectedItem.GetComponentInChildren<Obstacle>().PlaceObstacle();
                }
                else
                {
                    selectedItem.GetComponent<Obstacle>().PlaceObstacle();
                }

            }
            if (isDragging)
            {
                Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (topLeftCorner.x < worldMousePosition.x && worldMousePosition.x < bottomRightCorner.x && bottomRightCorner.y < worldMousePosition.y && worldMousePosition.y < topLeftCorner.y)
                {
                    newPosition = new Vector3(Mathf.RoundToInt((worldMousePosition + new Vector3(0, 0, 10)).x) - 0.5f,
                        Mathf.RoundToInt((worldMousePosition + new Vector3(0, 0, 10)).y) - 0.5f, 0f);
                    if (FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
                    {
                        if (selectedItem.CompareTag("Cooktop") ||
                            selectedItem.CompareTag("Grinder") ||
                            selectedItem.CompareTag("TrashCan") ||
                            selectedItem.CompareTag("IngredientBox")
                            )
                        {
                            // check if item is in kitchen
                            if (InKitchenBoundary(newPosition))
                            {
                                selectedItem.transform.position = newPosition;
                            }


                        }
                        else
                        {
                            if (!InKitchenBoundary(newPosition))
                            {
                                selectedItem.transform.position = newPosition;
                            }
                        }
                    }

                }

            }
            if (Input.GetKey(InputSystem.inst.finishPlacementKey))
            {

                if (selectedItem.transform.position == startingPositionObject.transform.position)
                {
                    Debug.Log("Item must be placed in restaurant.");
                }
                else
                {

                    isEnabled = false;
                    if (!Upgrades.inst.upgradesScreen.activeSelf)
                    {
                        ChangeFloorColorTo(originalFloorColor);
                        Upgrades.inst.upgradesScreen.SetActive(true);
                    }

                    if (Upgrades.inst.changeLayoutMode)
                    {
                        Upgrades.inst.changeLayoutMode = false;
                        FoodieSystem.inst.GetCurrentSeats();
                        return;
                    }
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
                        Upgrades.inst.cookStations.Add(selectedItem);
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
                        Upgrades.inst.animatronicPosition = selectedItem.transform.position;
                        Upgrades.inst.hasAnimatronic = true;
                        DistractionSystem.inst.animatronicDistraction = selectedItem.GetComponent<Distraction>();
                    }

                }

            }
        }

    }


    private void Enabled(bool enable)
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
        if (Upgrades.inst.upgradesScreen.activeSelf)
        {
            Upgrades.inst.upgradesScreen.SetActive(false);

        }
        if (enable)
        {
            ChangeFloorColorTo(placementFloorColor);
            instructions.SetActive(true);
            topUIObject.SetActive(true);
            bottomUIObject.SetActive(true);
            // Add if statement for change layout mode
            if (Upgrades.inst.changeLayoutMode)
            {
                return;
            }
            if (Upgrades.inst.tablePlacementMode)
            {
                selectedItem = Instantiate(prefabs[0], startingPositionObject.transform.position, Quaternion.identity);
                selectedItem.transform.parent = tablesParent.transform;
            }
            else if (Upgrades.inst.cookStationPlacementMode)
            {
                selectedItem = Instantiate(prefabs[1], startingPositionObject.transform.position, Quaternion.identity);
                selectedItem.transform.parent = cookStationsParent.transform;
            }
            else if (Upgrades.inst.counterPlacementMode)
            {
                selectedItem = Instantiate(prefabs[2], startingPositionObject.transform.position, Quaternion.identity);
                selectedItem.transform.parent = Upgrades.inst.counterParent.transform;
            }
            else if (Upgrades.inst.animatronicPlacementMode)
            {
                selectedItem = Instantiate(prefabs[3], startingPositionObject.transform.position, Quaternion.identity);
                selectedItem.transform.parent = distractionParent.transform;
                DistractionSystem.inst.animatronicDistraction = selectedItem.GetComponent<Distraction>();
            }

            instructionsTextMesh.text = "Hold left click to move object.\r\nPress " + InputSystem.inst.finishPlacementKey.ToString() + " to confirm placement";
        }
        else
        {
            instructions.SetActive(false);
            topUIObject.SetActive(false);
            bottomUIObject.SetActive(false);
            //Destroy(selectedItem);            
        }


    }

    bool InKitchenBoundary(Vector3 position)
    {
        Vector3 topLeft = Layout.inst.topLeftKitchenBoundaryPosition;
        Vector3 bottomRight = Layout.inst.bottomRightKitchenBoundaryPosition;
        bool withinXRange = topLeft.x <= position.x && position.x <= bottomRight.x;
        bool withinYRange = bottomRight.y <= position.y && position.y <= topLeft.y;
        Debug.Log(topLeft);
        Debug.Log(bottomRight);

        return withinXRange && withinYRange;
    }


    void ChangeFloorColorTo(Color color)
    {
        foreach (SpriteRenderer rend in floors)
        {
            rend.color = color;
        }
    }




}
