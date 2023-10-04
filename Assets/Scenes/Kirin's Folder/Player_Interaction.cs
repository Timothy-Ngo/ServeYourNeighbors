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

    private void Start() {
        interactionMessage = GameObject.Find("InteractionPrompt");
        messageText = interactionMessage.GetComponent<TextMeshProUGUI>();
        interactionMessage.SetActive(false);
    }

    private void Update() {
        // use TakeAction function to display a prompt and await user interaction
        if (cooktopRange) {
            if(TakeAction("[C] Cook", KeyCode.C)) {
                Debug.Log("You have cooked something!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // to create new interactions, add a tag to the collision object in question
        if(collision.tag == "Cooktop") {
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
}
