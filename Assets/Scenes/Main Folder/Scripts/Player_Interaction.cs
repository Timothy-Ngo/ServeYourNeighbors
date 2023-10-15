// Kirin Hardinger
// October 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_Interaction : MonoBehaviour {
    GameObject interactionMessage;
    TMP_Text messageText;

    // this should become an array in the future for multiple available cooktops to interact with
    GameObject cooktop;
    bool cooktopRange = false;

    private void Start() {
        interactionMessage = GameObject.Find("InteractionPrompt");
        messageText = interactionMessage.GetComponent<TextMeshProUGUI>();
        SetInteraction(false);

        cooktop = GameObject.Find("cook_station");
    }

    private void Update() {
        if(cooktopRange) {
            if(!cooktop.GetComponent<Cooking>().IsPrepping() && !cooktop.GetComponent<Cooking>().IsCooking()) {
                // use TakeAction function to display a prompt and await user interaction
                if (TakeAction("[C] Cook", KeyCode.C)) {
                    cooktop.GetComponent<Cooking>().StartPrep();
                    SetInteraction(false);
                }
            } else if (cooktop.GetComponent<Cooking>().IsPrepping()) {
                Prompt("Light it up!!!");
            } else if (!cooktop.GetComponent<Cooking>().IsPrepping()) {
                SetInteraction(false);
            }
        }


    }

    public void SetInteraction(bool status) {
        interactionMessage.SetActive(status);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // to create new interactions, add a tag to the collision object in question
        if(collision.tag == "Cooktop" && !cooktop.GetComponent<Cooking>().IsCooking()) {
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
        SetInteraction(true);
        messageText.SetText(prompt);
        if(Input.GetKeyDown(action_keycode)) {
            return true;
        } else {
            return false;
        }
    }

    private void Prompt(string prompt) {
        SetInteraction(true);
        messageText.SetText(prompt);
    }
}
