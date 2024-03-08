using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISettingsDataPersistence
{
    void LoadData(SettingsData gameData);
    void SaveData(SettingsData data);
}
