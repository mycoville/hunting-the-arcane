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
    public List<GameObject> selectionImages;

    void Start()
    {
        CheckJoyStickType();
    }

    public void StartGameplay()
    {
        StaticPlayer.ResetStats();
        SceneManager.LoadScene("GamePlay");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SelectJoystickType(int typeIndex)
    {
        /*
        Types:
        0 = Floating joystick (default)
        1 = Fixed joystick
        */

        PlayerPrefs.SetInt("joystick-type", typeIndex);
        StaticPlayer.joystickType = typeIndex;
        PlayerPrefs.Save();

        CheckJoyStickType();
    }

    public void CheckJoyStickType()
    {
        for(int i = 0; i < selectionImages.Count; i++)
        {
            selectionImages[i].SetActive(false);
        }

        switch(PlayerPrefs.GetInt("joystick-type"))
        {
            case 0:
                selectionImages[0].SetActive(true);
            break;
            case 1:
                selectionImages[1].SetActive(true);
            break;
            default:
                selectionImages[0].SetActive(true);
            break;
        }
    }
}
