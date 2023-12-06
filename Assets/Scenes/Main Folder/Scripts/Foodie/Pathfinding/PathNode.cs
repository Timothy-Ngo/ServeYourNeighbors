using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PathNode
{
    // constructer variables
    public int x;
    public int y;

    // pathfinding variables
    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable = true;
    public bool isPlaceable = true;

    public PathNode cameFromNode; // reference to previous node
    public PathNode(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
        //grid.TriggerGridObjectChanged(x, y);
    }

    public void SetIsPlaceable(bool isPlaceable)
    {
        this.isPlaceable = isPlaceable;
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}
