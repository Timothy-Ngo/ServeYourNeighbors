using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

// tutorial: https://youtu.be/alU04hvz6L4?si=Uak7a_jyl9cQwB0X
public class Pathfinding
{
  
    private const int MOVE_STRAIGHT_COST = 10; // 1 times 10
    private const int MOVE_DIAGONAL_COST = 14; // sqroot(2) = 1.4 -> times 10

    private Grid<PathNode> grid;
    private List<PathNode> openList; // used for queueing nodes to search
    private HashSet<PathNode> closedList; // used to store already searched nodes
    public Pathfinding(int width, int height, Vector3 startPoint, float cellSize)
    {
        grid = new Grid<PathNode>(width, height, cellSize, startPoint, (int x, int y) => new PathNode(x, y));
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * 0.5f);
            }
            return vectorPath;
        }
    }
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        Debug.Log($"Find Path Called");
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        openList = new List<PathNode> { startNode };
        closedList = new HashSet<PathNode>();

        // cycle through grid -- resets path
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue; // sets gCost to infinity
                pathNode.CalculateFCost(); 
                pathNode.cameFromNode = null; // set to null so won't retain the information from a previous path

            }
        }

        // set costs of startNode
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0) // while there are still nodes to search
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            
            // if reached final node
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // cycle through each neighbor node
            foreach (PathNode neighborNode in GetNeighborList(currentNode))
            {
                if (closedList.Contains(neighborNode)) continue; // skips nodes already visited
                if (!neighborNode.isWalkable)
                {
                    closedList.Add(neighborNode); 
                    continue;
                }

                // tentativeGCost = current gCost + movement cost from currentNode to neighborNode
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighborNode);

                // if true --> there exists a faster path from currentNode to the neighborNode
                if (tentativeGCost < neighborNode.gCost)
                {
                    neighborNode.cameFromNode = currentNode;
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateDistanceCost(neighborNode, endNode);
                    neighborNode.CalculateFCost();

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }

        }

        // out of nodes on the openList
        // if reached here, then searched whole grid and could not find path
        Debug.Log("Path could not be found");

        return null;
    }

    // checks the eight spaces surrounding a node -- adds existing spaces to a list
    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighborList = new List<PathNode>();

        // if column to the left exists
        if (currentNode.x - 1 >= 0)
        {
            // left
            neighborList.Add(GetNode(currentNode.x - 1, currentNode.y));

            // if row below exists - // down left
            if (currentNode.y - 1 >= 0)
                neighborList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));

            // if row above exists - // up left
            if (currentNode.y + 1 < grid.GetHeight())
                neighborList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }

        // if column to the right exists
        if (currentNode.x + 1 < grid.GetWidth())
        {
            // right
            neighborList.Add(GetNode(currentNode.x + 1, currentNode.y));

            // if row below exists - // down right
            if (currentNode.y - 1 >= 0)
                neighborList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));

            // if row above exists // up right
            if (currentNode.y + 1 < grid.GetHeight())
                neighborList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }

        // if node that is below current node exists - // down
        if (currentNode.y - 1 >= 0) 
            neighborList.Add(GetNode(currentNode.x, currentNode.y - 1));

        // if node that is above current node exists - // up
        if (currentNode.y + 1 < grid.GetHeight())
            neighborList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighborList;
    }

    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    public bool IsWalkable(Vector3 worldPosition)
    {
        return grid.GetGridObject(worldPosition).isWalkable;
    }

    public bool IsPlaceable(Vector3 worldPosition)
    {
        return grid.GetGridObject(worldPosition).isPlaceable;
    }
    public Grid<PathNode> GetGrid()
    {
        return grid;
    }

    

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;

        // while the currentNode has a parent
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode); // add the parent to the path
            currentNode = currentNode.cameFromNode; // currentNode is now the parent
        }

        path.Reverse(); // reverses path so path goes from startNode to endNode
        return path;
    }

    // calculates the distance cost (h) between nodes 
    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        // total diagonal distance + total straight distance
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining; 
    }

    // cycles through list of pathnodes and finds which node has the lowest f cost
    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];

        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }

        // returns node with lowest f cost
        return lowestFCostNode;
    }
    
}
