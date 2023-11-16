using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodieEatState : FoodieState
{
    bool eating;
    int eatingTime = 2;
    
    public FoodieEatState(Foodie foodie, FoodieStateMachine foodieStateMachine) : base(foodie, foodieStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();
        
        if (!foodie.table.testing)
            foodie.table.SetDish();
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Assert(foodie.table != null);
        foodie.table.RemoveDish();
       

    }

    public override void Update()
    {
        base.Update();

        //Debug.Log("eating");

        // when done eating --> foodie leave
        if (foodie.timerScript.timeLeft <= 0 && eating)
        {

            FoodieSystem.inst.availableSeats.Enqueue(foodie.orderState.tablePosition);
            foodie.stateMachine.ChangeState(foodie.leaveState);
        }

        // starts eating
        if (!eating)
        {
            eating = true;
            foodie.timerScript.SetMaxTime(eatingTime);
        }
    }
    
    
}
