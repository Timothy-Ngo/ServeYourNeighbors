using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public GameObject dish;
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
        Debug.Log("Set Dish called");
    }

    public void RemoveDish()
    {
        dish.SetActive(false);
        CustomerPayments.inst.RandomPayment();
    }
    
    
    
    
    
}
