using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodieKidnappedState : FoodieState
{
    bool stoppedMoving = false;
    public FoodieKidnappedState(Foodie foodie, FoodieStateMachine foodieStateMachine) : base(foodie, foodieStateMachine)
    {
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
        

        foodie.foodieSight.SetActive(false);
        foodie.kidnapCollider.enabled = false;

        foodie.GetComponent<BoxCollider2D>().enabled = false;
        if (!stoppedMoving)
        {

            foodie.foodieMovement.StopMoving();
            stoppedMoving = true;
        }
    }
}
