using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodieMovement : MonoBehaviour
{
    public static FoodieMovement inst;
    private void Awake()
    {
        inst = this;
    }

    public float speed = 20f;
    public Transform foodie;

    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    void Start()
    {
        Transform bodyTransform = transform.Find("Body");
    }
    
    void Update()
    {
        
        HandleMovement();
        /*
        if (Input.GetMouseButtonDown(0))
        {
            SetTargetPosition(Utils.GetMouseWorldPosition());
        }
        */
    }

    // moves one space
    private void HandleMovement()
    {
        // if path exists
        if (pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];

            // if targetPosition is far enough 
            if (Vector3.Distance(foodie.transform.position, targetPosition) > 0.5f)
            {
                // moves towards targetPosition
                Vector3 moveDir = (targetPosition - foodie.transform.position).normalized;
                foodie.transform.position = foodie.transform.position + moveDir * speed * Time.deltaTime;
            }
            else // if close to targetPosition
            {
                // move onto next target space
                currentPathIndex++;
                if(currentPathIndex >= pathVectorList.Count)
                {
                    foodie.transform.position = targetPosition; // snaps foodie to targetPosition
                    StopMoving();
                    
                }
            }
        }
    }

    public void StopMoving()
    {
        pathVectorList = null;
    }

    public Vector3 GetPosition()
    {
        return foodie.transform.position;
    }

    public void SetTargetPosition(Vector3 targetPosition, Pathfinding pathfinding)
    {
        Debug.Log(targetPosition);
        currentPathIndex = 0;
        Debug.Log(pathVectorList);
        Debug.Log(GetPosition());
        //List<Vector3> testPath = pathfinding.FindPath(GetPosition(), targetPosition);
        //Debug.Log("testPathLength: " + testPath.Count);

        pathVectorList = pathfinding.FindPath(GetPosition(), targetPosition);
        Debug.Log("pathVectorList: " + pathVectorList);
        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        } 
    }
}
