using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// tutorial for FSM: https://youtu.be/RQd44qSaqww?si=udxdALY2aHQrMB00
public class FoodieStateMachine
{
    public FoodieState currentEnemyState { get; set; }
    
    public void Initialize(FoodieState startingState)
    {
        currentEnemyState = startingState;
        currentEnemyState.EnterState();
    }

    public void ChangeState(FoodieState newState)
    {
        currentEnemyState.ExitState();
        currentEnemyState = newState;
        currentEnemyState.EnterState();
    }
}
