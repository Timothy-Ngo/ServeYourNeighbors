using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System;




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
    public GameObject selectedItem;
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

    // Keyboard Input
    [Header("Keyboard Input")]
    int currentIndex = 0;


    [Header("Change Layout Old Positions")]
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

    [SerializeField] GameObject placementConfirmationScreen;


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
            Obstacle[] tableObstacles = Upgrades.inst.tablesParent.GetComponentsInChildren<Obstacle>();
            foreach (Obstacle obs in tableObstacles)
            {
                if (obs.placementObstacle)
                {
                    obs.PlaceObstacle();
                }
            }

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
            else if (Upgrades.inst.counterPlacementMode) // Deprecated
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
            
            Obstacle[] tableObstacles = Upgrades.inst.tablesParent.GetComponentsInChildren<Obstacle>();
            Debug.Log("Placement disabled");
            foreach (Obstacle obs in tableObstacles)
            {   
                if (obs.placementObstacle)
                {
                    obs.RemoveObstacle();
                    Debug.Log(FoodieSystem.inst.pathfinding.IsWalkable(obs.transform.position));
                }
                if (obs.showObstacle) // Does not include walls
                {
                    obs.showObstacle = false;
                }
            }
           
        }


    }

    bool IsPlaceable(Vector3 position) // Did not want to write all that extra stuff everytime I used it
    {
        return FoodieSystem.inst.pathfinding.IsPlaceable(position);
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled && !placementConfirmationScreen.activeSelf)
        {

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Vector3 oldPosition = selectedItem.transform.position;
                Vector3 newPosition = oldPosition;
                bool foundNewPosition = false;

                if (!InMapBoundary(newPosition))
                {
                    newPosition = FirstAvailableSpace();
                    Debug.Assert(newPosition != Vector3.negativeInfinity, "Could not find first available space");
                    foundNewPosition = true;
                }
                else
                {
                    for (float x = oldPosition.x; x < bottomRightCorner.x; x++)
                    {
                        newPosition = new Vector3(x, newPosition.y, newPosition.z);
                        if (selectedItem.CompareTag("Table"))
                        {
                            Vector3 oldChairPosition = oldPosition;
                            Vector3 oldTablePosition = oldChairPosition + Vector3.right;
                            Vector3 newChairPosition = new Vector3(x, oldChairPosition.y, oldChairPosition.z);
                            Vector3 newTablePosition = newChairPosition + Vector3.right; // Table Position is always one to the right of the Chair
                            if (InMapBoundary(newChairPosition) && InMapBoundary(newTablePosition) &&
                                !InKitchenBoundary(newChairPosition) && !InKitchenBoundary(newTablePosition))
                            {
                                if (IsPlaceable(newTablePosition))
                                {
                                    if (IsPlaceable(newChairPosition))
                                    {
                                        foundNewPosition = true;
                                        newPosition = newChairPosition;
                                        break;
                                    }
                                    else if (newChairPosition == oldTablePosition)
                                    {
                                        foundNewPosition = true;
                                        newPosition = newChairPosition;
                                        break;
                                    }
                                }
                            }

                        }
                        else if (selectedItem.CompareTag("Distraction"))
                        {
                            if (!InKitchenBoundary(newPosition) && IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                        else if (selectedItem.CompareTag("Cooktop"))
                        {
                            if (InKitchenBoundary(newPosition) && IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                        else
                        {

                            if (InKitchenBoundary(newPosition) && FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                    }
                }
                if (foundNewPosition)
                {
                    Obstacle[] removeObstacles = selectedItem.GetComponentsInChildren<Obstacle>();
                    foreach (Obstacle obstacle in removeObstacles)
                    {
                        obstacle.RemoveObstacle();
                    }

                    selectedItem.transform.position = newPosition;

                    Obstacle[] placeObstacles = selectedItem.GetComponentsInChildren<Obstacle>();
                    foreach (Obstacle obstacle in placeObstacles)
                    {
                        obstacle.PlaceObstacle();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Vector3 oldPosition = selectedItem.transform.position;
                Vector3 newPosition = oldPosition;
                bool foundNewPosition = false;

                if (!InMapBoundary(newPosition))
                {
                    newPosition = FirstAvailableSpace();
                    Debug.Assert(newPosition != Vector3.negativeInfinity, "Could not find first available space");
                    foundNewPosition = true;
                }
                else
                {
                    for (float x = oldPosition.x; x > topLeftCorner.x; x--)
                    {
                        newPosition = new Vector3(x, newPosition.y, newPosition.z);
                        if (selectedItem.CompareTag("Table"))
                        {
                            Vector3 oldChairPosition = oldPosition;
                            Vector3 oldTablePosition = oldChairPosition + Vector3.right;
                            Vector3 newChairPosition = new Vector3(x, oldChairPosition.y, oldChairPosition.z);
                            Vector3 newTablePosition = newChairPosition + Vector3.right; // Table Position is always one to the right of the Chair
                            if (InMapBoundary(newChairPosition) && InMapBoundary(newTablePosition) &&
                                !InKitchenBoundary(newChairPosition) && !InKitchenBoundary(newTablePosition))
                            {
                                if (IsPlaceable(newChairPosition))
                                {
                                    if (IsPlaceable(newTablePosition))
                                    {
                                        foundNewPosition = true;
                                        newPosition = newChairPosition;
                                        break;
                                    }
                                    else if (newTablePosition == oldChairPosition)
                                    {
                                        foundNewPosition = true;
                                        newPosition = newChairPosition;
                                        break;
                                    }
                                }
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
                            if (!InKitchenBoundary(newPosition) && IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                        else
                        {

                            if (InKitchenBoundary(newPosition) && FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                    }
                }
                if (foundNewPosition)
                {
                    Obstacle[] removeObstacles = selectedItem.GetComponentsInChildren<Obstacle>();
                    foreach (Obstacle obstacle in removeObstacles)
                    {
                        obstacle.RemoveObstacle();
                    }

                    selectedItem.transform.position = newPosition;

                    Obstacle[] placeObstacles = selectedItem.GetComponentsInChildren<Obstacle>();
                    foreach (Obstacle obstacle in placeObstacles)
                    {
                        obstacle.PlaceObstacle();
                    }
                }
            }


            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Vector3 oldPosition = selectedItem.transform.position;
                Vector3 newPosition = oldPosition;
                bool foundNewPosition = false;

                if (!InMapBoundary(newPosition))
                {
                    newPosition = FirstAvailableSpace();
                    Debug.Assert(newPosition != Vector3.negativeInfinity, "Could not find first available space");
                    foundNewPosition = true;
                }
                else
                {
                    for (float y = oldPosition.y; y < topLeftCorner.y; y++)
                    {
                        newPosition = new Vector3(newPosition.x, y, newPosition.z);
                        if (selectedItem.CompareTag("Table"))
                        {
                            Vector3 oldChairPosition = oldPosition;
                            Vector3 newChairPosition = new Vector3(oldChairPosition.x, y, oldChairPosition.z);
                            Vector3 newTablePosition = newChairPosition + Vector3.right; // Table Position is always one to the right of the Chair
                            if (InMapBoundary(newChairPosition) && InMapBoundary(newTablePosition) &&
                                !InKitchenBoundary(newChairPosition) && !InKitchenBoundary(newTablePosition))
                            {
                                if (IsPlaceable(newChairPosition) && IsPlaceable(newTablePosition))
                                {
                                    foundNewPosition = true;
                                    newPosition = newChairPosition;
                                    break;
                                }
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
                            if (!InKitchenBoundary(newPosition) && IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                        else
                        {

                            if (InKitchenBoundary(newPosition) && FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                    }
                }
                if (foundNewPosition)
                {
                    Obstacle[] removeObstacles = selectedItem.GetComponentsInChildren<Obstacle>();
                    foreach (Obstacle obstacle in removeObstacles)
                    {
                        obstacle.RemoveObstacle();
                    }

                    selectedItem.transform.position = newPosition;

                    Obstacle[] placeObstacles = selectedItem.GetComponentsInChildren<Obstacle>();
                    foreach (Obstacle obstacle in placeObstacles)
                    {
                        obstacle.PlaceObstacle();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Vector3 oldPosition = selectedItem.transform.position;
                Vector3 newPosition = oldPosition;
                bool foundNewPosition = false;
                if (!InMapBoundary(newPosition))
                {
                    newPosition = FirstAvailableSpace();
                    Debug.Assert(newPosition != Vector3.negativeInfinity, "Could not find first available space");
                    foundNewPosition = true;
                }
                else
                {
                    for (float y = oldPosition.y; y > bottomRightCorner.y; y--)
                    {
                        newPosition = new Vector3(newPosition.x, y, newPosition.z);
                        if (selectedItem.CompareTag("Table"))
                        {
                            Vector3 oldChairPosition = oldPosition;
                            Vector3 newChairPosition = new Vector3(oldChairPosition.x, y, oldChairPosition.z);
                            Vector3 newTablePosition = newChairPosition + Vector3.right; // Table Position is always one to the right of the Chair
                            if (InMapBoundary(newChairPosition) && InMapBoundary(newTablePosition) &&
                                !InKitchenBoundary(newChairPosition) && !InKitchenBoundary(newTablePosition))
                            {
                                if (IsPlaceable(newChairPosition) && IsPlaceable(newTablePosition))
                                {
                                    foundNewPosition = true;
                                    newPosition = newChairPosition;
                                    break;
                                }
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
                            if (!InKitchenBoundary(newPosition) && IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                        else
                        {

                            if (InKitchenBoundary(newPosition) && FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                    }
                }
                if (foundNewPosition)
                {
                    Obstacle[] removeObstacles = selectedItem.GetComponentsInChildren<Obstacle>();
                    foreach (Obstacle obstacle in removeObstacles)
                    {
                        obstacle.RemoveObstacle();
                    }

                    selectedItem.transform.position = newPosition;

                    Obstacle[] placeObstacles = selectedItem.GetComponentsInChildren<Obstacle>();
                    foreach (Obstacle obstacle in placeObstacles)
                    {
                        obstacle.PlaceObstacle();
                    }
                }
            }

            if (Upgrades.inst.changeLayoutMode)
            {

                if (selectedItem == null)
                {
                    selectedItem = Upgrades.inst.selectableItems[currentIndex];
                    SelectItem(selectedItem);
                }
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    currentIndex++;
                    if (currentIndex >= Upgrades.inst.selectableItems.Count)
                    {
                        currentIndex = 0;
                    }
                    DeselectItem(selectedItem);
                    selectedItem = Upgrades.inst.selectableItems[currentIndex];
                    SelectItem(selectedItem);
                }
            }

            if (Input.GetMouseButtonDown(0) && !placementConfirmationScreen.activeSelf) // For dragging items
            {
                Obstacle[] removeObstacles = selectedItem.GetComponentsInChildren<Obstacle>();
                foreach (Obstacle obstacle in removeObstacles)
                {
                    obstacle.RemoveObstacle();
                }
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
            }



            if (Input.GetMouseButtonUp(0) && !placementConfirmationScreen.activeSelf)
            {
                isDragging = false;
                Obstacle[] removeObstacles = selectedItem.GetComponentsInChildren<Obstacle>();
                foreach (Obstacle obstacle in removeObstacles)
                {
                    obstacle.PlaceObstacle();
                }

            }
            if (isDragging && !placementConfirmationScreen.activeSelf)
            {
                Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (topLeftCorner.x < worldMousePosition.x && worldMousePosition.x < bottomRightCorner.x && bottomRightCorner.y < worldMousePosition.y && worldMousePosition.y < topLeftCorner.y)
                {
                    newPosition = new Vector3(Mathf.RoundToInt((worldMousePosition + new Vector3(0, 0, 10)).x) - 0.5f,
                        Mathf.RoundToInt((worldMousePosition + new Vector3(0, 0, 10)).y) - 0.5f, 0f);
                    if (IsPlaceable(newPosition))
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
                                if (selectedItem.CompareTag("Table"))
                                {
                                    if (IsPlaceable(newPosition + Vector3.right) && !InKitchenBoundary(newPosition + Vector3.right))
                                    {
                                        selectedItem.transform.position = newPosition;
                                    }
                                }
                                else
                                {
                                    selectedItem.transform.position = newPosition;
                                }
                            }
                        }
                    }

                }

            }
            if (Input.GetKeyDown(InputSystem.inst.finishPlacementKey) && !placementConfirmationScreen.activeSelf)
            {
                if (selectedItem.transform.position == startingPositionObject.transform.position)
                {
                    Debug.Log("Item must be placed in restaurant.");
                    // TODO: Highlight instruction text
                }
                else
                {
                    placementConfirmationScreen.SetActive(true);
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

                    if (!InKitchenBoundary(newPosition) &&
                        !InKitchenBoundary(newPosition + Vector3.right) &&
                        FoodieSystem.inst.pathfinding.IsPlaceable(newPosition) &&
                        FoodieSystem.inst.pathfinding.IsPlaceable(newPosition + Vector3.right))
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
                else if (selectedItem.CompareTag("Distraction"))
                {
                    if (!InKitchenBoundary(newPosition) && FoodieSystem.inst.pathfinding.IsPlaceable(newPosition))
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
        Debug.Log($"topleft position {topLeft}");
        Vector3 bottomRight = Layout.inst.bottomRightKitchenBoundaryPosition;
        Debug.Log($"bottomeRight position {bottomRight}");
        bool withinXRange = topLeft.x <= position.x && position.x <= bottomRight.x;
        bool withinYRange = bottomRight.y <= position.y && position.y <= topLeft.y;
        if (withinXRange)
        {
            Debug.Log($"position {position.x} is in kitchen x range");
        }
        else
        {
            Debug.Log($"position {position.x} is NOT in kitchen x range");
        }
        if (withinYRange)
        {
            Debug.Log($"position {position.y} is in kitchen y range");
        }
        else
        {
            Debug.Log($"position {position.y} is NOT in kitchen y range");
        }
        return withinXRange && withinYRange;
    }

    bool InMapBoundary(Vector3 position)
    {
        return topLeftCorner.x < position.x &&
                position.x < bottomRightCorner.x &&
                bottomRightCorner.y < position.y &&
                position.y < topLeftCorner.y;
    }


    void SelectItem(GameObject item) // Assumes item has an obstacle child object
    {
        Obstacle[] obstacles = item.GetComponentsInChildren<Obstacle>();
        foreach (Obstacle obstacle in obstacles)
        {
            obstacle.showObstacle = true;
        }
    }
    void DeselectItem(GameObject item) // Assumes item has an obstacle child object
    {
        Obstacle[] obstacles = item.GetComponentsInChildren<Obstacle>();
        foreach (Obstacle obstacle in obstacles)
        {
            obstacle.showObstacle = false;
        }
    }
    void ChangeFloorColorTo(Color color)
    {
        foreach (SpriteRenderer rend in floors)
        {
            rend.color = color;
        }
    }



    public void ConfirmYes()
    {
        placementConfirmationScreen.SetActive(false);

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

    public void ConfirmNo()
    {
        placementConfirmationScreen.SetActive(false);
    }


    public void ExitPlacementMode()
    {
        isEnabled = false;
        if (!Upgrades.inst.upgradesScreen.activeSelf)
        {
            ChangeFloorColorTo(originalFloorColor);
            Upgrades.inst.upgradesScreen.SetActive(true);
        }

        foreach (Obstacle obstacle in selectedItem.GetComponentsInChildren<Obstacle>())
        {
            obstacle.RemoveObstacle();
        }

        if (Upgrades.inst.changeLayoutMode)
        {
            Upgrades.inst.changeLayoutMode = false;
            Currency.inst.Deposit(Upgrades.inst.changeLayoutCost);
            Upgrades.inst.RestoreItemPositions();
            return;
        }
        if (Upgrades.inst.tablePlacementMode)
        {
            Upgrades.inst.tablePlacementMode = false;
            Currency.inst.Deposit(Upgrades.inst.tablesUpgradeCost);
            Upgrades.inst.tables.Remove(selectedItem);
            Destroy(selectedItem);
        }

        if (Upgrades.inst.cookStationPlacementMode)
        {
            Upgrades.inst.cookStationPlacementMode = false;
            Currency.inst.Deposit(Upgrades.inst.cookStationsUpgradeCost);
            Upgrades.inst.cookStations.Remove(selectedItem);
            Destroy(selectedItem);
        }

        if (Upgrades.inst.animatronicPlacementMode)
        {
            Upgrades.inst.animatronicPlacementMode = false;
            Currency.inst.Deposit(Upgrades.inst.animatronicUpgradeCost);
            Upgrades.inst.hasAnimatronic = false;
            DistractionSystem.inst.animatronicDistraction = null;
            Destroy(selectedItem);
        }
        
    }

}
