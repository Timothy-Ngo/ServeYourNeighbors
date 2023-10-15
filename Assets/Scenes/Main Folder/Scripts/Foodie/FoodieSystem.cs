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

        

    }

    
    public Pathfinding pathfinding;
    
    [Header("Debugging")]
    public bool showDebug = false;
    public FoodieMovement[] foodieMove;
    public GameObject foodiesParent;

    [Header("Line State")]
    public List<FoodieMovement> line;
    public Vector3 startOfLine;
    public GameObject startOfLineObject;

    [Header("Order State")]
    public List<Vector3> seats;
    public Queue<Vector3> availableSeats;
    public GameObject tableChairParent;

    [Header("Leave State")]
    public Vector3 despawnPoint;
    public GameObject despawnPointObject;

    void Start()
    {
        startOfLine = startOfLineObject.transform.position;
        despawnPoint = despawnPointObject.transform.position;

        line = new List<FoodieMovement>();

        availableSeats = new Queue<Vector3>();

        foreach (Transform transform in tableChairParent.transform)
        {
            seats.Add(transform.position);
        }

        foreach (Vector3 table in seats)
        {
            availableSeats.Enqueue(table);
        }


    }

    
    void Update()
    {
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
