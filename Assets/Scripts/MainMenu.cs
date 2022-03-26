using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    //switch to game scene
    public void PlayGame() {
        SceneManager.LoadScene(1);
    }

    //switch to menu scene
    public void Menu() {
        SceneManager.LoadScene(0);
    }

    //close application
    public void QuitGame() {
        Debug.Log("QUIT");
        Application.Quit();
    }

}
