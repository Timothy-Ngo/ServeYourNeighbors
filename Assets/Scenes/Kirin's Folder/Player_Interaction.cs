// Kirin Hardinger
// October 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_Interaction : MonoBehaviour {
    GameObject interactionMessage;
    TMP_Text messageText;

    bool cooktopRange = false;
    bool cooking = false;
    int cookTime = 5; // time in seconds
    GameObject dish;

    Timer timer_script;

    private void Start() {
        interactionMessage = GameObject.Find("InteractionPrompt");
        messageText = interactionMessage.GetComponent<TextMeshProUGUI>();
        interactionMessage.SetActive(false);

        dish = GameObject.Find("cook_station/dish");

        timer_script = FindObjectOfType<Timer>();
    }

    private void Update() {
        // use TakeAction function to display a prompt and await user interaction
        if (cooktopRange && !cooking) {
            if(TakeAction("[C] Cook", KeyCode.C)) {
                cooking = true;
                interactionMessage.SetActive(false);
                StartCoroutine(CookWaiter(cookTime));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // to create new interactions, add a tag to the collision object in question
        if(collision.tag == "Cooktop" && !cooking) {
            Debug.Log("Within range of cooktop");
            cooktopRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.tag == "Cooktop") {
            cooktopRange = false;
            Debug.Log("Out of range of cooktop");
        }

        interactionMessage.SetActive(false);
    }

    // displays a given prompt and awaits user interaction
    private bool TakeAction(string prompt, KeyCode action_keycode) {
        interactionMessage.SetActive(true);
        messageText.SetText(prompt);
        if(Input.GetKeyDown(action_keycode)) {
            return true;
        } else {
            return false;
        }
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
