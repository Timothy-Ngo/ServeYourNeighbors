// Kirin Hardinger
// November 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject {
    // cooking- and serving-related stats
    public int dishesMade = 0;
    public int itemsThrown = 0;
    public int foodiesServed = 0;
    public int msgAdded = 0;

    // kidnapping- and distraction-related stats
    public int foodiesKidnapped = 0;
    public int timesDistracted = 0;

    public void reset() {
        dishesMade = 0;
        itemsThrown = 0;
        foodiesServed = 0;
        msgAdded = 0;
        foodiesKidnapped = 0;
        timesDistracted = 0;
    }

    public int getDishesMade() {
        return dishesMade;
    }

    public void setDishesMade(int x) {
        dishesMade = x;
    }

    public void incDishesMade() {
        dishesMade++;
        Debug.Log("dishesMade is now " + dishesMade.ToString());
    }

    public int getItemsThrown() {
        return itemsThrown;
    }

    public void setItemsThrown(int x) {
        itemsThrown = x;
    }

    public void incItemsThrown() {
        itemsThrown++;
        Debug.Log("itemsThrown is now " + itemsThrown.ToString());
    }

    public int getFoodiesServed() {
        return foodiesServed;
    }

    public void setFoodiesServed(int x) {
        foodiesServed = x;
    }

    public void incFoodiesServed() {
        foodiesServed++;
        Debug.Log("foodiesServed is now " + foodiesServed.ToString());
    }

    public int getMSGAdded() {
        return msgAdded;
    }

    public void setMSGAdded(int x) {
        msgAdded = x;
    }

    public void incMSGAdded() {
        msgAdded++;
        Debug.Log("msgAdded is now " + msgAdded.ToString());
    }

    public int getFoodiesKidnapped() {
        return foodiesKidnapped;
    }

    public void setFoodiesKidnapped(int x) {
        foodiesKidnapped = x;
    }

    public void incFoodiesKidnapped() {
        foodiesKidnapped++;
    Debug.Log("foodiesKidnapped is now " + foodiesKidnapped.ToString());
    }

    public int getTimesDistracted() {
        return timesDistracted;
    }

    public void setTimesDistracted(int x) {
        timesDistracted = x;
    }

    public void incTimesDistracted() {
        timesDistracted++;
        Debug.Log("timesDistracted is now " + timesDistracted.ToString());
    }
}
