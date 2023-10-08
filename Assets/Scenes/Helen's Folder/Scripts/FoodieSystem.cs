using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodieSystem : MonoBehaviour
{
    public static FoodieSystem inst;
    private void Awake()
    {
        inst = this;
        pathfinding = new Pathfinding(15, 9); // initializes pathfinding grid
    }

    public Pathfinding pathfinding;

    [Header("Debugging")]
    public bool showDebug = false;
    public FoodieMovement[] foodieMove;
    public GameObject foodiesParent;

    [Header("Line")]
    public Queue<FoodieMovement> line;
    public Vector3 lineStartPosition;
    public FoodieMovement lastInLine;
    public bool lineUpdated = false;

    [Header("Seats")]
    public Vector3[] seats;
    public Queue<Vector3> availableSeats;
    

    void Start()
    {
        line = new Queue<FoodieMovement>();
        availableSeats = new Queue<Vector3>();

        foreach (Vector3 seat in seats)
            availableSeats.Enqueue(seat);

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
