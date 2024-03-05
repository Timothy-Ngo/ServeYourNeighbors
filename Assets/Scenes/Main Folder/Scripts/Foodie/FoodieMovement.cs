// Author: Helen Truong
// tutorial: https://youtu.be/alU04hvz6L4?si=Uak7a_jyl9cQwB0X
using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;
public class FoodieMovement : MonoBehaviour
{

    // Pathfinding
    
    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    public float speed = 20f;
    private SpriteRenderer foodieSR;
    private SpriteRenderer sightRangeSR;
    public bool facingRight = true;

    void Start()
    {
        foodieSR = gameObject.GetComponent<SpriteRenderer>();
        sightRangeSR = gameObject.GetComponentInChildren<SpriteRenderer>();
    }
    
    void Update()
    {
        
        HandleMovement();
       
        
    }

    // moves one space
    private void HandleMovement()
    {
        // if path exists
        if (pathVectorList != null)
        {
            FoodieSystem.inst.pathfinding.GetGrid().GetXY(transform.position, out int x, out int y);

            // commented out to try to fix foodies walking through walls
            //FoodieSystem.inst.pathfinding.GetNode(x, y).SetIsWalkable(true);

            Vector3 targetPosition = pathVectorList[currentPathIndex];

            // if targetPosition is far enough 
            if (Vector3.Distance(transform.position, targetPosition) > speed / 40f)
            {
                // moves towards targetPosition
                Vector3 moveDir = (targetPosition - transform.position).normalized;


                

                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }
            else // if close to targetPosition
            {
                // move onto next target space
                currentPathIndex++;
                if(currentPathIndex >= pathVectorList.Count)
                {

                    transform.position = targetPosition; // snaps foodie to targetPosition
                    StopMoving();
                    
                }
            }
        }
    }

    private void Flip()
    {
        transform.Rotate(new Vector3(0, 180, 0));
    }

    public void StopMoving()
    {
        pathVectorList = null;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetTargetPosition(Vector3 targetPosition, Pathfinding pathfinding)
    {
        currentPathIndex = 0;

        // find a path
        pathVectorList = pathfinding.FindPath(GetPosition(), targetPosition);
        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }



        // flips sprite when moving -- modified code from Player_Movement
        if (targetPosition.x <= transform.position.x && facingRight || targetPosition.x > transform.position.x && !facingRight)
        {
            //foodieSR.flipX = false;
            //sightRangeSR.flipX = false;
            Flip();
            facingRight = !facingRight;

            
        }
        else
        {
            //Flip();
            //foodieSR.flipX = true;
            //sightRangeSR.flipX = true;
        }

        
    }

    
}
