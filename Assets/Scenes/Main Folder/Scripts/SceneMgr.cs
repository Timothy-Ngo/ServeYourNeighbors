// Author: Helen Truong
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Adapted Start(), ChangeSceneToNewGame(), and ChangeSceneToContinuedGame() from tutorial https://youtu.be/ijVA5Z-Mbh8?si=OvM_nSTCUEZDqXME
// - added in if statement to check if scene is the main menu in Start()
// - changed LoadSceneAsync() to LoadScene()

public class SceneMgr : MonoBehaviour
{

    [SerializeField] private Button continueGameButton;
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

    public void ChangeSceneToGame()
    {
        SceneManager.LoadScene("Main Scene");
    }

    public void ChangeSceneToNewGame()
    {
        // create a new game -- will initialize the game data
        SaveSystem.inst.NewGame();

        // load the main scene -- will save the game because of OnSceneUnloaded() in the SaveSystem
        SceneManager.LoadScene("Main Scene");

    }

    public void ChangeSceneToContinuedGame()
    {
        // load the main scene -- will load the game data because of OnSceneLoaded() in the SaveSystem
        SceneManager.LoadScene("Main Scene");
    }

    public void ChangeSceneToIntro() 
    {
        SceneManager.LoadScene("Intro Scene");
    }

    public void ChangeSceneToHowToPlay()
    {
        SceneManager.LoadScene("How To Play");
    }

    public void ChangeSceneToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
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
