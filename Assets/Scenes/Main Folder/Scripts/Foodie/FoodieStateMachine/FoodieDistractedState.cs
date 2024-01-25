// Author: Helen Truong
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
        if (!isDistracted && DistractionSystem.inst.animatronicDistraction.distractionTrigger.enabled)
        {
            Debug.Log("is distracted");
            isDistracted = true;

            // pause foodie's timer
            foodie.timerScript.paused = true;
            
            // save foodie's state
            pausedState = foodie.stateMachine.previousFoodieState;

            // show distracted text
            foodie.distractedText.enabled = true;
            foodie.distractedText.text = "Distracted";

            // turn off foodie sight
            foodie.foodieSight.SetActive(false);

            // start distraction timer
            foodie.distractionTimerScript.SetMaxTime(distractedTime);
            //foodie.foodieMovement.StopMoving(); // messes with foodie movement
        }
        if (foodie.distractionTimerScript.timeLeft <= 0 && isDistracted)
        {
            Debug.Log("Distraction ended");
            isDistracted = false;
            foodie.distractedText.enabled = false; // disable distracted text

            // resume foodies task before being distracted
            foodie.timerScript.paused = false;

            // turn foodie's sight back on
            foodie.foodieSight.SetActive(true);

            // turns distraction off when distraction duration is finished
            //DistractionSystem.inst.ResetDistraction();

            foodie.stateMachine.ChangeState(pausedState);
            
        }
    }
}
