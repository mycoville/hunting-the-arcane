/*
In this script:
- Miscellaneous functionalities related to the UI
- Joystick activation
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public GameObject gameplayMenu;
    public Player player;
    public List<GameObject> weaponButtonList;
    public GameObject levelPassText;
    public GameObject continueButton;

    // Setting up the script with a static reference for easy access
    public static GameplayManager gpManagerInstance;

    void Awake()
    {
        gpManagerInstance = this;
    }

    void Start()
    {
        gameplayMenu.SetActive(false);
        levelPassText.SetActive(false);
        continueButton.SetActive(false);

        RefreshWeaponButtons();
    }

    public void PassLevel()
    {
        levelPassText.SetActive(true);
        continueButton.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void TrySceneAgain()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void RefreshWeaponButtons()
    {
        // Deactivating buttons
        for(int i = 0; i < weaponButtonList.Count; i++)
        {
            weaponButtonList[i].SetActive(false);
        }

        // Activating buttons for owned weapons 
        for(int i = 0; i < StaticPlayer.acquiredWeapons.Count; i++)
        {
            weaponButtonList[StaticPlayer.acquiredWeapons[i]].SetActive(true);
        }
    }

    public void SwitchWeaponTo(int weaponIndex)
    {
        StaticPlayer.SwitchToWeapon(weaponIndex);
        player.RefreshSprite();
    }


}
