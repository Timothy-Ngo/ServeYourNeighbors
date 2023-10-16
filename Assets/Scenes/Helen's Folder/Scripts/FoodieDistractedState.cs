using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FoodieDistractedState : FoodieState
{
    FoodieState pausedState;
    bool isDistracted;
    int distractedTime;


    public FoodieDistractedState(Foodie foodie, FoodieStateMachine foodieStateMachine) : base(foodie, foodieStateMachine)
    {
        distractedTime = foodie.distractedTime;
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

        // if they're not distracted
        if (!isDistracted)
        {
            isDistracted = true;

            // pause foodie's timer
            foodie.timerScript.paused = true;
            
            // save foodie's state
            pausedState = foodie.stateMachine.previousFoodieState;

            // show distracted text
            foodie.distractedText.enabled = true;
            foodie.distractedText.text = "Distracted";

            // start distraction timer
            foodie.distractionTimerScript.SetMaxTime(distractedTime);
            //foodie.foodieMovement.StopMoving(); // messes with foodie movement
        }
        else if (foodie.distractionTimerScript.timeLeft <= 0 && isDistracted)
        {
            isDistracted = false;
            foodie.distractedText.enabled = false; // disable distracted text

            // resume foodies task before being distracted
            foodie.timerScript.paused = false;

            // turns distraction off when distraction duration is finished
            DistractionSystem.inst.animatronicDistraction.distractionTrigger.enabled = false;
            DistractionSystem.inst.animatronicDistraction.statusText.text = "OFF";
            DistractionSystem.inst.animatronicDistraction.statusText.enabled = false;

            foodie.stateMachine.ChangeState(pausedState);
            
        }
    }
}
