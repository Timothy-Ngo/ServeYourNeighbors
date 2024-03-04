using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodieKidnappedState : FoodieState
{
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


        //foodie.foodieMovement.enabled = false;
        //foodie.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        foodie.GetComponent<BoxCollider2D>().enabled = false;
        foodie.foodieMovement.StopMoving();

        // NOTE: This is probably not the best way to go about moving the kidnapped foodie along with the player. 
        // I did it this way because the foodie wasn't moving with the player when the player kidnapped them.
        Vector3 offset = new Vector3(0,1,0);
        //foodie.transform.position = Player.inst.transform.position + offset;
    }
}
