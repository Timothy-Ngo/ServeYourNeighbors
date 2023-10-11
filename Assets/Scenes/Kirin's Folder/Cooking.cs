// Kirin Hardinger
// October 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooking : MonoBehaviour {
    GameObject dish;

    Timer timer_script;
    bool cooking = false;
    int cookTime = 5; // time in seconds

    void Start() {
        dish = GameObject.Find("dish");

        timer_script = FindObjectOfType<Timer>();
    }

    void Update() {
        
    }

    public void StartCooking() {
        cooking = true;
        StartCoroutine(CookWaiter(cookTime));
    }

    public bool IsCooking() {
        return cooking;
    }

    // https://stackoverflow.com/questions/30056471/how-to-make-the-script-wait-sleep-in-a-simple-way-in-unity
    IEnumerator CookWaiter(int sec) {
        timer_script.SetMaxTime(sec);
        yield return new WaitForSeconds(sec);
        dish.GetComponent<Food>().SetDish("tomatoSoup"); // https://forum.unity.com/threads/calling-function-from-other-scripts-c.57072/
        Debug.Log("You have cooked something!");
        cooking = false;
    }
}
