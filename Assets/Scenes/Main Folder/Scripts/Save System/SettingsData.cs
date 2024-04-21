using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SettingsData
{

    // key rebinding
    public KeyCode interactKey;
    public KeyCode cookKey;
    public KeyCode kidnapKey;
    public KeyCode serveKey;
    public KeyCode finishPlacementKey;
    public KeyCode pauseKey;
    public KeyCode foodieSightKey;
    public KeyCode activateAnimatronicKey;

    // audio settings
    public float masterValue;
    public float musicValue;
    public float audioFXValue;

    public SettingsData()
    {
        interactKey = KeyCode.F;
        cookKey = KeyCode.C;
        kidnapKey = KeyCode.E;
        serveKey = KeyCode.R;
        finishPlacementKey = KeyCode.Space;
        pauseKey = KeyCode.Escape;
        foodieSightKey = KeyCode.LeftShift;
        activateAnimatronicKey = KeyCode.Tab;

        masterValue = 1f;
        musicValue = 1f;
        audioFXValue = 1f;
    }
}
