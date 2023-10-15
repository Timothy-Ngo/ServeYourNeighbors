using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FoodieLeaveState : FoodieState
{
    bool atDespawnPoint = false;
    public FoodieLeaveState(Foodie foodie, FoodieStateMachine foodieStateMachine) : base(foodie, foodieStateMachine)
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

        if (!atDespawnPoint)
        {
            //Debug.Log("leaving");
            foodie.foodieMovement.SetTargetPosition(FoodieSystem.inst.despawnPoint, FoodieSystem.inst.pathfinding);
            atDespawnPoint = AtDespawnPoint();
            
        }
        else
        {
            foodie.DestroyFoodie();
        }

    }

    private bool AtDespawnPoint()
    {
        return foodie.transform.position.x == FoodieSystem.inst.despawnPoint.x && foodie.transform.position.y == FoodieSystem.inst.despawnPoint.y;
    }

}
