// Author: Helen Truong

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from tutorial: https://youtu.be/aUi9aijvpgs?si=zdMlarwm4Kh3JwqL
// adapted to fit variables for the game

[System.Serializable]
public class GameData
{
    // ----- GameLoop Data ----- //

    // Day Cycle
    public int day;
    public int dailyOperationCost;

    // Foodie Wave Settings
    public int wavesPerDay;
    public int numFoodiesPerWave;

    // ----- Currency Data ----- //

    public int gold;

    // ----- PlayerStat Data ----- //

    // cooking- and serving-related stats
    public int dishesMade;
    public int itemsThrown;
    public int foodiesServed;
    public int msgAdded;
    public int successfulServings;
    public int failedServings;

    // kidnapping- and distraction-related stats
    public int foodiesKidnapped;
    public int foodiesGround;
    public int timesDistracted;
    public int kidnappingsCaught;

    // ----- Upgrades Data ----- //

    public List<Vector3> tablePositions;
    public List<Vector3> cookStationPositions;
    public List<Vector3> counterPositions;

    public bool hasAnimatronic;
    public Vector3 animatronicPosition;

    // ----- SYN Meter ----- //
    public float fillAmount;

    // ----- Input System ----- //
    /*
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    */
    public KeyCode interactKey;
    public KeyCode cookKey;
    public KeyCode kidnapKey;
    public KeyCode serveKey;
    public KeyCode finishPlacementKey;
    public KeyCode pauseKey;
    public KeyCode foodieSightKey;

    // default constructor: sets variables to default values
    // is used when there is no data to load
    public GameData()
    {
        // GameLoop Data
        day = 1;
        dailyOperationCost = 15;
        wavesPerDay = 2;
        numFoodiesPerWave = 1;

        // Currency Data
        gold = 50;

        // PlayerStats Data
        dishesMade = 0;
        itemsThrown = 0;
        foodiesServed = 0;
        msgAdded = 0;
        successfulServings = 0;
        failedServings = 0;
        foodiesKidnapped = 0;
        foodiesGround = 0;
        timesDistracted = 0;
        kidnappingsCaught = 0;

        // Upgrades Data
        tablePositions = new List<Vector3>();
        cookStationPositions = new List<Vector3>();
        counterPositions = new List<Vector3>();
        hasAnimatronic = false;
        animatronicPosition = new Vector3();

        // SYN Meter
        fillAmount = 0f;
    }
}