using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;




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


    [SerializeField] Button tablesUpgradeButton;


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

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Vector3 newPosition = selectedItem.transform.position;
                bool foundNewPosition = false;
                if (!InMapBoundary(newPosition))
                {
                    newPosition = FirstAvailableSpace();
                    Debug.Assert(newPosition != Vector3.negativeInfinity, "Could not find first available space");
                    foundNewPosition = true;
                }
                else
                {
                    for (float x = selectedItem.transform.position.x + 1; x < bottomRightCorner.x; x++)
                    {
                        newPosition = new Vector3(x, newPosition.y, newPosition.z);
                        if (selectedItem.CompareTag("Table"))
                        {
                            if (!InKitchenBoundary(newPosition) &&
                                FoodieSystem.inst.pathfinding.IsPlaceable(newPosition) &&
                                FoodieSystem.inst.pathfinding.IsPlaceable(newPosition + Vector3.left) &&
                                FoodieSystem.inst.pathfinding.IsPlaceable(newPosition + (2 * Vector3.left)))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                        else if (selectedItem.CompareTag("Cooktop"))
                        {
                            if (InKitchenBoundary(newPosition) && FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                        else if (selectedItem.CompareTag("Distraction"))
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                for (int j = -1; j < 2; j++)
                                {
                                    if(!FoodieSystem.inst.pathfinding.IsPlaceable(new Vector3(newPosition.x + i, newPosition.y + j, newPosition.z)))
                                    {
                                        continue;
                                    }
                                }
                            }
                            foundNewPosition = true;
                            break;
                        }
                    }
                }
                if (foundNewPosition)
                {
                    selectedItem.transform.position = newPosition;
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Vector3 newPosition = selectedItem.transform.position;
                bool foundNewPosition = false;
                if (!InMapBoundary(newPosition))
                {
                    newPosition = FirstAvailableSpace();
                    Debug.Assert(newPosition != Vector3.negativeInfinity, "Could not find first available space");
                    foundNewPosition = true;
                }
                else
                {
                    for (float x = selectedItem.transform.position.x - 1; x > topLeftCorner.x; x--)
                    {
                        newPosition = new Vector3(x, newPosition.y, newPosition.z);
                        if (selectedItem.CompareTag("Table"))
                        {
                            if (FoodieSystem.inst.pathfinding.IsPlaceable(newPosition) &&
                                FoodieSystem.inst.pathfinding.IsPlaceable(newPosition + Vector3.left) &&
                                FoodieSystem.inst.pathfinding.IsPlaceable(newPosition + (2 * Vector3.left)))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                        else if (selectedItem.CompareTag("Cooktop"))
                        {
                            if (InKitchenBoundary(newPosition) && FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                        else if (selectedItem.CompareTag("Distraction"))
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                for (int j = -1; j < 2; j++)
                                {
                                    if(!FoodieSystem.inst.pathfinding.IsPlaceable(new Vector3(newPosition.x + i, newPosition.y + j, newPosition.z)))
                                    {
                                        continue;
                                    }
                                }
                            }
                            foundNewPosition = true;
                            break;
                        }
                    }
                }
                if (foundNewPosition)
                {
                    selectedItem.transform.position = newPosition;
                }
            }


            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Vector3 newPosition = selectedItem.transform.position;
                bool foundNewPosition = false;
                if (!InMapBoundary(newPosition))
                {
                    newPosition = FirstAvailableSpace();
                    Debug.Assert(newPosition != Vector3.negativeInfinity, "Could not find first available space");
                    foundNewPosition = true;
                }
                else
                {
                    for (float y = selectedItem.transform.position.y + 1; y < topLeftCorner.y; y++)
                    {
                        newPosition = new Vector3(newPosition.x, y, newPosition.z);
                        if (selectedItem.CompareTag("Table"))
                        {
                            if (FoodieSystem.inst.pathfinding.IsPlaceable(newPosition) &&
                                FoodieSystem.inst.pathfinding.IsPlaceable(newPosition + Vector3.left) &&
                                FoodieSystem.inst.pathfinding.IsPlaceable(newPosition + (2 * Vector3.left)))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                        else if (selectedItem.CompareTag("Cooktop"))
                        {
                            if (InKitchenBoundary(newPosition) && FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                        else if (selectedItem.CompareTag("Distraction"))
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                for (int j = -1; j < 2; j++)
                                {
                                    if(!FoodieSystem.inst.pathfinding.IsPlaceable(new Vector3(newPosition.x + i, newPosition.y + j, newPosition.z)))
                                    {
                                        continue;
                                    }
                                }
                            }
                            foundNewPosition = true;
                            break;
                        }
                    }
                }
                if (foundNewPosition)
                {
                    selectedItem.transform.position = newPosition;
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Vector3 newPosition = selectedItem.transform.position;
                bool foundNewPosition = false;
                if (!InMapBoundary(newPosition))
                {
                    newPosition = FirstAvailableSpace();
                    Debug.Assert(newPosition != Vector3.negativeInfinity, "Could not find first available space");
                    foundNewPosition = true;
                }
                else
                {
                    for (float y = selectedItem.transform.position.y - 1; y > bottomRightCorner.y; y--)
                    {
                        newPosition = new Vector3(newPosition.x, y, newPosition.z);
                        if (selectedItem.CompareTag("Table"))
                        {
                            if (FoodieSystem.inst.pathfinding.IsPlaceable(newPosition) &&
                                FoodieSystem.inst.pathfinding.IsPlaceable(newPosition + Vector3.left) &&
                                FoodieSystem.inst.pathfinding.IsPlaceable(newPosition + (2 * Vector3.left)))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                        else if (selectedItem.CompareTag("Cooktop"))
                        {
                            if (InKitchenBoundary(newPosition) && FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                        else if (selectedItem.CompareTag("Distraction"))
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                for (int j = -1; j < 2; j++)
                                {
                                    if(!FoodieSystem.inst.pathfinding.IsPlaceable(new Vector3(newPosition.x + i, newPosition.y + j, newPosition.z)))
                                    {
                                        continue;
                                    }
                                }
                            }
                            foundNewPosition = true;
                            break;
                        }
                    }
                }
                if (foundNewPosition)
                {
                    selectedItem.transform.position = newPosition;
                }
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
                    EventSystem.current.firstSelectedGameObject = tablesUpgradeButton.gameObject;
                    EventSystem.current.SetSelectedGameObject(tablesUpgradeButton.gameObject);
                }

            }
        }

    }

    Vector3 FirstAvailableSpace()
    {
        Vector3 newPosition;
        for (float x = topLeftCorner.x + 1; x < bottomRightCorner.x; x++)
        {
            for (float y = topLeftCorner.y - 1; y > bottomRightCorner.y; y--)
            {
                newPosition = new Vector3(x, y, 0);
                if (selectedItem.CompareTag("Table"))
                {
                    if (FoodieSystem.inst.pathfinding.IsPlaceable(newPosition) &&
                        FoodieSystem.inst.pathfinding.IsPlaceable(newPosition + Vector3.left) &&
                        FoodieSystem.inst.pathfinding.IsPlaceable(newPosition + (2 * Vector3.left)))
                    {
                        return newPosition;
                    }
                }
                else if (selectedItem.CompareTag("Cooktop"))
                {
                    if (InKitchenBoundary(newPosition) && FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
                    {
                        return newPosition;
                    }
                }
                else
                {
                    if (FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
                    {
                        return newPosition;
                    }
                }
            }
        }
        Debug.Log("could not find an available space");
        return Vector3.negativeInfinity;
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

    bool InMapBoundary(Vector3 position)
    {
        return topLeftCorner.x < position.x &&
                position.x < bottomRightCorner.x &&
                bottomRightCorner.y < position.y &&
                position.y < topLeftCorner.y;
    }

    void ChangeFloorColorTo(Color color)
    {
        foreach (SpriteRenderer rend in floors)
        {
            rend.color = color;
        }
    }




}
