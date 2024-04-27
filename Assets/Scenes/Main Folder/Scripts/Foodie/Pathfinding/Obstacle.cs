// Author: Timothy Ngo, Helen Truong
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    SpriteRenderer obstacleRend;

    public bool showObstacle
    {
        get { return (obstacleRend.enabled);}
        set
        { 
            obstacleRend.enabled = value;
        }
    }
    /// <summary>
    /// If object exists at the very beginning of the scene, initialize it as an already placed object
    /// </summary>
    public bool startObject = false;
    public bool placementObstacle = false;
    private void Awake()
    {
        obstacleRend = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        
        showObstacle = false;
        if (startObject)
        {
            PlaceObstacle();
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) // TODO: Remove in build
        {
            showObstacle = !showObstacle;
        }
        
    }

    public void RemoveObstacle()
    {
        if (!placementObstacle)
            FoodieSystem.inst.pathfinding.obstaclePositions.Remove(transform.position);
        int x = 0;
        int y = 0;
        // sets position on pathfinding grid as walkable
        FoodieSystem.inst.pathfinding.GetGrid().GetXY(transform.position, out x, out y);
        FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsWalkable(true);
        FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsPlaceable(true);
        

        // for debugging
        if (FoodieSystem.inst.showDebug)
            FoodieSystem.inst.pathfinding.GetGrid().GetDebugTextArray()[x, y].color = Color.red;
    }



    public void PlaceObstacle()
    {
        if (!placementObstacle)
            FoodieSystem.inst.pathfinding.obstaclePositions.Add(transform.position);
        int x = 0;
        int y = 0;
        // sets position on pathfinding grid as unwalkable
        FoodieSystem.inst.pathfinding.GetGrid().GetXY(transform.position, out x, out y);
        FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsWalkable(false);
        FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsPlaceable(false);
        

        // for debugging
        if (FoodieSystem.inst.showDebug)
            FoodieSystem.inst.pathfinding.GetGrid().GetDebugTextArray()[x, y].color = Color.red;
    }
}
