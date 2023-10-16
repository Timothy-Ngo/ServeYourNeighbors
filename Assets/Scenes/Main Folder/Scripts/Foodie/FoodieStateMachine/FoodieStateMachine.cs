using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// tutorial for FSM: https://youtu.be/RQd44qSaqww?si=udxdALY2aHQrMB00
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
        previousFoodieState = currentFoodieState;
        currentFoodieState.ExitState();
        currentFoodieState = newState;
        currentFoodieState.EnterState();
    }
}
