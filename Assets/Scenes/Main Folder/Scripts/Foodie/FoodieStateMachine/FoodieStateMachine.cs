// Author: Helen Truong
// tutorial for FSM: https://youtu.be/RQd44qSaqww?si=udxdALY2aHQrMB00
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FoodieStateMachine
{
    public FoodieState currentFoodieState { get; set; }
    public FoodieState previousFoodieState { get; set; }

    public void Initialize(FoodieState startingState)
    {
        currentFoodieState = startingState;
        currentFoodieState.EnterState();
    }

    public void ChangeState(FoodieState newState)
    {
        Debug.Log(currentFoodieState + ": changing state to :" + newState);

        previousFoodieState = currentFoodieState;
        currentFoodieState.ExitState();
        currentFoodieState = newState;
        currentFoodieState.EnterState();
    }
}
