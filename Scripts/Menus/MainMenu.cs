using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public OptionsMenu optionsMenu;

    void Start()
    {
        optionsMenu.updateSettingPath();
        optionsMenu.Load();
    }

    public void onLevelsButton()
    {
        // TODO
    }

    public void onOptionsButton()
    {
        // TODO
    }

    public void onExitButton()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
