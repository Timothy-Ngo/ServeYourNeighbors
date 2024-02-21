using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Adapted from https://youtu.be/aUi9aijvpgs?si=zdMlarwm4Kh3JwqL
// - used my own method for creating a singleton
// - moved code in Start() to Awake()

public class SaveSystem : MonoBehaviour
{
    [Header("File Storage Configuration")]
    [SerializeField] private string fileName;

    private GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;


    public static SaveSystem inst;
    private void Awake()
    {
        inst = this;


        // moved this code from Start() to Awake() so data is loaded before other scripts Start() functions are called
            // there was an issue where the day text wasn't updating to the loaded day value because the GameLoop.cs Start() was called before this Start()
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        // loads data when game is started up
        LoadGame();
    }

    private void Start()
    {
        
    }


    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // load any saved data from a file using the data handler
        this.gameData = dataHandler.Load(); // if save data doesn't exist, then gameData will be null

        // if no data can be loaded, initialize to a new game
        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }

        // push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Loaded day: " + gameData.day);
    }

    public void SaveGame()
    {
        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        // save that data to a file using the data handler
        dataHandler.Save(gameData);

        Debug.Log("Saved day: " + gameData.day);
    }

    private void OnApplicationQuit()
    {
        // saves data when game is closed
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // find all scripts that implement the interface IDataPersistence
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
