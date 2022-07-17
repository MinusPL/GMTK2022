using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Game exited!");
    }

    public void CreditsButton()
    {
        SceneManager.LoadScene("CreditsSceneDummy");
    }

    public void StartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

}
