using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FoodieLineState : FoodieState
{
    int placeInLine = 0;
    bool inLine = false;
    bool atFrontOfLine;
    bool hasBeenInLine = false;

    public FoodieLineState(Foodie foodie, FoodieStateMachine foodieStateMachine) : base(foodie, foodieStateMachine)
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

        if (FoodieSystem.inst.availableSeats.Count > 0 && atFrontOfLine && foodie.transform.position.x == FoodieSystem.inst.startOfLine.x)
        {
            atFrontOfLine = false;
            FoodieSystem.inst.line.RemoveAt(0);
            
            inLine = false;
            
            foodie.stateMachine.ChangeState(foodie.orderState);
            //Debug.Log("changing state");
        }
        
        if (!inLine && !hasBeenInLine) 
        {
            Debug.Log("Should be called one time");
            hasBeenInLine = true; // put in bc there was a bug with it running all this again bc the foodie was no longer in line in the if statement above

            foodie.orderBubble.SetActive(false);

            inLine = true;
            FoodieSystem.inst.line.Add(foodie.foodieMovement); // adds foodie to line
            //Debug.Log("num of foodies in line: " + FoodieSystem.inst.line.Count);
            // gets foodie's place in line 
            placeInLine = FoodieSystem.inst.line.IndexOf(foodie.foodieMovement);
            if (placeInLine == 0)
                atFrontOfLine = true;


            // finds target position in line based on placeInLine
            Vector3 targetPosition = new Vector3(FoodieSystem.inst.startOfLine.x + placeInLine, FoodieSystem.inst.startOfLine.y);
            //Debug.Log("targetPosition: " + targetPosition);

            // moves foodie 
            //Debug.Log("LineState: SetTargetPosition");
            foodie.foodieMovement.SetTargetPosition(targetPosition, FoodieSystem.inst.pathfinding);
            
            
        }
        else
        {
            
            // foodie moves up in line if there is an available space (if a foodie left the line)
            placeInLine = FoodieSystem.inst.line.IndexOf(foodie.foodieMovement);
            //Debug.Log("placeInLine: " + placeInLine);
            //Debug.Log("targetPosition: " + targetPosition);
            if (!atFrontOfLine)
            {
                Vector3 targetPosition = new Vector3(FoodieSystem.inst.startOfLine.x + placeInLine, FoodieSystem.inst.startOfLine.y);
                //Debug.Log("OrderState2: SetTargetPosition");
                foodie.foodieMovement.SetTargetPosition(targetPosition, FoodieSystem.inst.pathfinding);
            }
            if (placeInLine == 0)
                atFrontOfLine = true;
        }

    }
}
