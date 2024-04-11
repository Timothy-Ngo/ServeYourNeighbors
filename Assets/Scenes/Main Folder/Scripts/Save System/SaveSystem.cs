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
    * game data is saved in NewGame() after gameData is reset to default values
    * removed functionality of saving data when the game closes
        * could've been used as an exploit to generate money without progressing the game
    * similarly to previous change, removed functionality of saving data when the scene changes
        * could've been used as an exploit to generate money without progressing the game
    * duplicated code to save Settings data
*/


public class SaveSystem : MonoBehaviour
{

    [Header("File Storage Configuration")]
    [SerializeField] private string fileName;
    [SerializeField] private string settingsFileName;
    [SerializeField] private bool useEncryption;

    public GameData gameData;
    public SettingsData settingsData;

    private List<IDataPersistence> dataPersistenceObjects;
    private List<ISettingsDataPersistence> settingsDataPersistenceObjects;
    private FileDataHandler dataHandler;

    [SerializeField] SkillTree skillTree;


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


        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, settingsFileName, useEncryption);

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
        if (SceneManager.GetActiveScene().name == "Layouts")
        {
            this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        }

        if (SceneManager.GetActiveScene().name == "Main Scene")
        {
            skillTree = GameLoop.inst.GetSkillTreeScreenObject().GetComponent<SkillTree>();
            skillTree.gameObject.SetActive(true);
            this.dataPersistenceObjects = FindAllDataPersistenceObjects();
            this.settingsDataPersistenceObjects = FindAllSettingsDataPersistenceObjects();
            skillTree.gameObject.SetActive(false);

            // loads data when game is started up
            LoadGame();
            LoadSettings();
        }

        if (SceneManager.GetActiveScene().name == "Main Menu" || SceneManager.GetActiveScene().name == "Tutorial")
        {
            this.dataPersistenceObjects = FindAllDataPersistenceObjects();
            this.settingsDataPersistenceObjects = FindAllSettingsDataPersistenceObjects();

            // loads data when game is started up
            LoadGame();
            LoadSettings();
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

    public void NewSettings()
    {
        Debug.Log("New settings initialized");
        this.settingsData = new SettingsData();
    }

    public void LoadGame()
    {
        // load any saved data from a file using the data handler
        this.gameData = dataHandler.Load(); // if save data doesn't exist, then gameData will be null

        // start a new game if the data is null and player is on Main Scene
        if (this.gameData == null && SceneManager.GetActiveScene().name == "Main Scene" )
        {
            Debug.Log("No data was found. A New Game and default settings are being initialized.");
            NewGame();
            SaveGame();
        }

        if(!(SceneManager.GetActiveScene().name == "Tutorial")) {
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
    }

    public void LoadSettings()
    {
        // load any saved data from a file using the data handler
        this.settingsData = dataHandler.LoadSettings(); // if settings data doesn't exist, then settingsData will be null

        if (this.settingsData == null && SceneManager.GetActiveScene().name == "Main Menu" || this.settingsData == null && SceneManager.GetActiveScene().name == "Tutorial")
        {
            Debug.Log("No settings data was found. Default settings are being initialized.");
            NewSettings();
            InputSystem.inst.SetDefaultKeybinding();
            SaveSettings();
        }

        // push the loaded settings data to all other scripts that need it
        foreach (ISettingsDataPersistence dataPersistenceObj in settingsDataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(settingsData);
        }

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
     * ex: serving one customer, getting the money, then quitting and then start up game again with that money saved but it's the same day with the customer back
    private void OnApplicationQuit()
    {
        // saves data when game is closed
        SaveGame();
    }
    */

    
    public void SaveSettings()
    {

        // if there is no data to save, log a warning here
        if (this.settingsData == null)
        {
            Debug.LogWarning("No settings data was found.");
        }

        // pass the data to other scripts so they can update it
        foreach (ISettingsDataPersistence dataPersistenceObj in settingsDataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(settingsData);
        }

        // save that data to a file using the data handler
        dataHandler.SaveSettings(settingsData);
    }
    

    public void SaveSkillTree(SkillTree script)
    {
        script.SaveData(gameData);
    }

    public void LoadSkillTree(SkillTree script)
    {
        script.LoadData(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // find all scripts that implement the interface IDataPersistence
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    private List<ISettingsDataPersistence> FindAllSettingsDataPersistenceObjects()
    {
        // find all scripts that implement the interface IDataPersistence
        IEnumerable<ISettingsDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISettingsDataPersistence>();

        return new List<ISettingsDataPersistence>(dataPersistenceObjects);
    }

    // used in SceneMgr to check if there is existing game data 
    public bool HasGameData()
    {
        return gameData != null;
    }
}