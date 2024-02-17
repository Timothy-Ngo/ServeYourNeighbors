// Author: Helen Truong
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneMgr : MonoBehaviour
{
    public void ChangeSceneToGame()
    {
        SceneManager.LoadScene("Main Scene");
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
