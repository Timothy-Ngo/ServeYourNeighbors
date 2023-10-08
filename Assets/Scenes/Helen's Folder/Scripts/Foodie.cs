using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Foodie : MonoBehaviour
{
    FoodieMovement foodieMovement;
    bool atFrontOfLine = false;
    public Vector3 seat;
    bool inSeat = false;

    Canvas orderBubble;

    // Start is called before the first frame update
    void Start()
    {
        foodieMovement = GetComponent<FoodieMovement>();
        orderBubble = GetComponentInChildren<Canvas>();
        orderBubble.enabled = false;

        EnterLine();

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Line Count: " + FoodieSystem.inst.line.Count);

        if (Input.GetKeyDown(KeyCode.H))
            GoToSeat();

        if (Input.GetKeyDown(KeyCode.J))
            OrderFood();

        if (Input.GetKeyDown(KeyCode.K))
            LeaveSeat();
    }


    private void EnterLine()
    {
        Vector3 targetPosition;


        // if line is empty
        if (FoodieSystem.inst.line.Count == 0)
        {
            // move to the front of the line
            atFrontOfLine = true;
            targetPosition = FoodieSystem.inst.lineStartPosition;
        }
        else
        {
            // move to behind the last person in line
            Vector3 lastInLinePosition = FoodieSystem.inst.lastInLine.transform.position;
            targetPosition = new Vector3(lastInLinePosition.x + 1, lastInLinePosition.y);
        }

        FoodieSystem.inst.lastInLine = foodieMovement;
        FoodieSystem.inst.line.Enqueue(foodieMovement);
        foodieMovement.SetTargetPosition(targetPosition, FoodieSystem.inst.pathfinding); // moves foodie
    }

    // moves all foodies in line up in line
    private void MoveUpInLine()
    {
        if (FoodieSystem.inst.line.Contains(foodieMovement))
        {
            Vector3 moveUpPosition = new Vector3(transform.position.x - 1, transform.position.y);
            foodieMovement.SetTargetPosition(moveUpPosition, FoodieSystem.inst.pathfinding);

            if (moveUpPosition.x == FoodieSystem.inst.lineStartPosition.x)
                atFrontOfLine = true;
        }
    }



    private void GoToSeat()
    {
        // checks if there are available seats and if foodie is at the front of the line
        if (FoodieSystem.inst.availableSeats.Count > 0 && atFrontOfLine)
        {
            seat = FoodieSystem.inst.availableSeats.Dequeue(); // gets seat from available seats
            foodieMovement.SetTargetPosition(seat, FoodieSystem.inst.pathfinding); // moves to seat
            
            FoodieSystem.inst.line.Dequeue(); // removes foodie from line
            atFrontOfLine = false;
            
            inSeat = true;

        }

        // BUG: lineUpdated isn't working -- foodies move anyway
        // prevents foodies in line from moving if the line has already moved
        if (!FoodieSystem.inst.lineUpdated)
        {
            MoveUpInLine(); // moves foodies up in line
            FoodieSystem.inst.lineUpdated = true; // foodies have moved, so do not move again
        }

        FoodieSystem.inst.lineUpdated = false; // resets lineUpdated

    }

    private void LeaveSeat()
    {
        // checks if foodie is seated
        if (inSeat)
        {
            orderBubble.enabled = false;
            inSeat = false; // removes foodie from seat
            FoodieSystem.inst.availableSeats.Enqueue(seat); // seat is available
            foodieMovement.SetTargetPosition(Vector3.zero, FoodieSystem.inst.pathfinding); // moves foodie out of restaurant


            //Destroy(gameObject); // destroying has to be on some kind of coroutine --> destroys instantly right now
        }
    }

    private void OrderFood()
    {
        if (inSeat)
        {
            orderBubble.enabled = true;
            Debug.Log("Ordered Food");
        }
    }

    // ---------------------------------------  IGNORE  --------------------------------------------

    // not currently in use -- keeping as skeleton code for if foodies can leave line out of impatience
    private void LeaveLine()
    {
        if (atFrontOfLine && FoodieSystem.inst.line.Count != 0)
        {
            FoodieSystem.inst.line.Dequeue();
            //Destroy(gameObject);
        }

        MoveUpInLine();
    }

}
