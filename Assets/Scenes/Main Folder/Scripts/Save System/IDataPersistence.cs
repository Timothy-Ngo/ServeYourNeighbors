using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code taken from tutorial: https://youtu.be/aUi9aijvpgs?si=zdMlarwm4Kh3JwqL
    // used to describe the methods that the implementing script needs to have
public interface IDataPersistence
{
    void LoadData(GameData gameData);
    void SaveData(GameData data);
}
