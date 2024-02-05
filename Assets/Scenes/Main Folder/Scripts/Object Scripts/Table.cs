// Author: Timothy Ngo
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public GameObject dish;
    public SpriteRenderer dishSR;
    public Obstacle obstacleScript; // drag and drop from Table in inspector

    public Foodie foodie;
    // Start is called before the first frame update
    void Start()
    {
        //dish.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool HasDish()
    {
        return dish.activeSelf;
    }
    public void SetFoodie(Foodie newFoodie)
    {
        foodie = newFoodie;
    }

    public Foodie GetFoodie()
    {
        return foodie;
    }
    public void SetDish()
    {
        dish.SetActive(true); // Temp Dish, should be replaced later by getting a specific dish from player
        //dishSR.sprite = PickupSystemObjects.inst.GetItemInHands().GetComponent<SpriteRenderer>().sprite;
        //Debug.Log("Set Dish called");
    }

    public void RemoveDish()
    {
        Debug.Assert(dish.gameObject != null);
        dish.SetActive(false);
            
        if (dish.GetComponent<Food>().hasMSG)
        {
            //Debug.Log("MSGPayment");
            CustomerPayments.inst.MSGPayment();
        }
        else
        {
            //Debug.Log("Timebased    Payment");
            CustomerPayments.inst.TimeBasedPayment(foodie.timeAtOrderTaken / foodie.orderTime);
        }

        Destroy(dish);
    }
    
    
    
    
    
}
