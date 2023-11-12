// Kirin Hardinger
// November 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour {
    public GameObject item;
    public bool hasItem = false;

    public void set_item(GameObject i) {
        item = i;
        hasItem = true;
    }

    // need to add functionality to set the sprite renderer of the item on the counter
    // need to add functionality of picking the item back up
    public bool full() {
        return hasItem;
    }
}
