// Kirin Hardinger
// October 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooking : MonoBehaviour {
    [Header("-----SPRITES-----")]
    private SpriteRenderer sr;
    public Sprite std;
    public Sprite fire;

    [Header("-----GAME OBJECTS-----")]
    public GameObject dish;
    public GameObject dishDefault;
    private GameObject ingredient;
    private Sprite ingredientSprite;
    [SerializeField] Timer timer_script;
    [SerializeField] QT_Event qt_script;

    [Header("-----STATE-----")]
    bool prepping = false;
    bool cooking = false;
    bool foodReady = false;
    int cookTime = 5; // time in seconds, can be updated by upgrade system

    void Start() {
        sr = gameObject.GetComponent<SpriteRenderer>();

        //dish = GameObject.Find("dish");
        //match = GameObject.Find("Match");

        //timer_script = FindObjectOfType<Timer>();
    }

    private void Update() {
        if (prepping && qt_script.isComplete()) {
            prepping = false;
            StartCooking();
        }
    }

    public void SetIngredient(GameObject ingredient)
    {
        this.ingredient = ingredient;
        ingredientSprite = ingredient.GetComponent<SpriteRenderer>().sprite;
    }

    public void SetCookTime(int newTime) {
        cookTime = newTime;
    }

    public void StartPrep() {
        //Debug.Log("Starting prep");
        prepping = true;
        qt_script.resetEvent();
    }

    public void StartCooking() {
        prepping = false;
        cooking = true;
        sr.sprite = fire;
        StartCoroutine(CookWaiter(cookTime)); // when ordering system is combined, this will be a variable passed in to StartCooking() from the oddering system
    }

    public bool IsCooking() {
        return cooking;
    }

    public bool IsPrepping() {
        return prepping;
    }

    public bool IsFoodReady() {
        return foodReady;
    }

    public void ResetCooktop() {
        dish = dishDefault;
        //dish.GetComponent<Food>().ResetDish();
        foodReady = false;
    }

    // https://stackoverflow.com/questions/30056471/how-to-make-the-script-wait-sleep-in-a-simple-way-in-unity
    IEnumerator CookWaiter(int sec) {
        timer_script.SetMaxTime(sec);
        yield return new WaitForSeconds(sec);
        sr.sprite = std;
        dish = dish.GetComponent<Food>().SetDish(ingredientSprite, gameObject.transform); // https://forum.unity.com/threads/calling-function-from-other-scripts-c.57072/
        //Debug.Log("You have cooked something!");
        cooking = false;
        foodReady = true;
    }
}