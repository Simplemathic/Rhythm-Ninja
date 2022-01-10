using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public void onCloseLevel()
    {
        LevelInformation.songID = null;
        SceneManager.LoadScene("Menu");
    }
}