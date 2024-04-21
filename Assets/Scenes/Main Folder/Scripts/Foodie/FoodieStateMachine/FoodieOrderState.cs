// Author: Helen Truong
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FoodieOrderState : FoodieState
{
    public Vector3 tablePosition;
    public Table tableScript;
    public bool atTable = false;
    float orderTime = 5;
    bool isOrdering = false;
    
    
    public bool orderGiven = false;

    bool eating = false;
    bool leaving = false;

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

        

        // order given --> goes into eating
        if (isOrdering && foodie.timerScript.timeLeft > 0 && orderGiven)
        {
            foodie.orderBubble.SetActive(false);
            foodie.timeAtOrderTaken = foodie.timerScript.timeLeft;
            foodie.timerScript.timeLeft = 0;

            if (eating)
            {
                // switch to eating state
                foodie.stateMachine.ChangeState(foodie.eatState);
            }
            else if (leaving)
            {
                // switch to leaving state -- and put table back into available tables
                FoodieSystem.inst.availableSeats.Enqueue(tablePosition);
                foodie.stateMachine.ChangeState(foodie.leaveState);
            }
        }

        // if their order isn't taken in time then they leave
        if (isOrdering && foodie.timerScript.timeLeft <= 0 && !orderGiven)
        {

            foodie.orderBubble.SetActive(false);
            FoodieSystem.inst.availableSeats.Enqueue(tablePosition);

            foodie.gameObject.GetComponent<Animator>().Play("Angry Leave");
            foodie.stateMachine.ChangeState(foodie.leaveState);
        }

        // puts foodie at a table
        if (!AtTable() && !atTable) 
        {
            //Debug.Log("at table");
            atTable = true;

            // finds available table from tables            
            tablePosition = FoodieSystem.inst.availableSeats.Dequeue();

            // moves foodie to table
            //Debug.Log(tablePosition);
            //Debug.Log("OrderState: SetTargetPosition");
            foodie.foodieMovement.SetTargetPosition(tablePosition, FoodieSystem.inst.pathfinding);
            foodie.tablePosition = tablePosition;
            
            GetFoodieTableScript();

        }

        //Debug.Log("attable: " + AtTable());
        // if foodie is at the table and hasn't ordered yet
        if (AtTable() && !isOrdering)
        {
            if(!foodie.isTutorial)
            {
                isOrdering = true;
                //Debug.Log("in FoodieOrderState");
                // set a timer for their order and they order
                foodie.orderBubble.SetActive(true);
                foodie.timerScript.SetMaxTime(orderTime);

                // makes sure the foodie is facing right when seated at a table
                if (!foodie.foodieMovement.facingRight)
                {
                    foodie.foodieMovement.Flip();
                    foodie.foodieMovement.facingRight = true;
                }
            }
        }

        

    }

    // called in PlayerInteraction.cs when player gives foodie the correct order
    public void ReceivedOrder()
    {
        if (atTable)
        {
            orderGiven = true;
            eating = true;
        }
    }

    // called in PlayerInteraction.cs when player gives foodie the wrong order
    public void ReceivedWrongOrder()
    {
        if (atTable)
        {
            orderGiven = true;
            leaving = true;
        }
    }

    public void GetFoodieTableScript()
    {
        foreach (Table table in FoodieSystem.inst.tables) // Finds table script that foodie is at
        {
            
            if (Mathf.Approximately(foodie.tablePosition.x, table.gameObject.transform.position.x) &&
                Mathf.Approximately(foodie.tablePosition.y, table.gameObject.transform.position.y))
            {
                //Debug.Log("Table initialize");
                foodie.table = table;
                foodie.tablePosition = table.gameObject.transform.position;
                foodie.table.SetFoodie(foodie);
                break;
            }
        }
    }
    

    public bool AtTable()
    {
        return Mathf.Approximately(tablePosition.x,foodie.transform.position.x) &&
                Mathf.Approximately(tablePosition.y, foodie.transform.position.y);
    }
}
