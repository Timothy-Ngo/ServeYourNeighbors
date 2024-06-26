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
    [SerializeField] GameObject errorMsg;


    //Boundaries
    [Header("Whole Map Boundary")]
    public GameObject topLeftCornerObj;
    public GameObject bottomRightCornerObj;
    public Vector2 topLeftCorner;
    public Vector2 bottomRightCorner;


    [SerializeField] Button tablesUpgradeButton;

    [SerializeField] Button confirmYesButton;
    [SerializeField] GameObject cancelButton;

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

    public bool _isEnabled;
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

    [Header("Placement Mode")]
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
        errorMsg.SetActive(false);
        this.gameObject.SetActive(false);
    }

    private void Enabled(bool enable)
    {
        
        if (Upgrades.inst.upgradesScreen.activeSelf)
        {
            Upgrades.inst.upgradesScreen.SetActive(false);

        }
        if (enable)
        {
            instructions.SetActive(true);
            topUIObject.SetActive(true);
            bottomUIObject.SetActive(true);
            errorMsg.SetActive(false);
            cancelButton.SetActive(true);
            ChangeFloorColorTo(placementFloorColor);

            Obstacle[] tableObstacles = Upgrades.inst.tablesParent.GetComponentsInChildren<Obstacle>(); // Ensure all obstacles that need to be placed in placement mode are placed, typically just the tables. These obstacles are supposed to be disabled when out of placement mode.
            foreach (Obstacle obs in tableObstacles)
            {
                if (obs.placementObstacle)
                {
                    obs.PlaceObstacle();
                }
            }

            string normalInstructions = $"Use the arrow keys to move object or\r\nleft click where you want to place your item.\r\nThen, press {InputSystem.inst.finishPlacementKey.ToString()} to confirm placement";
            string changeLayoutInstructions = $"Use the arrow keys to move object and tab to switch between objects or just use left click and drag.\r\nThen, press {InputSystem.inst.finishPlacementKey.ToString()} to confirm placement";
            
            if (Upgrades.inst.changeLayoutMode)
            {
                instructionsTextMesh.text = changeLayoutInstructions;
                selectedItem = Upgrades.inst.selectableItems[currentIndex];
                SelectItem(selectedItem);
                
            }
            else
            {
                instructionsTextMesh.text = normalInstructions;
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
                else if (Upgrades.inst.animatronicPlacementMode)
                {
                    selectedItem = Instantiate(prefabs[3], startingPositionObject.transform.position, Quaternion.identity);
                    selectedItem.transform.parent = distractionParent.transform;
                    DistractionSystem.inst.animatronicDistraction = selectedItem.GetComponent<Distraction>();
                }
            }
            isDragging = false;
        }
        else
        {
            instructions.SetActive(false);
            topUIObject.SetActive(false);
            bottomUIObject.SetActive(false);
            
            // Remove obstacles that only needed to be active in placement mode
            Obstacle[] tableObstacles = Upgrades.inst.tablesParent.GetComponentsInChildren<Obstacle>();
            foreach (Obstacle obs in tableObstacles) 
            {   
                if (obs.placementObstacle)
                {
                    obs.RemoveObstacle();
                }
            }
           DeselectAllItems();
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

            if (Upgrades.inst.changeLayoutMode)
            {
                if (Input.GetKeyDown(KeyCode.Tab) && !isDragging)
                {
                    PlaceAllObstaclesOf(selectedItem);
                    currentIndex = (currentIndex + 1) % Upgrades.inst.selectableItems.Count; // Keeps currentIndex within index range
                    Debug.Assert(currentIndex < Upgrades.inst.selectableItems.Count);
                    DeselectAllItems();

                    selectedItem = Upgrades.inst.selectableItems[currentIndex];
                    SelectItem(selectedItem);
                    PlaceAllObstaclesOf(selectedItem);
                }
            }
            if (Input.GetMouseButtonDown(0)) // For dragging items
            {
                if (Upgrades.inst.changeLayoutMode)
                {
                    Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Collider2D collider = Physics2D.OverlapPoint(worldPosition);
                    
                    if (collider != null)
                    {
                        PlaceAllObstaclesOf(selectedItem);
                        DeselectAllItems();
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
                            selectedItem = collider.gameObject;
                        }
                        currentIndex = Upgrades.inst.selectableItems.IndexOf(selectedItem);
                        SelectItem(selectedItem);
                    }
                    RemoveAllObstaclesOf(selectedItem);
                }
                else
                {
                    Obstacle[] removeObstacles = selectedItem.GetComponentsInChildren<Obstacle>();
                    foreach (Obstacle obstacle in removeObstacles)
                    {
                        obstacle.RemoveObstacle();
                    }
                }
                isDragging = true;
            }



            if (isDragging)
            {
                Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (InMapBoundary(worldMousePosition))
                {
                    newPosition = new Vector3(Mathf.RoundToInt((worldMousePosition + new Vector3(0, 0, 10)).x) - 0.5f,
                        Mathf.RoundToInt((worldMousePosition + new Vector3(0, 0, 10)).y) - 0.5f, 0f);
                    if (IsPlaceable(newPosition))
                    {
                        errorMsg.SetActive(false);
                        instructionsTextMesh.color = Color.white;
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
                                else // Only other item should be the animatronic
                                {
                                    selectedItem.transform.position = newPosition;
                                }
                            }
                        }
                    }

                }

            }
            if (Input.GetMouseButtonUp(0))
            {
                errorMsg.SetActive(false);
                instructionsTextMesh.color = Color.white;
                isDragging = false;
                PlaceAllObstaclesOf(selectedItem);
                /*
                if (Upgrades.inst.changeLayoutMode)
                {
                    Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Collider2D collider = Physics2D.OverlapPoint(worldPosition);
                    if (collider != null)
                    {
                        PlaceAllObstaclesOf(selectedItem);
                    }
                    
                }
                else
                {

                }
                */
            }
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
                        else
                        {

                            if (InKitchenBoundary(newPosition) && IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                    }
                }
                if (foundNewPosition)
                {
                    errorMsg.SetActive(false);
                    instructionsTextMesh.color = Color.white;
                    RemoveAllObstaclesOf(selectedItem);

                    selectedItem.transform.position = newPosition;

                    PlaceAllObstaclesOf(selectedItem);
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

                            if (InKitchenBoundary(newPosition) && IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                    }
                }
                if (foundNewPosition)
                {
                    errorMsg.SetActive(false);
                    instructionsTextMesh.color = Color.white;
                    RemoveAllObstaclesOf(selectedItem);
                    selectedItem.transform.position = newPosition;
                    PlaceAllObstaclesOf(selectedItem);
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

                            if (InKitchenBoundary(newPosition) && IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                    }
                }
                if (foundNewPosition)
                {
                    errorMsg.SetActive(false);
                    instructionsTextMesh.color = Color.white;
                    RemoveAllObstaclesOf(selectedItem);

                    selectedItem.transform.position = newPosition;
                    PlaceAllObstaclesOf(selectedItem);
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

                            if (InKitchenBoundary(newPosition) && IsPlaceable(newPosition))
                            {
                                foundNewPosition = true;
                                break;
                            }
                        }
                    }
                }
                if (foundNewPosition)
                {
                    errorMsg.SetActive(false);
                    instructionsTextMesh.color = Color.white;
                    RemoveAllObstaclesOf(selectedItem);

                    selectedItem.transform.position = newPosition;

                    PlaceAllObstaclesOf(selectedItem);
                }
            }



            if (Input.GetKeyDown(InputSystem.inst.finishPlacementKey) && !placementConfirmationScreen.activeSelf)
            {
                if (selectedItem.transform.position == startingPositionObject.transform.position)
                {
                    Debug.Log("Item must be placed in restaurant.");
                    instructionsTextMesh.color = Color.red;
                }
                else
                {
                    // check if foodie can find a path to table
                    if (selectedItem.CompareTag("Table") || selectedItem.CompareTag("Distraction"))
                    {

                        bool pathExists = true;
                        foreach (Table table in FoodieSystem.inst.tables) // check if there exists a path to every table
                        {
                            GameObject tableObject = table.gameObject;

                            Obstacle[] tempObstacles = tableObject.GetComponentsInChildren<Obstacle>();
                            foreach (Obstacle obstacle in tempObstacles)
                            {
                                obstacle.RemoveObstacle();
                            }
                            List<Vector3> path = FoodieSystem.inst.pathfinding.FindPath(FoodieSystem.inst.startOfLine, tableObject.transform.position);
                            foreach (Obstacle obstacle in tempObstacles)
                            {
                                obstacle.PlaceObstacle();
                            }

                            if (path == null)
                            {
                                pathExists = false;
                            }

                        }

                        if (pathExists)
                        {
                            placementConfirmationScreen.SetActive(true);
                            confirmYesButton.Select();
                            cancelButton.SetActive(false);
                        }
                        else
                        {
                            errorMsg.SetActive(true);
                            Debug.Log("INVALID PLACEMENT: path cannot be found to position");
                        }
                    }
                    else
                    {
                        placementConfirmationScreen.SetActive(true);
                        confirmYesButton.Select();
                        cancelButton.SetActive(false);
                    }

                }
            }

            if (Input.GetKeyDown(InputSystem.inst.pauseKey))
            {
                ExitPlacementMode();
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
                        IsPlaceable(newPosition) &&
                        IsPlaceable(newPosition + Vector3.right))
                    {
                        return newPosition;
                    }
                }
                else if (selectedItem.CompareTag("Cooktop"))
                {
                    if (InKitchenBoundary(newPosition) && IsPlaceable(newPosition))
                    {
                        return newPosition;
                    }
                }
                else if (selectedItem.CompareTag("Distraction"))
                {
                    if (!InKitchenBoundary(newPosition) && IsPlaceable(newPosition))
                    {
                        return newPosition;
                    }
                }
                else
                {
                    if (IsPlaceable(newPosition))
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

    void DeselectAllItems()
    {
        foreach (GameObject item in Upgrades.inst.selectableItems)
        {
            DeselectItem(item);
        }
    }

    void PlaceAllObstaclesOf(GameObject parentItem) // Must be parent object of gameobjects containing the obstacles. Will place placementObstacles as well
    {
        Obstacle[] obstacles = parentItem.GetComponentsInChildren<Obstacle>();
        foreach (Obstacle obstacle in obstacles)
        {
            obstacle.PlaceObstacle();
        }
    }

    void RemoveAllObstaclesOf(GameObject parentItem) // Must be parent object of gameobjects containing the obstacles. Will place placementObstacles as well
    {
        Obstacle[] obstacles = parentItem.GetComponentsInChildren<Obstacle>();
        foreach (Obstacle obstacle in obstacles)
        {
            obstacle.RemoveObstacle();
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
                tablesUpgradeButton.Select();
            }
            if (Upgrades.inst.changeLayoutMode)
            {
                Upgrades.inst.changeLayoutMode = false;
                FoodieSystem.inst.GetCurrentSeats();
            }
            else
            {
                if (Upgrades.inst.tablePlacementMode)
                {
                    FoodieSystem.inst.GetCurrentSeats();
                    Upgrades.inst.tablePlacementMode = false;
                    selectedItem.GetComponent<Table>().obstacleScript.PlaceObstacle();
                }

                if (Upgrades.inst.cookStationPlacementMode)
                {
                    Upgrades.inst.cookStationPlacementMode = false;
                    selectedItem.GetComponent<Cooking>().obstacleScript.PlaceObstacle();
                    //Upgrades.inst.cookStations.Add(selectedItem);
                }

                if (Upgrades.inst.animatronicPlacementMode)
                {
                    Upgrades.inst.animatronicPlacementMode = false;
                    selectedItem.GetComponent<Distraction>().obstacleScript.PlaceObstacle();
                    Upgrades.inst.animatronicPosition = selectedItem.transform.position;
                    Upgrades.inst.hasAnimatronic = true;
                    DistractionSystem.inst.animatronicDistraction = selectedItem.GetComponent<Distraction>();
                }
            }
            EventSystem.current.firstSelectedGameObject = tablesUpgradeButton.gameObject;
            EventSystem.current.SetSelectedGameObject(tablesUpgradeButton.gameObject);
            this.gameObject.SetActive(false);
        }

    }

    public void ConfirmNo()
    {
        placementConfirmationScreen.SetActive(false);
        cancelButton.SetActive(true);
    }


    public void ExitPlacementMode()
    {
        isEnabled = false;
        if (!Upgrades.inst.upgradesScreen.activeSelf)
        {
            ChangeFloorColorTo(originalFloorColor);
            Upgrades.inst.upgradesScreen.SetActive(true);
            tablesUpgradeButton.Select();
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
        else
        {
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
                Upgrades.inst.animatronicDescription.gameObject.transform.parent.GetComponent<Button>().interactable = true;
                Destroy(selectedItem);
            }
        }
        
        this.gameObject.SetActive(false);

    }

}
