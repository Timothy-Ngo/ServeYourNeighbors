// Kirin Hardinger
// October 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Interaction : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Cooktop") {
            Debug.Log("Within range of cooktop");
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.tag == "Cooktop") {
            Debug.Log("Out of range of cooktop");
        }
    }
}
