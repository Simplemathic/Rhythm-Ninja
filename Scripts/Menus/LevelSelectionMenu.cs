using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionMenu : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> musicSamples;
    private bool playing = false;
    private const float volumeSpeed = 3;

    public void onStartLevel(int levelID)
    {
        LevelInformation.songID = levelID;
        SceneManager.LoadSceneAsync("Game");
    }

    // Gently increase/lower music volume played when hovering a level button
    void Update()
    {
        if (playing)
        {
            if (audioSource.volume < Settings.normalizeVolume())
            {
                audioSource.volume += volumeSpeed * Settings.normalizeVolume() * Time.deltaTime;
            }
        }
        else
        {
            if (audioSource.volume > 0)
            {
                audioSource.volume -= volumeSpeed * Settings.normalizeVolume() * Time.deltaTime;
                if (audioSource.volume <= 0)
                    audioSource.Stop();
            }
        }
    }

    // Start playing the music sample for the hovered level
    public void onLevelHover(int levelID)
    {
        audioSource.clip = musicSamples[levelID];
        audioSource.volume = 0;
        playing = true;
        audioSource.Play();
    }

    public void onLevelUnhover(int levelID)
    {
        playing = false;
    }
}