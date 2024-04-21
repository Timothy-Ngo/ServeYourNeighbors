// Author: Helen Truong
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FoodieSystem : MonoBehaviour
{
    public static FoodieSystem inst;
    
    [SerializeField] private int gridWidth = 16;
    [SerializeField] private int gridHeight = 9;
    [SerializeField] private Vector3 startPoint = Vector3.zero;
    [SerializeField] private float cellSize = 1.0f;
    private void Awake()
    {
        inst = this;
        pathfinding = new Pathfinding(gridWidth, gridHeight, startPoint, cellSize); // initializes pathfinding grid
                                                                                    // Note: this warning is given on this line, be aware incase future problems arise ("You are trying to create a MonoBehaviour using the 'new' keyword.  This is not allowed.  MonoBehaviours can only be added using AddComponent(). Alternatively, your script can inherit from ScriptableObject or no base class at all UnityEngine.MonoBehaviour:.ctor()")

    }


    
    public Pathfinding pathfinding;
    
    [Header("-----DEBUGGING-----")]
    public bool showDebug = false;
    public FoodieMovement[] foodieMove;
    public GameObject foodiesParent;

    [Header("-----LINE STATE-----")]
    public List<FoodieMovement> line;
    public Vector3 startOfLine;
    public GameObject startOfLineObject;

    [Header("-----ORDER STATE-----")]
    public List<Vector3> seats;
    public Queue<Vector3> availableSeats;
    public List<Vector3> blockedSeats;
    public GameObject tableChairParent;
    public List<Table> tables;

    [Header("-----LEAVE STATE-----")]
    public Vector3 despawnPoint;
    public GameObject despawnPointObject;

    [Header("-----FOODIE SIGHT-----")]
    public bool sightToggleEnabled = false;

    void Start()
    {
        startOfLine = startOfLineObject.transform.position;
        despawnPoint = despawnPointObject.transform.position;

        line = new List<FoodieMovement>();

        availableSeats = new Queue<Vector3>();

        GetCurrentSeats();

    }

    public void GetCurrentSeats()
    {
        seats.Clear();
        availableSeats.Clear();
        tables.Clear();
        foreach (Transform transform in tableChairParent.transform)
        {
            tables.Add(transform.gameObject.GetComponent<Table>());
            if (transform.gameObject.activeSelf)
            {
                seats.Add(transform.position);
            }
        }

        foreach (Vector3 table in seats)
        {
            availableSeats.Enqueue(table);
        }

    }

    
    void Update()
    {
        if (Input.GetKeyDown(InputSystem.inst.foodieSightKey))
        {
            sightToggleEnabled = !sightToggleEnabled;
        }

        if (showDebug)
        {
            if (Input.GetMouseButtonDown(0))
            {
                foodieMove = foodiesParent.GetComponentsInChildren<FoodieMovement>();

                foreach (FoodieMovement foodie in foodieMove)
                {
                    Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
                    pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
                    float cellSize = pathfinding.GetGrid().GetCellSize();

                    pathfinding.GetGrid().GetXY(foodie.transform.position, out int foodiePositionX, out int foodiePositionY);
                    List<PathNode> path = pathfinding.FindPath(foodiePositionX, foodiePositionY, x, y);
                    foodie.SetTargetPosition(mouseWorldPosition, pathfinding);


                    if (path != null)
                    {
                        foreach (PathNode pathNode in path)
                        {
                            for (int i = 0; i < path.Count - 1; i++)
                            {
                                Debug.DrawLine(new Vector3(path[i].x, path[i].y) * cellSize + Vector3.one * cellSize/2, new Vector3(path[i + 1].x, path[i + 1].y) * cellSize + Vector3.one * cellSize/2, Color.green, 1f);

                            }
                        }
                    }
                    else
                    {
                        Debug.Log("invalid path");
                    }
                }
            }
        }
    }
}
