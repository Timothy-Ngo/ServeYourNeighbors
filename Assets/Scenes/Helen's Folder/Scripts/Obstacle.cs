using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    bool isWalkable = false;

    public Vector3 position = Vector3.zero;

    void Update()
    {
        position = transform.position;
        int x = 0;
        int y = 0;
        // sets position on pathfinding grid as unwalkable
        FoodieSystem.inst.pathfinding.GetGrid().GetXY(transform.position, out x, out y);
        
        FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsWalkable(isWalkable);
        
        // for debugging
        if (FoodieSystem.inst.showDebug)
            FoodieSystem.inst.pathfinding.GetGrid().GetDebugTextArray()[x, y].color = Color.red;
    }
}
