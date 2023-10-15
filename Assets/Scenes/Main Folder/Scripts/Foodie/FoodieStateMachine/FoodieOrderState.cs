using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodieOrderState : FoodieState
{
    public Vector3 table;
    bool atTable = false;
    int orderTime = 5;
    bool isOrdering = false;
    
    public bool orderGiven = false;

    public FoodieOrderState(Foodie foodie, FoodieStateMachine foodieStateMachine) : base(foodie, foodieStateMachine)
    {
        orderTime = foodie.orderTime;
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void Update()
    {
        base.Update();

        // PLACEHOLDER: takes order
        if (Input.GetKeyDown(KeyCode.F))
        {
            orderGiven = true;
            Debug.Log("key press");
        }

        // order given --> goes into eating
        if (isOrdering && foodie.timerScript.timeLeft > 0 && orderGiven)
        {
            foodie.orderBubble.SetActive(false);
            foodie.timerScript.timeLeft = 0;
            foodie.stateMachine.ChangeState(foodie.eatState);
        }

        // if their order isn't taken in time then they leave
        if (isOrdering && foodie.timerScript.timeLeft <= 0 && !orderGiven)
        {

            foodie.orderBubble.SetActive(false);
            FoodieSystem.inst.availableSeats.Enqueue(table);

            foodie.stateMachine.ChangeState(foodie.leaveState);
        }

        if (!atTable) // puts foodie at a table
        {
            
            atTable = true;

            // finds available table from tables            
            table = FoodieSystem.inst.availableSeats.Dequeue();

            // moves foodie to table
            foodie.foodieMovement.SetTargetPosition(table, FoodieSystem.inst.pathfinding);
            

        }
        
        // if foodie is at the table and hasn't ordered yet
        if (foodie.transform.position.x == table.x && foodie.transform.position.y == table.y && !isOrdering)
        {

            isOrdering = true;
            //Debug.Log("in FoodieOrderState");

            // set a timer for their order and they order
            foodie.orderBubble.SetActive(true);
            foodie.timerScript.SetMaxTime(orderTime);
            
            
        }

        

    }
}
