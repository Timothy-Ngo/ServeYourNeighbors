using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Adapted from tutorial: https://youtu.be/aUi9aijvpgs?si=zdMlarwm4Kh3JwqL
    // adapted to fit variables for the game

[System.Serializable]
public class GameData
{
    // ----- GameLoop data -----

    // Day Cycle
    public int day;
    public int dailyOperationCost;

    // Foodie Wave Settings
    public int wavesPerDay;
        //public int currentWaveCount;
    public int numFoodiesPerWave;
        //public int currentFoodiesPerWave;

    // default constructor: sets variables to default values
        // is used when there is no data to load
    public GameData()
    {
        // GameLoop data
        this.day = 1;
        this.dailyOperationCost = 15;
        this.wavesPerDay = 2;
            //this.currentWaveCount = 2;
        this.numFoodiesPerWave = 1;
            //this.currentFoodiesPerWave = 1;


    }
}
