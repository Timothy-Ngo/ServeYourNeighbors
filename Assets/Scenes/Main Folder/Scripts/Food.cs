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

    private void Start() {
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.enabled = false;
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

    public void ResetDish()
    {
        sr.sprite = null;
    }

    public void Display() {
        sr.enabled = true;
    }
}
