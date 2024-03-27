// Author: Timothy Ngo, Helen Truong
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public Vector3 position = Vector3.zero;

    /// <summary>
    /// If object exists at the very beginning of the scene, initialize it as an already placed object
    /// </summary>
    public bool startObject = false;

    private void Start()
    {
        if (startObject)
        {
            PlaceObstacle();
        }
    }


    void Update()
    {

    }

    public void RemoveObstacle()
    {
        position = transform.position;
        FoodieSystem.inst.pathfinding.obstaclePositions.Add(position);
        int x = 0;
        int y = 0;
        // sets position on pathfinding grid as walkable
        FoodieSystem.inst.pathfinding.GetGrid().GetXY(transform.position, out x, out y);
        FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsWalkable(true);

        if (gameObject.CompareTag("Table"))
        {
            //Debug.Log("Table object");
            FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsPlaceable(true);
            FoodieSystem.inst.pathfinding.GetNode(x - 1, y).SetIsPlaceable(true);
            FoodieSystem.inst.pathfinding.GetNode(x - 2, y).SetIsPlaceable(true);
        }
        else if (gameObject.CompareTag("Distraction"))
        {
            //Debug.Log("Distraction object");
            for (int j = 0; j < 1; j++)
            {
                FoodieSystem.inst.pathfinding.GetNode(x, y + j).SetIsPlaceable(true);
            }

        }
        else if (gameObject.CompareTag("Cooktop"))
        {
            FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsPlaceable(true);
        }
        else if (gameObject.CompareTag("Counter"))
        {
            FoodieSystem.inst.pathfinding.GetNode(x - 1, y).SetIsPlaceable(true);
            FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsPlaceable(true);
            //FoodieSystem.inst.pathfinding.GetNode(x + 1, y).SetIsPlaceable(true);
        }
        else if (gameObject.CompareTag("PlayerSpawn"))
        {
            FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsPlaceable(true);
        }
        else
        {
            FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsPlaceable(true);
            //FoodieSystem.inst.pathfinding.GetNode(x - 1, y).SetIsPlaceable(true);
        }

        // for debugging
        if (FoodieSystem.inst.showDebug)
            FoodieSystem.inst.pathfinding.GetGrid().GetDebugTextArray()[x, y].color = Color.red;
    }



    public void PlaceObstacle()
    {
        //Debug.Log("PLACING OBSTACLE");
        position = transform.position;
        FoodieSystem.inst.pathfinding.obstaclePositions.Add(position);
        int x = 0;
        int y = 0;
        // sets position on pathfinding grid as unwalkable
        FoodieSystem.inst.pathfinding.GetGrid().GetXY(transform.position, out x, out y);
        FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsWalkable(false);

        if (gameObject.CompareTag("Table"))
        {
            //Debug.Log("Table object");
            FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsPlaceable(false);
            FoodieSystem.inst.pathfinding.GetNode(x - 1, y).SetIsPlaceable(false);
            FoodieSystem.inst.pathfinding.GetNode(x - 2, y).SetIsPlaceable(false);
        }
        else if (gameObject.CompareTag("Distraction"))
        {
            //Debug.Log("Distraction object");
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    FoodieSystem.inst.pathfinding.GetNode(x + i, y + j).SetIsPlaceable(false);
                }
            }
        }
        else if (gameObject.CompareTag("Cooktop"))
        {
            FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsPlaceable(false);
        }
        else if (gameObject.CompareTag("PlayerSpawn"))
        {
            FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsPlaceable(false);
        }
        else
        {
            FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsPlaceable(false);
            //FoodieSystem.inst.pathfinding.GetNode(x - 1, y).SetIsPlaceable(false);
        }

        // for debugging
        if (FoodieSystem.inst.showDebug)
            FoodieSystem.inst.pathfinding.GetGrid().GetDebugTextArray()[x, y].color = Color.red;
    }
}
