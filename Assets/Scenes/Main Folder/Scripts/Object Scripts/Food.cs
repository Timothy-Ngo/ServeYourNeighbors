// Kirin Hardinger
// October 2023
// Adapted from https://www.youtube.com/watch?v=BLEjkIGrkLM

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour {
    private SpriteRenderer sr;
    public string dishName = "";
    public int value;
    public bool hasMSG;

    // pre-load sprites in inspector
    public Sprite tomatoSoup;

    // pre-load sprites in inspector
    public Sprite tomatoIngredient;

    public GameObject tomatoSoupPrefab;
    public GameObject tomatoIngredientPrefab;

    public Sprite salad;
    public Sprite lettuceIngredient;
    public GameObject saladPrefab;
    public GameObject lettuceIngredientPrefab;

    public Sprite garlicBread;
    public Sprite flourIngredient;
    public GameObject garlicBreadPrefab;
    public GameObject flourIngredientPrefab;

    private void Start() {
        //sr = gameObject.GetComponent<SpriteRenderer>();
        //sr.enabled = false;
        if(!(gameObject.GetComponent<Animator>() is null))
        {
            gameObject.GetComponent<Animator>().Play("Idle");
        }
    }

    public void SetDish(string name) {
        //Debug.Log("New sprite!");
        dishName = name;
        if (name == "tomatoSoup") {
            sr.sprite = tomatoSoup;
        } else {
            Debug.Log("INVALID DISH NAME");
        }
        
        Display();
    }

    public void SetDish(Sprite ingredient)
    {
        //Debug.Log("New sprite!");

        if (ingredient == tomatoIngredient)
        {
            sr.sprite = tomatoSoup;
        }
        else if (ingredient == lettuceIngredient) 
        {
            sr.sprite = salad;
        }
        else if (ingredient == flourIngredient)
        {
            sr.sprite = garlicBread;
        }
        else
        {
            Debug.Log("INVALID DISH SPRITE");
        }

        Display();
    }

    

    public void Display() {
        sr.enabled = true;
    }

    public void AddMSG()
    {
        hasMSG = true;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.9804f, 0.6863f, 1f);
        Debug.Log("Added MSG");
    }
    
    public void EatAnimation()
    {
        gameObject.GetComponent<Animator>().Play("Eating");
    }
}
