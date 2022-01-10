using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject levelSelectionMenu;
    public GameObject levelSummaryMenu;

    // Start is called before the first frame update
    void Start()
    {
        if (LevelInformation.songID == null) return;

        if (LevelInformation.songID >= 0)
        {
            levelSummaryMenu.GetComponent<LevelSummaryMenu>().updateSummary();

            mainMenu.SetActive(false);
            optionsMenu.SetActive(false);
            levelSelectionMenu.SetActive(false);
            levelSummaryMenu.SetActive(true);
        }
        else
        {
            Settings.inputOffset = LevelInformation.averageAccuracy;
            optionsMenu.GetComponent<OptionsMenu>().updateOffsetSliderValue();

            mainMenu.SetActive(false);
            optionsMenu.SetActive(true);
            levelSelectionMenu.SetActive(false);
            levelSummaryMenu.SetActive(false);
        }  
    }
}
