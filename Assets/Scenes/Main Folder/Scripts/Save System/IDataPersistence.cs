using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// used to describe the methods that the implementing script needs to have
public interface IDataPersistence
{
    void LoadData(GameData gameData);
    void SaveData(GameData data);
}
