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
    private bool tableRange = false;

    [Header("Tables")] 
    [SerializeField] private Table table;
    

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

        if (tableRange)
        {
            // need to add a check for if food is already on the table
            if (TakeAction("[F] Give Order", KeyCode.F))
            {
                // Make dish pop up on table, 
                // change foodie to eating state
                Debug.Log($"current table object: {table.gameObject}");
                table.foodie.orderState.ReceivedOrder();
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
        else if (collision.CompareTag("Table"))
        {
            Debug.Log("Within range of table");
            tableRange = true;
            table = collision.gameObject.transform.parent.GetComponent<Table>();
            Debug.Log($"table: {table}");

        }
        
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.tag == "Cooktop") {
            cooktopRange = false;
            Debug.Log("Out of range of cooktop");
        }
        else if (collision.CompareTag("Table"))
        {
            tableRange = false;
            table = null;
            Debug.Log("Out of range of table");
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
