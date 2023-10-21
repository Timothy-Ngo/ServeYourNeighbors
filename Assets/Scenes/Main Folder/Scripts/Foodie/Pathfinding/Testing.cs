using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private Pathfinding pathfinding;

    void Start()
    {
        pathfinding = new Pathfinding(15, 9, Vector3.zero, 1f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            float cellSize = pathfinding.GetGrid().GetCellSize();

            List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
            //SetTargetPosition(mouseWorldPosition, pathfinding);


            if (path != null)
            {
                foreach (PathNode pathNode in path)
                {
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        //Debug.DrawLine(new Vector3(path[i].x, path[i].y) * cellSize + Vector3.one * cellSize/2, new Vector3(path[i + 1].x, path[i + 1].y) * cellSize + Vector3.one * cellSize/2, Color.green, 1f);

                    }
                }
            }
            else
            {
                Debug.Log("invalid path");
            }
            

        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
            if (!pathfinding.GetNode(x, y).isWalkable)
                pathfinding.GetGrid().GetDebugTextArray()[x, y].color = Color.red;
            else
                pathfinding.GetGrid().GetDebugTextArray()[x, y].color = Color.white;
        }
    }


}
