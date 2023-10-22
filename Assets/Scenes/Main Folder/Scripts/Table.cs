using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public GameObject dish;
    public SpriteRenderer dishSR;

    public Foodie foodie;
    // Start is called before the first frame update
    void Start()
    {
        dish.SetActive(false);
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
        Debug.Log(PickupSystem.inst.GetItem());
        dishSR.sprite = PickupSystem.inst.GetItem();
        Debug.Log("Set Dish called");
        PickupSystem.inst.DropItem();
    }

    public void RemoveDish()
    {
        dish.SetActive(false);
        CustomerPayments.inst.RandomPayment();
    }
    
    
    
    
    
}
