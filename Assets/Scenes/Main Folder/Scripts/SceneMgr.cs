// Author: Helen Truong

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Adapted Start(), ChangeSceneToNewGame(), and ChangeSceneToContinuedGame() from tutorial https://youtu.be/ijVA5Z-Mbh8?si=OvM_nSTCUEZDqXME
// adaptations:
// - added in if statement to check if scene is the main menu in Start()
// - changed LoadSceneAsync() to LoadScene()

public class SceneMgr : MonoBehaviour
{
    [SerializeField] private Button continueGameButton;
    [SerializeField] private GameObject settingsScreen;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            // if there is no saved game data -- disable the continue button 
            if (!SaveSystem.inst.HasGameData())
            {
                continueGameButton.interactable = false;
            }
        }
    }

    public void EnableSettingsScreen()
    {
        settingsScreen.SetActive(true);
        InputSystem.inst.SetLabels();
    }

    public void DisableSettingsScreen()
    {
        settingsScreen.SetActive(false);
        SaveSystem.inst.SaveSettings();
    }

    public void ChangeSceneToGame()
    {
        // load the main scene -- will load the game data because of OnSceneLoaded() in the SaveSystem
        SceneManager.LoadScene("Main Scene");
    }

    public void ChangeSceneToNewGame()
    {
        
        // create a new game -- will initialize the game data
        SaveSystem.inst.NewGame();

        // Save game data after setting game data to default values
        // this fixes an issue where the New Game button wasn't working bc the default values weren't being saved before the file's original contents were loaded
        SaveSystem.inst.SaveGame();


        // load the main scene -- will save the game because of OnSceneUnloaded() in the SaveSystem
        SceneManager.LoadScene("Intro Scene");

    }

    public void ChangeSceneToIntro() 
    {
        SceneManager.LoadScene("Intro Scene");
    }

    public void ChangeSceneToTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void ChangeSceneToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
    }

    public void ChangeSceneToSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void ChangeSceneToCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void ReloadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
