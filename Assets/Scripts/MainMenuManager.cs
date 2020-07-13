/*
In this script:
- Miscellaneous functionalities related to the UI
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGameplay()
    {
        StaticPlayer.ResetStats();
        SceneManager.LoadScene("GamePlay");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
