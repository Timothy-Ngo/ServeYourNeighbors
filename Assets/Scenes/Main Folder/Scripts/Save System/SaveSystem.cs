// Author: Helen Truong

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

/* Adapted from https://youtu.be/aUi9aijvpgs?si=zdMlarwm4Kh3JwqL and https://youtu.be/ijVA5Z-Mbh8?si=Lc4XusD21Mvc_tiT
 adaptations: 
    * used my own method for creating a singleton
    * named class SaveSystem instead of DataPersistenceManager
    * added if statements to OnSceneLoaded() and OnSceneUnloaded() to check for scene names
        * this change prevents a NullReferenceError that comes up when the player tries to go to extra scenes (Tutorial, Credits) with no save data
            * error would happen bc code tries to save data when unloading the Main Menu but there is no current save file to save to
    * instead of checking a bool to initialize data if it was null, data will always be initialized if null if the scene is the Main Scene
        * this change prevents a NullReferenceError that comes up when the game tries to save the game data
            * error would happen bc was not initializing the new game for some reason
    * added bool newGameOnLoad for debugging/development purposes
        * when set to true, save data is reset every time the game is loaded
    * game data is saved in NewGame() after gameData is reset to default values
    * removed functionality of saving data when the game closes
        * could've been used as an exploit to generate money without progressing the game
    * similarly to previous change, removed functionality of saving data when the scene changes
        * could'be been used as an exploit to generate money without progressing the game
*/


public class SaveSystem : MonoBehaviour
{
    [Header("Debugging/Developing")]
    [Tooltip("Change in prefab to make sure it's checked/unchecked across all scenes")]
    [SerializeField] private bool newGameOnLoad = false;

    [Header("File Storage Configuration")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;


    public static SaveSystem inst;
    private void Awake()
    {
        // if there is already a SaveSystem object in the scene
        if (inst != null)
        {
            // destroy this one
            Destroy(this.gameObject);
            return;
        }

        inst = this;
        DontDestroyOnLoad(this.gameObject);


        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

    }


    // use OnEnable and OnDisable to subscribe and unsubscribe scenes to the Scene Manager
    // this is required to do in order to keep track when scenes are loaded and unloaded
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // OnSceneLoaded is called after OnEnable() but before Start()
    // will load game data when a scene is loaded
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Main Menu" || SceneManager.GetActiveScene().name == "Main Scene")
        {
            this.dataPersistenceObjects = FindAllDataPersistenceObjects();

            // loads data when game is started up
            LoadGame();
        }
    }

    /* saving when the scene changes could be used as an exploit
     * ex: serving one customer, getting the money, then using pause menu to return to main menu and then continue game with that money saved but it's the same day with the customer back
    // will save the game data when switching scenes
    public void OnSceneUnloaded(Scene scene)
    {
        if (SceneManager.GetActiveScene().name == "Main Scene")
        {

            SaveGame();
        }
    }
    */

    public void NewGame()
    {
        Debug.Log("New game initialized");
        this.gameData = new GameData();

        
    }

    public void LoadGame()
    {
        // load any saved data from a file using the data handler
        this.gameData = dataHandler.Load(); // if save data doesn't exist, then gameData will be null

        // start a new game if the data is null and player is on Main Scene OR if debug configuration newGameOnLoad is turned on
        if (this.gameData == null && SceneManager.GetActiveScene().name == "Main Scene" || newGameOnLoad)
        {
            NewGame();
            InputSystem.inst.SetDefaultKeybinding(); // completely new game will have default keybinds
            SaveGame();
        }

        // if no data can be loaded, initialize to a new game
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
            return;
        }

        // push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Loaded day: " + gameData.day);
    }


    // is called at the start of each new game day and when leaving the Main Scene
    public void SaveGame()
    {
        // if there is no data to save, log a warning here
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
        }

        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        // save that data to a file using the data handler
        dataHandler.Save(gameData);

        Debug.Log("Saved day: " + gameData.day);
    }

    /* saving when the game closes could be used as an exploit
     * ex: serving one customer, getting the money, then quitting and then start up game         again with that money saved but it's the same day with the customer back
    private void OnApplicationQuit()
    {
        // saves data when game is closed
        SaveGame();
    }
    */

    public void SaveSettings()
    {
        InputSystem.inst.SaveData(gameData);
        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // find all scripts that implement the interface IDataPersistence
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    // used in SceneMgr to check if there is existing game data 
    public bool HasGameData()
    {
        return gameData != null;
    }
}