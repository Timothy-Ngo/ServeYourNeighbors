// Kirin Hardinger
// October 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooking : MonoBehaviour {
    private SpriteRenderer sr;
    public Sprite std;
    public Sprite fire;

    GameObject dish;
    GameObject match;
    Timer timer_script;

    bool prepping = false;
    bool cooking = false;
    int cookTime = 5; // time in seconds, can be updated by upgrade system

    void Start() {
        sr = gameObject.GetComponent<SpriteRenderer>();

        dish = GameObject.Find("dish");
        match = GameObject.Find("Match");

        timer_script = FindObjectOfType<Timer>();
    }

    public void SetCookTime(int newTime) {
        cookTime = newTime;
    }

    public void StartPrep() {
        Debug.Log("Starting prep");
        prepping = true;
        match.GetComponent<Match>().SetActive(true);
    }

    public void StartCooking() {
        prepping = false;
        match.GetComponent<Match>().SetActive(false);
        cooking = true;
        sr.sprite = fire;
        StartCoroutine(CookWaiter(cookTime, "tomatoSoup")); // when ordering system is combined, this will be a variable passed in to StartCooking() from the oddering system
    }

    public bool IsCooking() {
        return cooking;
    }

    public bool IsPrepping() {
        return prepping;
    }

    public void OnMouseOver() {
        if(Input.GetMouseButtonDown(0) && prepping) {
            Debug.Log("Clicked and prepped!");
            StartCooking();
        }
    }

    // https://stackoverflow.com/questions/30056471/how-to-make-the-script-wait-sleep-in-a-simple-way-in-unity
    IEnumerator CookWaiter(int sec, string dishName) {
        timer_script.SetMaxTime(sec);
        yield return new WaitForSeconds(sec);
        sr.sprite = std;
        dish.GetComponent<Food>().SetDish(dishName); // https://forum.unity.com/threads/calling-function-from-other-scripts-c.57072/
        Debug.Log("You have cooked something!");
        cooking = false;
    }
}
