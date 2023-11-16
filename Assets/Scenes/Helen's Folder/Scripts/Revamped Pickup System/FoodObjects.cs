// Kirin Hardinger
// October 2023
// Adapted from https://www.youtube.com/watch?v=BLEjkIGrkLM

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodObjects : MonoBehaviour {
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

    private void Start() {
        //sr = gameObject.GetComponent<SpriteRenderer>();
        //sr.enabled = false;
    }

    public void SetDish(string name) {
        Debug.Log("New sprite!");
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
        Debug.Log("New sprite!");

        if (ingredient == tomatoIngredient)
        {
            sr.sprite = tomatoSoup;
        }
        else
        {
            Debug.Log("INVALID DISH SPRITE");
        }

        Display();
    }

    public GameObject SetDish(Sprite ingredient, Transform cookingStation)
    {
        Debug.Log("New sprite!");

        GameObject finishedDish = gameObject;

        if (ingredient == tomatoIngredient)
        {
             finishedDish = SpawnDish(tomatoSoupPrefab, cookingStation);
        }
        else
        {
            Debug.Log("INVALID INGREDIENT");
        }

        //Display();
        return finishedDish;
    }

    public GameObject SpawnDish(GameObject dishPrefab, Transform cookingStation)
    {
        Vector3 offset = new Vector3(0.015f, 0.255f, 0);
        Vector3 spawnPosition = cookingStation.position + offset;
        GameObject dish = Instantiate(dishPrefab, spawnPosition, Quaternion.identity, cookingStation);

        return dish;
    }
 

    public void ResetDish()
    {
        hasMSG = false;
        //sr.sprite = null;
    }

    public void Display() {
        sr.enabled = true;
    }

    public void AddMSG()
    {
        hasMSG = true;
        Debug.Log("Added MSG");
    }
    
}
