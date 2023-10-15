using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodieState
{
    protected Foodie foodie;
    protected FoodieStateMachine foodieStateMachine;
    

    public FoodieState(Foodie foodie, FoodieStateMachine foodieStateMachine)
    {
        this.foodie = foodie;
        this.foodieStateMachine = foodieStateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void Update() { }
    public virtual void AnimationTriggerEvent() { }

}
